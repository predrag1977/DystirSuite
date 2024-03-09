import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';
import { NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import MatchTimeAndColor from '../../extentions/matchTimeAndColor';

export class LiveMatchView extends Component {
    constructor(props) {
        super(props);
        this.matchTimeAndColor = new MatchTimeAndColor();
        this.state = {
            matchTime: this.matchTimeAndColor.getMatchTime(this.props.match),
            statusColor: this.matchTimeAndColor.getStatusColor(this.props.match.statusID)
        };
    }

    componentDidMount() {
        document.body.addEventListener('onMatchTime', this.onMatchTime.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onMatchTime', this.onMatchTime.bind(this));
    }

    onMatchTime(event) {
        var connected = event.detail.connected;
        var matchTime = this.matchTimeAndColor.getMatchTime(this.props.match);
        var statusColor = this.matchTimeAndColor.getStatusColor(this.props.match.statusID);
        this.setState({ matchTime: matchTime, statusColor: statusColor });
    }

    render() {
        const match = this.props.match;
        const page = this.props.page == "" ? "" : "/" + this.props.page;

        return (
            <NavLink tag={Link} to={page + "/matchdetails/" + match.matchID} style={{padding:"0px"}}>
                <div >
                    <table>
                        <tbody>
                            <tr>
                                <td className="match_info pl-1">
                                    <span>{match.matchTypeName}</span>
                                </td>
                                <td className="match_time" style={{ whiteSpace: "nowrap", width: "0px" }}>
                                    <div style={{ color: this.state.statusColor }}>{this.state.matchTime}</div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table>
                        <tbody>
                            <tr>
                                <td className="match_item_same_day_team_name">
                                    <span>
                                    {
                                        format('{0} {1} {2}', match.homeTeam, match.homeSquadName, match.homeCategoriesName)
                                    }
                                    </span>
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            <td className="penalty_score text-end">
                                            {
                                                match.homeTeamPenaltiesScore
                                            }
                                            </td>
                                            || match.statusID == 10 &&
                                            <td className="text-end" style={{ padding: "0 3px" }}>0</td>
                                        }
                                        <td style={{ whiteSpace: "nowrap", width: "0px" }}>
                                            <div className="score_match_view">
                                            {
                                                (match.homeTeamScore ?? 0) - (match.homeTeamPenaltiesScore ?? 0)
                                            }
                                            </div>
                                        </td>
                                    </>
                                    ||
                                    <td style={{ whiteSpace: "nowrap", width: "0px" }}>
                                        <div className="score_match_view">-</div>
                                    </td>
                                }
                            </tr>
                        </tbody>
                    </table>

                    <table>
                        <tbody>
                            <tr>
                                <td className="match_item_same_day_team_name">
                                    <span>
                                    {
                                        format('{0} {1} {2}', match.awayTeam, match.awaySquadName, match.awayCategoriesName)
                                    }
                                    </span>
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            <td className="penalty_score text-start">
                                            {
                                                match.awayTeamPenaltiesScore
                                            }
                                            </td>
                                            || match.statusID == 10 &&
                                            <td className="text-start" style={{ padding: "0 3px" }}>0</td>
                                        }
                                        <td style={{ whiteSpace: "nowrap", width: "0px" }}>
                                            <div className="score_match_view">
                                            {
                                                (match.awayTeamScore ?? 0) - (match.awayTeamPenaltiesScore ?? 0)
                                            }
                                            </div>
                                        </td>
                                    </>
                                    ||
                                    <td style={{ whiteSpace: "nowrap", width: "0px" }}>
                                        <div className="score_match_view">-</div>
                                    </td>
                                }
                            </tr>
                        </tbody>
                    </table>
                </div>
            </NavLink>
        );
    }
}
