namespace DystirWeb.Shared
{
    public class MatchFullTime
    {
        public int MatchID { get; private set; }
        public string HomeTeam { get; private set; }
        public string AwayTeam { get; private set; }
        public string Location { get; private set; }
        public string HomeTeamScore { get; private set; }
        public string AwayTeamScore { get; private set; }
        public string HomeTeamPenaltiesScore { get; private set; }
        public string AwayTeamPenaltiesScore { get; private set; }
        public string Competition { get; private set; }
        public string RoundName { get; private set; }
        public string HomeTeamLogo { get; private set; }
        public string AwayTeamLogo { get; private set; }
        public bool IsMatchFinished { get; private set; }

        public MatchFullTime(Matches match)
        {
            MatchID = match.MatchID;
            HomeTeam = $"{match.HomeTeam} {match.HomeSquadName} {match.HomeCategoriesName}".Trim();
            AwayTeam = $"{match.AwayTeam} {match.AwaySquadName} {match.AwayCategoriesName}".Trim();
            Location = match.Location;
            HomeTeamScore = ((match.StatusID ?? 0) == 12 || (match.StatusID ?? 0) == 13) ? ((match.HomeTeamScore ?? 0) - (match.HomeTeamPenaltiesScore ?? 0)).ToString() : "";
            AwayTeamScore = ((match.StatusID ?? 0) == 12 || (match.StatusID ?? 0) == 13) ? ((match.AwayTeamScore ?? 0) - (match.AwayTeamPenaltiesScore ?? 0)).ToString() : "";
            HomeTeamPenaltiesScore = ((match.HomeTeamPenaltiesScore ?? 0) > 0 || (match.AwayTeamPenaltiesScore ?? 0) > 0) ? (match.HomeTeamPenaltiesScore?.ToString() ?? "0") : "";
            AwayTeamPenaltiesScore = ((match.HomeTeamPenaltiesScore ?? 0) > 0 || (match.AwayTeamPenaltiesScore ?? 0) > 0) ? (match.AwayTeamPenaltiesScore?.ToString() ?? "0") : "";
            Competition = match.MatchTypeName;
            RoundName = match.RoundName;
            HomeTeamLogo = match.HomeTeamLogo;
            AwayTeamLogo = match.AwayTeamLogo;
            IsMatchFinished = (match.StatusID ?? 0) == 12 || (match.StatusID ?? 0) == 13;
        }
    }
}
