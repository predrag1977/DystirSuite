namespace Dystir.Models
{
    public class PlayersInLineups
    {
        public PlayerOfMatch HomePlayer { get; set; }
        public PlayerOfMatch AwayPlayer { get; set; }
        public bool IsFirstSub { get; set; } = false;
    }
}