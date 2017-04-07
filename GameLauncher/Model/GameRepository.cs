using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace GameLauncher.Model
{
    class GameRepository
    {
        private const string DbFilename = "games.db";

        public static void Prepare()
        {
            if (File.Exists(DbFilename))
            {
                return;
            }

            using (var db = new SQLiteConnection(DbFilename, false))
            {
                db.CreateTable<Game>();
                db.CreateTable<Launch>();
                db.CreateIndex("Launch", "GameId");
                db.CreateIndex("Launch", "LaunchTime");
            }
        }

        public void PopulateStub()
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                var games = Enumerable.Range(1, 20).Select(i => new Game
                {
                    Name = String.Format("Game{0}", i),
                    ImagePath = "",
                    ExePath = "",
                    Duration = 45
                }).ToList();

                db.InsertAll(games);

                var rnd = new Random();
                var startTime = new DateTime(2016, 5, 1);

                for (int i = 0; i < 5000; i++)
                {
                    startTime = startTime.AddMinutes(rnd.Next(15, 46));

                    var launch = new Launch
                    {
                        GameId = games[rnd.Next() % games.Count].Id,
                        LaunchTime = startTime,
                        QuitTime = startTime.AddSeconds(45)
                    };

                    db.Insert(launch);
                }
            }
        }

        public List<Game> LoadGames(int toSkip, int toTake)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                return db.Table<Game>()
                         .Skip(toSkip)
                         .Take(toTake)
                         .ToList();
            }
        }

        public List<Game> LoadLastGames(int pageSize, out int pageStartPosition)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                var count = db.Table<Game>().Count();

                pageStartPosition = (count > 0) ? count - 1 - (count - 1) % pageSize : 0;

                return db.Table<Game>()
                         .Skip(pageStartPosition)
                         .Take(pageSize + 1)
                         .ToList();
            }
        }

        public void AddGame(Game game)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                db.Insert(game);
            }
        }

        public void UpdateGame(Game game)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                db.Update(game);
            }
        }

        public void DeleteGame(int gameId)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                db.Delete<Game>(gameId);

                // ...and clean the entire history of game launches
                var launches = db.Table<Launch>().Where(l => l.GameId == gameId);
                foreach (var launch in launches)
                {
                    db.Delete<Launch>(launch.Id);
                }
            }
        }

        public void AddLaunch(int gameId, int elapsedTime)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                var launch = new Launch
                {
                    GameId = gameId,
                    LaunchTime = DateTime.Now.AddSeconds(-elapsedTime),
                    QuitTime = DateTime.Now
                };

                db.Insert(launch);
            }
        }

        public List<LaunchInfo> LoadLaunches(DateTime startPeriod, DateTime endPeriod)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                var launches = db.Table<Launch>()
                    .Where(l => l.LaunchTime >= startPeriod && l.LaunchTime <= endPeriod)
                    .ToList();

                return launches.Select(l => new LaunchInfo
                {
                    LaunchDate = l.LaunchTime.ToString("dd MMMM yyyy"),
                    LaunchTime = l.LaunchTime.ToShortTimeString(),
                    QuitTime = l.QuitTime.ToShortTimeString(),
                    Game = db.Get<Game>(l.GameId).Name
                }).ToList();
            }
        }

        public List<GameStats> LoadGamesStats(DateTime startPeriod, DateTime endPeriod, int recentListCount)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                var launches = db.Table<Launch>()
                                 .Where(l => l.LaunchTime >= startPeriod && l.LaunchTime <= endPeriod)
                                 .ToList();

                var gameIds = launches.Select(l => l.GameId)
                                      .Distinct()
                                      .OrderBy(id => id);

                return (from id in gameIds
                    let game = db.Get<Game>(id)
                    let launchCount = launches.Count(l => l.GameId == id)
                    let recentLaunches = launches.Where(l => l.GameId == id)
                                                 .Skip(launchCount - recentListCount)
                                                 .Take(recentListCount)
                                                 .Select(l => l.LaunchTime.ToString("dd MMMM yyyy  [HH:mm]"))
                                                 .ToList()
                    select new GameStats(game)
                    {
                        LaunchCount = launchCount, 
                        RecentLaunches = recentLaunches
                    })
                    .ToList();
            }
        }

        public List<LaunchInfo> LoadGameLaunchData(int gameId, DateTime startPeriod, DateTime endPeriod)
        {
            using (var db = new SQLiteConnection(DbFilename, false))
            {
                var launches = db.Table<Launch>()
                                 .Where(l => l.GameId == gameId && l.LaunchTime >= startPeriod && l.LaunchTime <= endPeriod)
                                 .ToList()
                                 .Select(l => new LaunchInfo
                                 {
                                     LaunchDate = l.LaunchTime.ToString("dd MMMM yyyy"),
                                     LaunchTime = l.LaunchTime.ToShortTimeString(),
                                     QuitTime = l.QuitTime.ToShortTimeString(),
                                     Game = db.Get<Game>(l.GameId).Name
                                 });

                return launches.ToList();
            }
        }
    }
}
