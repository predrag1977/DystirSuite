import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';

export class StandingView extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        var standing = this.props.standing;
        var isSharedPage = this.props.isSharedPage ?? false;
        if (standing.teamStandings === undefined || standing.teamStandings === null) {
            return;
        }
        return (
            <>
                {
                    (isSharedPage == false) &&
                    <div className="match-group-competition-name">{standing.standingCompetitionName ?? ""}</div>
                }
                <div>
                    <div className="standings_table" style={{ margin: "5px 5px " + (isSharedPage ? "2px 5px" : "15px 5px") }}>
                        <table className="content_table">
                            <tbody>
                                <tr id="standings_header" style={{ borderTop: (isSharedPage ? "0px" : "1px solid dimGray") }}>
                                    <td className="standings_cell standings_cell_number" style={{ padding: (isSharedPage ? "7.8px 0px" : "10px 0px")}}>Nr</td>
                                    <td className="standings_cell standings_cell_team_name_title">LIÐ</td>
                                    <td className="standings_cell fw-bold">ST</td>
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
                                        <tr key={teamStanding.teamID} style={{ borderBottom: "1px solid " + (isSharedPage ? "lightGray" : teamStanding.positionColor) }}>
                                            <td className="standings_cell standings_cell_number" style={{ padding: (isSharedPage ? "7.8px 0px" : "12px 0px") }}>{teamStanding.position + "."}</td>
                                            <td className="standings_cell standings_cell_team_name">
                                                <div className={teamStanding.isLive == true ? "border border-success rounded-circle live_standings_indicator" : ""}></div>
                                                <span>{teamStanding.team}</span>
                                            </td>
                                            <td className="standings_cell text-white fw-bold">{teamStanding.points}</td>
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
            </>
        );
    }
}
