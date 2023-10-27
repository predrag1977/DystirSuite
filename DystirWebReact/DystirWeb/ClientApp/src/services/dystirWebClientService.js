import * as signalR from '@microsoft/signalr';

export default class DystirWebClientService {
    static matchesData = {
        matches: null,
        isLoading: true,
        selectedPeriod: 0
    }

    static resultsData = {
        matches: null,
        isLoading: true
    }

    constructor() {
        console.log("DWCS");
        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                //.withUrl('../dystirhub')
                .withUrl('https://www.dystir.fo/dystirhub')
                .withAutomaticReconnect([0, 1000, 2000, 3000, 5000, 10000])
                .configureLogging(signalR.LogLevel.Information)
                .build()
        };
    }

    init() {
        console.log("init");
        this.hubConnection.connection.on('ReceiveMatchDetails', (matchID, matchDetailsJson) => {
            const matchDetails = matchDetailsJson.replace(/"([^"]+)":/g,
                function ($0, $1) { return ('"' + $1.charAt(0).toLowerCase() + $1.slice(1) + '":'); }
            );

            const match = JSON.parse(matchDetails)['match'];
            const matchDetail = JSON.parse(matchDetails);
            document.body.dispatchEvent(new CustomEvent("onReceiveMatchDetails", { detail: { matchDetail } }));
        });

        this.hubConnection.connection.on('RefreshData', () => {
            document.body.dispatchEvent(new CustomEvent("onRefreshData"));
        });

        this.hubConnection.connection.on('ReceiveMessage', (match, matchJson) => {
            //console.log('ReceiveMessage');
        });
        

        this.hubConnection.connection.onclose(async () => {
            document.body.dispatchEvent(new CustomEvent("onDisconnected"));
            await this.startHubConnection();
        });

        this.hubConnection.connection.onreconnected(() => {
            //console.log('Reconnected');
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
                    this.startHubConnection();
                }, 2000);
            });
    }

    static async loadMatchesDataAsync() {
        const response = await fetch('api/matches/matches');
        const data = await response.json();
        const sortedMatches = data.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));
        DystirWebClientService.matchesData = {
            matches: sortedMatches,
            isLoading: false
        };
        document.body.dispatchEvent(new CustomEvent("onMatchesDataLoaded"));
    }

    static async loadResultDataAsync() {
        const response = await fetch('api/matches/results');
        const data = await response.json();
        const sortedMatches = data
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => b.roundID - a.roundID)
            .sort((a, b) => a.matchTypeID - b.matchTypeID);


        DystirWebClientService.resultsData = {
            matches: sortedMatches,
            isLoading: false
        };
        document.body.dispatchEvent(new CustomEvent("onResultDataLoaded"));
    }
}

