import React, { Component } from 'react';
import * as signalR from '@microsoft/signalr';

export class Matches extends Component {
    static displayName = Matches.name;

    constructor(props) {
        super(props);
        this.state = { matches: [], loading: true };
        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                .withUrl('https://www.dystir.fo/dystirhub')
                .withAutomaticReconnect([0, 100, 1000, 2000, 3000, 5000])
                .configureLogging(signalR.LogLevel.Information)
                .build()
        }
    }

    componentDidMount() {
        this.populateMatchData();

        this.hubConnection.connection.on('ReceiveMatchDetails', (matchID, matchDetailsJson) => {
            alert(matchDetailsJson);
            console.log(matchDetailsJson);
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

        this.start();
    }

    async start() {
        this.hubConnection.connection.start()
            .then(() => console.log('Connection started!'))
            .catch(err => {
                console.log('Error while establishing connection :(');
                setTimeout(this.start(), 5000);
            });
    }

    static renderForecastsTable(matches) {
        return (
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                    </tr>
                </thead>
                <tbody>
                    {matches.map(match =>
                        <tr key={match.matchID}>
                            <td>{match.homeTeam}</td>
                            <td>-</td>
                            <td>{match.awayTeam}</td>
                            <td>{match.homeTeamScore}</td>
                            <td>:</td>
                            <td>{match.awayTeamScore}</td>
                            <td>{match.matchTypeName}</td>
                            <td>{match.location}</td>
                            <td>{match.time}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Matches.renderForecastsTable(this.state.matches);
        return (
            <div>{contents}</div>
        );
    }

    async populateMatchData() {
        const response = await fetch('api/matches');
        const data = await response.json();
        const sortedMatches = data.sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)));
        this.setState({ matches: sortedMatches, loading: false });
    }

    static convertUTCDateToLocalDate() {
        return (
            new Date()
        );
    }
}


