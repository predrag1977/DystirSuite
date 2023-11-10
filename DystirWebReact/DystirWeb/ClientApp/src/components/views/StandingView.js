import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';

export class StandingView extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const standing = this.props.standing;
        return (
            <div id="main_container">
                <div className="standings_table pt-2">
                    <table className="content_table">
                        <tbody>
                            <tr id="standings_header">
                                <td className="standings_cell standings_cell_number">Nr</td>
                                <td className="standings_cell standings_cell_team_name_title">LIÐ</td>
                                <td className="standings_cell font-weight-bold">ST</td>
                                <td className="standings_cell">DS</td>
                                <td className="standings_cell">VD</td>
                                <td className="standings_cell">JD</td>
                                <td className="standings_cell">TD</td>
                                <td className="standings_cell">MF</td>
                                <td className="standings_cell">MÍ</td>
                                <td className="standings_cell">MM</td>
                            </tr>
                            {
                                standing.teamStandings.map(teamStanding =>
                                    <tr key={teamStanding.teamID} style={{ borderBottom: "1px solid " + teamStanding.positionColor }}>
                                        <td className="standings_cell standings_cell_number">{teamStanding.position + "."}</td>
                                        <td className="standings_cell standings_cell_team_name">
                                            <div className={teamStanding.isLive == true ? "border border-success rounded-circle live_standings_indicator" : ""}></div>
                                            <span>{teamStanding.team}</span>
                                        </td>
                                        <td className="standings_cell text-white font-weight-bold">{teamStanding.points}</td>
                                        <td className="standings_cell">{teamStanding.matchesNo}</td>
                                        <td className="standings_cell">{teamStanding.victories}</td>
                                        <td className="standings_cell">{teamStanding.draws}</td>
                                        <td className="standings_cell">{teamStanding.losses}</td>
                                        <td className="standings_cell">{teamStanding.goalScored}</td>
                                        <td className="standings_cell">{teamStanding.goalAgainst}</td>
                                        <td className="standings_cell">{teamStanding.goalDifference}</td>
                                    </tr>
                                )
                            }
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }
}
