import React, { Component } from 'react';
import { GiSoccerBall } from "react-icons/gi";

export class PlayerView extends Component {
    static displayName = PlayerView.name;

    constructor(props) {
        super(props);
    }

    render() {
        const player = this.props.player;
        let contents =
            <div className="player-list-item">
                <table className="content_table" style={{ textAlign: "left" }}>
                    <tbody>
                        <tr>
                            <td className="player_number">
                                <div>{player.number}</div>
                            </td>
                            <td>
                                <div className="player_name">
                                    {((player.firstName ?? "") + " " + (player.lastname ?? "")).trim()}
                                </div>
                                <div>
                                {
                                    player.position == "GK" && <span className="position_text">MM</span> ||
                                    player.position == "DEF" && <span className="position_text">VL</span> ||
                                    player.position == "MID" && <span className="position_text">MV</span> ||
                                    player.position == "ATT" && <span className="position_text">AL</span> ||
                                    <span className="position_text">--</span>
                                }
                                {
                                    player.goal > 0 &&
                                    <>
                                        <GiSoccerBall className="goal_icon" />
                                        <span className="player_statistic_lineups_text">{player.goal}</span>
                                    </>
                                }
                                {
                                    player.ownGoal > 0 &&
                                    <>
                                        <GiSoccerBall className="owngoal_icon" />
                                        <span className="player_statistic_lineups_text">{player.ownGoal}</span>
                                    </>
                                }
                                {
                                    player.yellowCard > 0 &&
                                    <span className="yellow_card player_statistic_lineups_text"></span>
                                }

                                {
                                    player.yellowCard > 1 &&
                                    <span className="yellow_card player_statistic_lineups_text"></span>
                                }

                                {
                                    player.redCard > 0 &&
                                    <span className="red_card player_statistic_lineups_text"></span>
                                }

                                {
                                    player.subIn > -1 &&
                                    <>
                                        <span className="sub_in">&#9650;</span>
                                        <span className="player_statistic_lineups_text">{player.subIn}'</span>
                                    </>
                                }

                                {
                                    player.subOut > -1 &&
                                    <>
                                        <span className="sub_out">&#9660;</span>
                                        <span className="player_statistic_lineups_text">{player.subOut}'</span>
                                    </>
                                }
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

        return contents
    }
}