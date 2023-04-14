using System;
using System.Collections.ObjectModel;

namespace Dystir.Models
{
    public class MatchGroup : ObservableCollection<Match>
    {
        public string HeaderTitle { get; internal set; }

        public MatchGroup(string key, ObservableCollection<Match> group) : base(group)
        {
            HeaderTitle = group?.Count > 0 ? key : string.Empty;
        }
    }
}

