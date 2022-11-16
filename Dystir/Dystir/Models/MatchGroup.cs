using System;

namespace Dystir.Models
{
    public class MatchGroup : List<Match>
    {
        public string HeaderTitle { get; internal set; }

        public MatchGroup(string key, List<Match> group) : base(group)
        {
            HeaderTitle = group?.Count > 0 ? key : string.Empty;
        }
    }
}

