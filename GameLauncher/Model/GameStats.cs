using System.Collections.Generic;

namespace GameLauncher.Model
{
    class GameStats
    {
        private readonly Game _game;

        public GameStats(Game game = null)
        {
            _game = game ?? new Game();
        }

        public int Id
        {
            get { return _game.Id; }
            set { _game.Id = value; }
        }

        public string Name
        {
            get { return _game.Name; }
            set { _game.Name = value; }
        }

        public string ImagePath
        {
            get { return _game.ImagePath; }
            set { _game.ImagePath = value; }
        }

        public int LaunchCount { get; set; }
        public List<string> RecentLaunches { get; set; }
    }
}