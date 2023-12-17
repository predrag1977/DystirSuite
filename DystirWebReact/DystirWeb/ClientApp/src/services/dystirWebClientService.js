import * as signalR from '@microsoft/signalr';

export class DystirWebClientService {
    static instance: DystirWebClientService

    constructor() {
        this.matchesData = {
            matches: [],
            selectedPeriod: "",
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
            eventsOfMatch: [],
            playersOfMatch: [],
            statistic: null,
            standings: []
        };

        this.state = {
            matchesData: this.matchesData,
            resultsData: this.resultsData,
            fixturesData: this.fixturesData,
            standingsData: this.standingsData,
            statisticsData: this.statisticsData,
            matchDetailsData: this.matchDetailsData
        };

        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                .withUrl('../dystirhub')
                //.withUrl('https://www.dystir.fo/dystirhub')
                .withAutomaticReconnect([0, 1000, 2000, 3000, 5000, 10000])
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
        const matches = await response.json();
        this.state.matchesData = {
            matches: matches,
            selectedPeriod: selectedPeriod
        };
        document.body.dispatchEvent(new CustomEvent("onReloadData"));
    }

    async loadResultDataAsync(selectedResultsCompetitionId) {
        const response = await fetch('api/matches/results');
        const data = await response.json();
        const sortedMatches = data
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
        const sortedMatches = data
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => b.roundID - a.roundID)
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

    async loadMatchDetailsDataAsync(matchId) {
        const response = await fetch('api/matchdetails/' + matchId);
        const matchDetails = await response.json();
        const match = matchDetails['match'];
        const eventsOfMatch = matchDetails['eventsOfMatch'];
        const playersOfMatch = matchDetails['playersOfMatch'];
        const statistic = matchDetails['statistic'];
        const standings = matchDetails['standings'];

        this.state.matchDetailsData = {
            matches: this.state.matchesData.matches,
            match: matchDetails['match'],
            matchId: matchId,
            eventsOfMatch: eventsOfMatch,
            playersOfMatch: playersOfMatch,
            statistic: statistic,
            standings: standings
        }
        this.onUpdateMatchDetails(matchDetails);
    }

    onUpdateMatchDetails(matchDetails) {
        const match = matchDetails['match'];
        const eventsOfMatch = matchDetails['eventsOfMatch'];
        const playersOfMatch = matchDetails['playersOfMatch'];
        const statistic = matchDetails['statistic'];
        const standings = matchDetails['standings'];

        this.onUpdateMatch(match);
        this.onUpdateStandings(standings);
    }

    onUpdateMatch(match) {
        const list = this.state.matchesData.matches.filter((item) => item.matchID !== match.matchID);
        list.push(match);

        const sortedMatches = list.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));

        this.state.matchesData = {
            matches: sortedMatches,
            selectedPeriod: this.state.matchesData.selectedPeriod
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

