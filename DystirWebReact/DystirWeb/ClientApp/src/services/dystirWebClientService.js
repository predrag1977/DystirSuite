import * as signalR from '@microsoft/signalr';

export class DystirWebClientService {
    static instance: DystirWebClientService

    constructor() {
        this.selectedPage = "";

        this.matchesData = {
            matches: [],
            selectedPeriod: "",
            isMatchesLoaded: false
        };
        this.resultsData = {
            matches: [],
            selectedResultsCompetitionId: ""
        };
        this.fixturesData = {
            matches: [],
            selectedFixturesCompetitionId: ""
        };
        this.standingsData = {
            standings: [],
            selectedStandingsCompetitionId: ""
        };
        this.statisticsData = {
            statistics: [],
            selectedStatisticsCompetitionId: ""
        };
        this.matchDetailsData = {
            matches: [],
            match: "",
            matchId: "",
            selectedTab: ""
        };
        this.sponsorsData = {
            sponsors: []
        };

        this.state = {
            matchesData: this.matchesData,
            resultsData: this.resultsData,
            fixturesData: this.fixturesData,
            standingsData: this.standingsData,
            statisticsData: this.statisticsData,
            matchDetailsData: this.matchDetailsData,
            sponsorsData: this.sponsorsData
        };

        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                .withUrl('../dystirhub')
                //.withUrl('https://www.dystir.fo/dystirhub')
                .withAutomaticReconnect([0, 1000, 1000, 2000, 3000])
                .configureLogging(signalR.LogLevel.Information)
                .build()
        };
    }

    static getInstance(): DystirWebClientService {
        if (!DystirWebClientService.instance) {
            DystirWebClientService.instance = new DystirWebClientService()
        }

        return DystirWebClientService.instance
    }

    init() {
        this.hubConnection.connection.on('ReceiveMatchDetails', (matchID, matchDetailsJson) => {
            const matchDetailsData = matchDetailsJson.replace(/"([^"]+)":/g,
                function ($0, $1) { return ('"' + $1.charAt(0).toLowerCase() + $1.slice(1) + '":'); }
            );

            const matchDetails = JSON.parse(matchDetailsData);

            this.onUpdateMatchDetails(matchDetails);
        });

        this.hubConnection.connection.on('RefreshData', () => {
            document.body.dispatchEvent(new CustomEvent("onReloadData"));
        });

        this.hubConnection.connection.on('ReceiveMessage', (match, matchJson) => {
            //console.log('ReceiveMessage');
        });

        this.hubConnection.connection.onreconnected(() => {
            //console.log('Reconnected');
        });

        this.hubConnection.connection.onclose(async () => {
            document.body.dispatchEvent(new CustomEvent("onDisconnected"));
            await this.startHubConnection();
        });

        this.startHubConnection();
    }

    async startHubConnection() {
        this.hubConnection.connection.start()
            .then(() => {
                document.body.dispatchEvent(new CustomEvent("onConnected"));
            })
            .catch(err => {
                setTimeout(() => {
                    document.body.dispatchEvent(new CustomEvent("onDisconnected"));
                    this.startHubConnection();
                }, 2000);
            });
    }

    async loadMatchesDataAsync(selectedPeriod) {
        const response = await fetch('api/matches/matches');
        const data = await response.json();

        var matchesList = this.state.matchesData.matches;
        data.forEach((match) => {
            matchesList = matchesList.filter((item) => item.matchID !== match.matchID);
            matchesList.push(match);
        });
        this.state.matchesData = {
            matches: matchesList,
            selectedPeriod: selectedPeriod,
            isMatchesLoaded: true
        };
        document.body.dispatchEvent(new CustomEvent("onReloadData"));
    }

    async loadResultDataAsync(selectedResultsCompetitionId) {
        const response = await fetch('api/matches/results');
        const data = await response.json();

        var matchesList = this.state.matchesData.matches;
        data.forEach((match) => {
            matchesList = matchesList.filter((item) => item.matchID !== match.matchID);
            matchesList.push(match);
        });
        this.state.matchesData.matches = matchesList;
        
        const sortedMatches = this.state.matchesData.matches
            .sort((a, b) => Date.parse(new Date(b.time)) - Date.parse(new Date(a.time)))
            .sort((a, b) => b.roundID - a.roundID)
            .sort((a, b) => a.matchTypeID - b.matchTypeID);

        this.state.resultsData = {
            matches: sortedMatches,
            selectedResultsCompetitionId: selectedResultsCompetitionId
        };
        document.body.dispatchEvent(new CustomEvent("onReloadData"));
    }

    async loadFixturesDataAsync(selectedFixturesCompetitionId) {
        const response = await fetch('api/matches/fixtures');
        const data = await response.json();

        var matchesList = this.state.matchesData.matches;
        data.forEach((match) => {
            matchesList = matchesList.filter((item) => item.matchID !== match.matchID);
            matchesList.push(match);
        });
        this.state.matchesData.matches = matchesList;

        const sortedMatches = this.state.matchesData.matches
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => a.roundID - b.roundID)
            .sort((a, b) => a.matchTypeID - b.matchTypeID);

        this.state.fixturesData = {
            matches: sortedMatches,
            selectedFixturesCompetitionId: selectedFixturesCompetitionId
        };
        document.body.dispatchEvent(new CustomEvent("onReloadData"));
    }

    async loadStandingsDataAsync(selectedStandingsCompetitionId) {
        const response = await fetch('api/standings');
        const standings = await response.json();

        this.state.standingsData = {
            standings: standings,
            selectedStandingsCompetitionId: selectedStandingsCompetitionId
        };
        document.body.dispatchEvent(new CustomEvent("onReloadData"));
    }

    async loadStatisticsDataAsync(selectedStatisticsCompetitionId) {
        const response = await fetch('api/statistics');
        const statistics = await response.json();

        this.state.statisticsData = {
            statistics: statistics,
            selectedStatisticsCompetitionId: selectedStatisticsCompetitionId
        };
        document.body.dispatchEvent(new CustomEvent("onReloadData"));
    }

    async loadMatchDetailsDataAsync(matchId, selectedTab) {
        const response = await fetch('api/matchdetails/' + matchId);
        const matchDetails = await response.json();
        var match = matchDetails['match'];

        if (match != null) {
            match.matchDetails = matchDetails;
        }
        this.state.matchDetailsData = {
            match: match,
            matchId: matchId,
            selectedTab: selectedTab
        }

        this.onUpdateMatchDetails(matchDetails);
    }

    async loadSponsorsDataAsync() {
        const response = await fetch('api/sponsors');
        const sponsors = await response.json();

        this.state.sponsorsData = {
            sponsors: sponsors
        };
        document.body.dispatchEvent(new CustomEvent("onLoadedSponsorsData"));
    }

    onUpdateMatchDetails(matchDetails) {
        console.log(matchDetails);
        var match = matchDetails['match'];
        if (match != null) {
            match.matchDetails = matchDetails;
        }
        this.onUpdateMatch(match);
        this.onUpdateStandings(match?.matchDetails?.standings);
        console.log(match?.matchDetails?.standings);

        var isMatchIdEqual = this.state.matchDetailsData.matchId == match?.matchID;
        this.state.matchDetailsData = {
            matches: matchDetails.matches,
            match: isMatchIdEqual ? match : this.state.matchDetailsData.match,
            matchId: this.state.matchDetailsData.matchId,
            selectedTab: this.state.matchDetailsData.selectedTab
        }

        document.body.dispatchEvent(new CustomEvent("onUpdateMatchDetails"));
    }

    onUpdateMatch(match) {
        const list = this.state.matchesData.matches.filter((item) => item.matchID !== match?.matchID);
        if (match != null) {
            list.push(match);
        }

        const sortedMatches = list.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));

        this.state.matchesData = {
            matches: sortedMatches,
            selectedPeriod: this.state.matchesData.selectedPeriod,
            isMatchesLoaded: this.state.matchesData.isMatchesLoaded
        };
        document.body.dispatchEvent(new CustomEvent("onUpdateMatch"));
    }

    onUpdateStandings(standings) {
        this.state.standingsData = {
            standings: standings,
            selectedStandingsCompetitionId: this.state.standingsData.selectedStandingsCompetitionId
        };
        document.body.dispatchEvent(new CustomEvent("onUpdateStandings"));
    }
}

export const PageName = {
    MATCHES: "DYSTIR",
    RESULTS: "ÚRSLIT",
    FIXTURES: "KOMANDI",
    STANDINGS: "STØÐAN",
    STATISTICS: "HAGTØL"
}

export const SelectPeriodName = {
    BEFORE: "before",
    YESTERDAY: "yesterday",
    TODAY: "today",
    TOMORROW: "tomorrow",
    NEXT: "next"
}

export const TabName = {
    SUMMARY: "summary",
    LINEUPS: "lineups",
    COMMENTARY: "commentary",
    STATISTICS: "statistics",
    STANDINGS: "standings"
}

export const EventName = {
    GOAL: "GOAL",
    OWNGOAL: "OWNGOAL",
    PENALTYSCORED: "PENALTYSCORED",
    PENALTYMISSED: "PENALTYMISSED",
    PENALTY: "PENALTY",
    PLAYEROFTHEMATCH: "PLAYEROFTHEMATCH",
    BIGCHANCE: "BIGCHANCE",
    YELLOW: "YELLOW",
    RED: "RED",
    SUBSTITUTION: "SUBSTITUTION",
    ASSIST: "ASSIST",
    CORNER: "CORNER",
    ONTARGET: "ONTARGET",
    OFFTARGET: "OFFTARGET",
    BLOCKEDSHOT: "BLOCKEDSHOT",
    PLAYEROFTHEMATCH: "PLAYEROFTHEMATCH"
}

