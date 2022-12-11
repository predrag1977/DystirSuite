using System;

namespace Dystir.Models
{
    public class PlayersInRowGroup : List<PlayersInRow>
    {
        public bool ShowSubtitutionLine { get; set; }
        public double HeaderHeight { get; set; }

        public PlayersInRowGroup(int key, List<PlayersInRow> group) : base(group)
        {
            ShowSubtitutionLine = group?.FirstOrDefault(x => x.IsFirstSub) != null;
            HeaderHeight = ShowSubtitutionLine ? 20 : 0;
        }
    }
}

