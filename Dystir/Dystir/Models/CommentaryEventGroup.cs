using System;
namespace Dystir.Models
{
    public class CommentaryEventGroup : List<SummaryEventOfMatch>
    {
        public string EventText { get; set; }

        public CommentaryEventGroup(int key, List<SummaryEventOfMatch> group) : base(group)
        {
            EventText = group?.FirstOrDefault()?.EventOfMatch?.EventText;
        }
    }
}

