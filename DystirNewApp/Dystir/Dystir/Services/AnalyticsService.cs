using System.Collections.Generic;
using System.Threading.Tasks;
using Dystir.Models;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Dystir.Services
{
	public class AnalyticsService
	{
		internal async void StartAnalytics()
        {
            await Task.Run(() =>
            {
                AppCenter.Start("ios=3e8bf986-c3f6-49be-8d24-e75c9d9b0a69;" +
                  "uwp={Your UWP App secret here};" +
                  "android=f8d197dc-a359-4e1c-8afe-f9487281f374;",
                  typeof(Analytics), typeof(Crashes));
            });
        }

        internal void Matches()
        {
            Analytics.TrackEvent("Matches");
        }

        internal void Results()
        {
            Analytics.TrackEvent("Results");
        }

        internal void Fixtures()
        {
            Analytics.TrackEvent("Fixtures");
        }

        internal void Standings()
        {
            Analytics.TrackEvent("Standings");
        }

        internal void Statistics()
        {
            Analytics.TrackEvent("Statistics");
        }

        internal void MatchDetails(Match match)
        {
            string homeTeam = $"{match?.HomeTeam} {match?.HomeCategoriesName} {match?.HomeSquadName}".Trim();
            string awayTeam = $"{match?.AwayTeam} {match?.AwayCategoriesName} {match?.AwaySquadName}".Trim();
            string matchDetails = $"{homeTeam} vs {awayTeam} ({match.MatchTypeName})";
            Analytics.TrackEvent("MatchDetails", new Dictionary<string, string> { { "Match", matchDetails } });
        }
    }
}

