using System;
using SQLite;

namespace GameLauncher.Model
{
    class Launch
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }

        [NotNull]
        public int GameId { get; set; }

        [NotNull]
        public DateTime LaunchTime { get; set; }

        [NotNull]
        public DateTime QuitTime { get; set; }
    }
}
