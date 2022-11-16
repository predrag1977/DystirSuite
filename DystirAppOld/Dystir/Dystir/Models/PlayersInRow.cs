namespace Dystir.Models
{
    public class PlayersInRow
    {
        public int? PlayingStatus { get; internal set; }
        public PlayerOfMatch FirstPlayer { get; internal set; } = new PlayerOfMatch();
        public PlayerOfMatch SecondPlayer { get; internal set; } = new PlayerOfMatch();
    }
}