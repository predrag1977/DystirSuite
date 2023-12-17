import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { PlayerView } from './views/PlayerView';

export class Lineups extends Component {
    static displayName = Lineups.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        const homePlayers = this.props.playersOfMatch.filter((player) => player.teamName == match.homeTeam);
        const awayPlayers = this.props.playersOfMatch.filter((player) => player.teamName == match.awayTeam);
        const playersCount = this.props.playersOfMatch.filter((player) => player.playingStatus == 1 || player.playingStatus == 2).length;
        let contents =
            <table className="lineups content_table">
                <tbody>
                    <tr>
                        <td style={{textAlign: "center"}, {verticalAlign: "top"}, {padding: "5px 0"}}>
                        {
                            homePlayers.filter((player) => player.playingStatus == 1).map((player) =>
                                <PlayerView key={player.playerId} player={player} />
                            )
                        }
                        </td>
                        <td style={{ textAlign: "center" }, { verticalAlign: "top" }, { padding: "5px 0" }}>
                        {
                            awayPlayers.filter((player) => player.playingStatus == 1).map((player) =>
                                <PlayerView key={player.playerId} player={player} />
                            )
                        }
                        </td>
                    </tr>
                    {
                        playersCount > 0 &&
                        <>
                            <tr>
                                <td>
                                    <div className="lineups_subs_line" />
                                </td>
                                <td>
                                    <div className="lineups_subs_line" />
                                </td>
                            </tr>

                            <tr>
                                <td style={{ textAlign: "center" }, { verticalAlign: "top" }, { padding: "5px 0" }}>
                                {
                                    homePlayers.filter((player) => player.playingStatus == 2).map((player) =>
                                        <PlayerView key={player.playerId} player={player} />
                                    )
                                }
                                </td>
                                <td style={{ textAlign: "center" }, { verticalAlign: "top" }, { padding: "5px 0" }}>
                                {
                                    awayPlayers.filter((player) => player.playingStatus == 2).map((player) =>
                                        <PlayerView key={player.playerId} player={player} />
                                    )
                                }
                                </td>
                            </tr>
                        </>
                    }
                </tbody>
            </table>

        return contents
    }
}