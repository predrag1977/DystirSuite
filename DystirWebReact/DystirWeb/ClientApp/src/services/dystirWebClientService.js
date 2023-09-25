import * as signalR from '@microsoft/signalr';

export default class DystirWebClientService {
    static matchesData = {
        matches: null,
        isMatchesLoading: false
    }

    static resultsData = {
        matches: null,
        isMatchesLoading: false
    }

    constructor() {
        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                .withUrl('../dystirhub')
                //.withUrl('https://www.dystir.fo/dystirhub')
                .withAutomaticReconnect([0, 1000, 2000, 3000, 5000, 10000])
                .configureLogging(signalR.LogLevel.Information)
                .build()
        };
        
    }

    init() {
        this.hubConnection.connection.on('ReceiveMatchDetails', (matchID, matchDetailsJson) => {
            //if (DystirWebClientService.allMatches === null) {
            //    return;
            //}
            const matchDetails = matchDetailsJson.replace(/"([^"]+)":/g,
                function ($0, $1) { return ('"' + $1.charAt(0).toLowerCase() + $1.slice(1) + '":'); }
            );

            const match = JSON.parse(matchDetails)['match'];
            const matchDetail = JSON.parse(matchDetails);

            //const list = DystirWebClientService.allMatches.filter((item) => item.matchID !== match.matchID)

            //list.push(match);

            //DystirWebClientService.allMatches = list

            //const sortedMatches = DystirWebClientService.allMatches;
            //document.body.dispatchEvent(new CustomEvent("onFullDataLoaded", { detail: { sortedMatches } }));
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

    //async loadFullDataAsync() {
    //    const response = await fetch('api/matches');
    //    const data = await response.json();
    //    const sortedMatches = data.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));
    //    DystirWebClientService.allMatches = sortedMatches;
    //    document.body.dispatchEvent(new CustomEvent("onFullDataLoaded", { detail: { sortedMatches } }));
    //}
}

