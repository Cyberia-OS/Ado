using System;

namespace GameModels
{
    public enum GameMode
    {
        SinglePlayer,
        Multiplayer,
        Both
    }

    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Studio { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }

        public GameMode Mode { get; set; }    
        public int CopiesSold { get; set; }    
    }
}