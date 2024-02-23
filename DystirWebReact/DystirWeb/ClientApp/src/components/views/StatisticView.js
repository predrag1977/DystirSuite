import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';

export class StatisticView extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const statistic = this.props.statistic;
        const goalPlayers = statistic.goalPlayers ?? [];
        const assistPlayers = statistic.assistPlayers ?? [];
        return (
            <div>
                <table className="lineups content_table">
                    <tbody>
                        <tr>
                            <td className="statistics_title">
                                <span>Málskjúttar</span>
                                <div className="statistics_border" />
                            </td>
                            <td className="statistics_title">
                                <span>Upplegg</span>
                                <div className="statistics_border" />
                            </td>
                        </tr>
                        <tr>
                            <td className="align-top">
                            {
                                goalPlayers.map((player, index) =>
                                    <div key={index} className="player-list-item">
                                        <table className="content_table text-left">
                                            <tbody>
                                                <tr>
                                                    <td className="statistics_number">
                                                        {`${index + 1}.`}
                                                    </td>
                                                    <td>
                                                        <div className="player_name">
                                                            {(player.firstName + " " + (player.lastname ?? "")).trim()}
                                                        </div>
                                                        <div className="statistics_team">
                                                            {player.teamName}
                                                        </div>
                                                    </td>
                                                    <td className="statistics_goal">{player.goal}</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                )
                            }
                            </td>

                            <td className="align-top">
                            {
                                assistPlayers.map((player, index) =>
                                    <div key={index} className="player-list-item">
                                        <table className="content_table text-left">
                                            <tbody>
                                                <tr>
                                                    <td className="statistics_number">
                                                        {`${index + 1}.`}
                                                    </td>
                                                    <td>
                                                        <div className="player_name">
                                                            {(player.firstName + " " + (player.lastname ?? "")).trim()}
                                                        </div>
                                                        <div className="statistics_team">
                                                            {player.teamName}
                                                        </div>
                                                    </td>
                                                    <td className="statistics_goal">{player.assist}</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                )
                            }
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}
