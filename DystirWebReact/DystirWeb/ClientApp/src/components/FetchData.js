import React, { Component } from 'react';
import * as signalR from '@microsoft/signalr';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { matches: [], loading: true };
        this.hubConnection = {
            connection: new signalR.HubConnectionBuilder()
                .withUrl('/dystirhub')
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
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
            <tbody>
                {
                    matches.map(match =>
                <tr key={match.matchID}>

                            <td>{match.matchTypeName}</td>
                </tr>

                )}
            </tbody>
        </table>
      );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.matches);

      return (

      <div>
        <h1 id="tableLabel">Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateMatchData() {
    const response = await fetch('api/matches?action=test');
    const data = await response.json();
      this.setState({ matches: data, loading: false });
  }
}
