import * as signalR from '@microsoft/signalr';

export class DystirWebClientService {
    static instance: DystirWebClientService

    constructor() {
        this.matchesData = {
            matches: null,
            isLoading: true,
            selectedPeriod: ""
        };
        this.resultsData = {
            matches: null,
            isLoading: true
        };
        this.fixturesData = {
            matches: null,
            isLoading: true
        };
        this.standingsData = {
            standings: null,
            isLoading: true
        };
        this.state = {
            matchesData: this.matchesData,
            resultsData: this.resultsData,
            fixturesData: this.fixturesData,
            standingsData: this.standingsData
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
            const matchDetails = matchDetailsJson.replace(/"([^"]+)":/g,
                function ($0, $1) { return ('"' + $1.charAt(0).toLowerCase() + $1.slice(1) + '":'); }
            );

            const match = JSON.parse(matchDetails)['match'];
            const matchDetail = JSON.parse(matchDetails);
            this.onReceiveMatch(match);
        });

        this.hubConnection.connection.on('RefreshData', () => {
            this.loadMatchesDataAsync(this.state.matchesData.selectedPeriod);
        });

        this.hubConnection.connection.on('ReceiveMessage', (match, matchJson) => {
            //console.log('ReceiveMessage');
        });

        this.hubConnection.connection.onreconnected(() => {
            //console.log('Reconnected');
        });

        this.hubConnection.connection.onclose(async () => {
            this.state.matchesData = {
                matches: this.state.matchesData.matches,
                isLoading: true,
                selectedPeriod: this.state.matchesData.selectedPeriod
            };
            document.body.dispatchEvent(new CustomEvent("onDisconnected"));
            await this.startHubConnection();
        });

        this.startHubConnection();
    }

    async startHubConnection() {
        this.hubConnection.connection.start()
            .then(() => {
                this.loadMatchesDataAsync(this.state.matchesData.selectedPeriod);
            })
            .catch(err => {
                setTimeout(() => {
                    this.startHubConnection();
                }, 2000);
            });
    }

    async loadMatchesDataAsync(selectedPeriod) {
        const response = await fetch('api/matches/matches');
        const matches = await response.json();
        this.state.matchesData = {
            matches: matches,
            isLoading: false,
            selectedPeriod: selectedPeriod
        };
        document.body.dispatchEvent(new CustomEvent("onMatchesDataLoaded"));
    }

    async loadResultDataAsync() {
        const response = await fetch('api/matches/results');
        const data = await response.json();
        const sortedMatches = data
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => b.roundID - a.roundID)
            .sort((a, b) => a.matchTypeID - b.matchTypeID);

        this.state.resultsData = {
            matches: sortedMatches,
            isLoading: false
        };
        document.body.dispatchEvent(new CustomEvent("onResultsDataLoaded"));
    }

    async loadFixturesDataAsync() {
        const response = await fetch('api/matches/fixtures');
        const data = await response.json();
        const sortedMatches = data
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => b.roundID - a.roundID)
            .sort((a, b) => a.matchTypeID - b.matchTypeID);


        this.state.fixturesData = {
            matches: sortedMatches,
            isLoading: false
        };
        document.body.dispatchEvent(new CustomEvent("onFixturesDataLoaded"));
    }

    async loadStandingsDataAsync() {
        const response = await fetch('api/standings');
        const standings = await response.json();

        this.state.standingsData = {
            standings: standings,
            isLoading: false
        };
        console.log(standings);
        document.body.dispatchEvent(new CustomEvent("onStandingsDataLoaded"));
    }

    onReceiveMatch(match) {
        const list = this.state.matchesData.matches.filter((item) => item.matchID !== match.matchID)

        list.push(match);

        const sortedMatches = list.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));

        this.state.matchesData = {
            matches: sortedMatches,
            isLoading: false,
            selectedPeriod: this.state.matchesData.selectedPeriod
        };
        document.body.dispatchEvent(new CustomEvent("onMatchesDataLoaded"));
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

