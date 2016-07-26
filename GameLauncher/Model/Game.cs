using SQLite;

namespace GameLauncher.Model
{
    class Game
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }

        [MaxLength(100), NotNull]
        public string Name { get; set; }

        [MaxLength(255), NotNull]
        public string ExePath { get; set; }

        [MaxLength(255), NotNull]
        public string ImagePath { get; set; }

        [NotNull]
        public int Duration { get; set; }
    }
}
