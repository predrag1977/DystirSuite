import * as signalR from '@microsoft/signalr';

export default class DystirWebClientService {
    static matches = null;
    
    constructor() {
        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                .withUrl('https://www.dystir.fo/dystirhub')
                .withAutomaticReconnect([0, 100, 1000, 2000, 3000, 5000])
                .configureLogging(signalR.LogLevel.Information)
                .build()
        };
    }

    init() {
        this.hubConnection.connection.on('ReceiveMatchDetails', (matchID, matchDetailsJson) => {
            if (DystirWebClientService.matches === null) {
                return;
            }
            const matchDetails = matchDetailsJson.replace(/"([^"]+)":/g,
                function ($0, $1) { return ('"' + $1.charAt(0).toLowerCase() + $1.slice(1) + '":'); }
            );

            const match = JSON.parse(matchDetails)['match'];

            const list = DystirWebClientService.matches.filter((item) => item.matchID !== match.matchID)

            list.push(match);

            DystirWebClientService.matches = list

            const sortedMatches = DystirWebClientService.matches;
            document.body.dispatchEvent(new CustomEvent("onFullDataLoaded", { detail: { sortedMatches } }));
        });

        this.hubConnection.connection.on('RefreshData', () => {
            console.log('RefreshData');
        });

        this.hubConnection.connection.onclose(async () => {
            console.log('Disconnected');
            await this.start();
        });

        this.hubConnection.connection.onreconnected(() => {
            console.log('Reconnected');
        });

        this.startHubConnection();
    }

    async startHubConnection() {
        this.hubConnection.connection.start()
            .then(() => this.loadFullDataAsync())
            .catch(err => {
                console.log(err);
                setTimeout(this.start(), 1000);
            });
    }

    async loadFullDataAsync() {
        const response = await fetch('api/matches');
        const data = await response.json();
        const sortedMatches = data.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));
        DystirWebClientService.matches = sortedMatches;
        document.body.dispatchEvent(new CustomEvent("onFullDataLoaded", { detail: { sortedMatches } }));
    }
}