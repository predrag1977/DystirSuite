using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dystir.Models
{
    public class CommentaryEventGroup : ObservableCollection<SummaryEventOfMatch>
    {
        string eventText = string.Empty;
        public string EventText
        {
            get { return eventText; }
            set { eventText = value; }
        }

        public CommentaryEventGroup(int key, ObservableCollection<SummaryEventOfMatch> group) : base(group)
        {
            EventText = group?.FirstOrDefault()?.EventOfMatch?.EventText;
        }
    }
}

