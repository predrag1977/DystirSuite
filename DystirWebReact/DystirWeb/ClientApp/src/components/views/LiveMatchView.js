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

    onMatchTime() {
        var matchTime = this.matchTimeAndColor.getMatchTime(this.props.match);
        this.setState({ matchTime: matchTime, statusColor: this.matchTimeAndColor.getStatusColor(this.props.match.statusID) });
    }

    matchOnClick(matchID) {
        document.location.href = "/matchdetails/" + matchID;
    }

    render() {
        const match = this.props.match;
        return (
            <div onClick={() => this.matchOnClick(match.matchID) }>
            <NavLink tag={Link} to={"/matchdetails/" + match.matchID}>
                <div key={match.matchID} className="match_details_item">
                    <table className="w-100">
                        <tbody>
                            <tr>
                                <td className="match_time text-center" style={{ whiteSpace: 'nowrap' }}>
                                    {
                                        match.statusID < 14 &&
                                        <div style={{ color: this.state.statusColor }}>{this.state.matchTime}</div>
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table className="w-100 mb-1 mt-1">
                        <tbody>
                            <tr>
                                <td className="match_item_team_name pr-1 text-end">
                                    <span>
                                    {
                                        format('{0} {1} {2}', match.homeTeam, match.homeSquadName, match.homeCategoriesName)
                                    }
                                    </span>
                                </td>
                                <td className="text-center" style={{ width: '30px' }}>
                                    <img onError={(ev) => ev.target.src = "team_logos/empty.png"} src={"team_logos/" + match.homeTeamLogo} width="25" height="25" />
                                </td>
                                <td className="text-center" style={{ width: '20px' }}>-</td>
                                <td className="text-center" style={{ width: '30px' }}>
                                    <img onError={(ev) => ev.target.src = "team_logos/empty.png"} src={("team_logos/" + match.awayTeamLogo)} width="25" height="25" />
                                </td>
                                <td className="match_item_team_name pl-1 text-start">
                                    <span>
                                    {
                                        format('{0} {1} {2}', match.awayTeam, match.awaySquadName, match.awayCategoriesName)
                                    }
                                    </span>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table className="content_table">
                        <tbody>
                            <tr className="match_item_text_color">
                                <td/>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            <td className="text-end" style={{ pading: "0 3px" }}>
                                            {
                                                format("( {0} )", match.homeTeamPenaltiesScore)
                                            }
                                            </td>
                                            || match.statusID == 10 &&
                                            <td className="text-end" style={{ pading: "0 3px" }}>( 0 )</td>
                                        }

                                        <td className="score_match_view">
                                        {
                                            (match.homeTeamScore ?? 0) - (match.homeTeamPenaltiesScore ?? 0)
                                        }
                                        </td>
                                        <td className="text-center" style={{ width: "20px" }}>:</td>
                                        <td className="score_match_view">
                                        {
                                            (match.awayTeamScore ?? 0) - (match.awayTeamPenaltiesScore ?? 0)
                                        }
                                        </td>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            <td className="text-start" style={{ pading: "0 3px" }}>
                                            {
                                                format("( {0} )", match.awayTeamPenaltiesScore)
                                            }
                                            </td>
                                            || match.statusID == 10 &&
                                            <td className="text-start" style={{ pading: "0 3px" }}>( 0 )</td>
                                        }
                                    </>
                                    ||
                                    <>
                                        <td className="score_match_view">-</td>
                                        <td className="text-center" style={{ width: "20px" }}>:</td>
                                        <td className="score_match_view">-</td>
                                    </>
                                }
                                <td />
                            </tr>
                        </tbody>
                    </table>
                </div>
            </NavLink>
            </div>
        );
    }
}
