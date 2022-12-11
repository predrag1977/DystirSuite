namespace Dystir.Models
{
    public class PlayersInRow
    {
        public PlayerOfMatch HomePlayer { get; internal set; }
        public PlayerOfMatch AwayPlayer { get; internal set; }
        public bool IsFirstSub { get; internal set; } = false;
        public int RowIndex { get; internal set; }
    }
}