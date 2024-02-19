import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';
import { NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import MatchTimeAndColor from '../../extentions/matchTimeAndColor';

export class MatchView extends Component {
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

    render() {
        const match = this.props.match;
        const page = this.props.page;
        var link = "/" + page + "/matchdetails/";
        if (page == "" || page == undefined) {
            link = "/matchdetails/";
        }
        return (
            <NavLink tag={Link} to={link + match.matchID}>
                <div key={match.matchID} className="match_item">
                    <table className="w-100">
                        <tbody>
                            <tr>
                                <td className="match_info text-start">
                                    <span>{(new MatchDate(Date.parse(match.time)).toDateTimeString())}</span>
                                    {
                                        (match.matchTypeName?.trim() !== undefined && match.matchTypeName?.trim() !== "") && <span> - </span>
                                    }
                                    <span>{match.matchTypeName}</span>
                                    {
                                        (match.roundName?.trim() !== undefined && match.roundName?.trim() !== "") && <span> - </span>
                                    }
                                    <span>{match.roundName}</span>
                                    {
                                        (match.location?.trim() !== undefined && match.location?.trim() !== "") && <span> - </span>
                                    }
                                    <span>{match.location}</span>
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
                                <td className="match_time text-start" style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                {
                                    match.statusID < 14 &&
                                    <div style={{ color: this.state.statusColor }}>{this.state.matchTime}</div>
                                }
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            <td className="text-end" style={{ padding: "0 10px 0 3px" }}>
                                            {
                                                match.homeTeamPenaltiesScore
                                            }
                                            </td>
                                            || match.statusID == 10 &&
                                            <td className="text-end" style={{ padding: "0 3px" }}>( 0 )</td>
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
                                            <td className="text-start" style={{ padding: "0 3px 0 10px" }}>
                                            {
                                                match.awayTeamPenaltiesScore
                                            }
                                            </td>
                                            || match.statusID == 10 &&
                                            <td className="text-start" style={{ padding: "0 3px" }}>( 0 )</td>
                                        }
                                    </>
                                    ||
                                    <>
                                        <td className="score_match_view">-</td>
                                        <td className="text-center" style={{ width: "20px" }}>:</td>
                                        <td className="score_match_view">-</td>
                                    </>
                                }
                                <td className="text-right" />
                                </tr>
                        </tbody>
                    </table>
                </div>

                <div className="match_item_mobile">
                    <table className="w-100">
                        <tbody>
                            <tr>
                                <td className="match_info text-start">
                                    <span>{(new MatchDate(Date.parse(match.time)).toDateTimeString())}</span>
                                    {
                                        (match.matchTypeName?.trim() !== undefined && match.matchTypeName?.trim() !== "") && <span> - </span>
                                    }
                                    <span>{match.matchTypeName}</span>
                                    {
                                        (match.roundName?.trim() !== undefined && match.roundName?.trim() !== "") && <span> - </span>
                                    }
                                    <span>{match.roundName}</span>
                                    {
                                        (match.location?.trim() !== undefined && match.location?.trim() !== "") && <span> - </span>
                                    }
                                    <span>{match.location}</span>
                                </td>

                                <td className="match_time" style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                {
                                    match.statusID < 14 &&
                                    <div style={{ color: this.state.statusColor }}>{this.state.matchTime}</div>
                                }
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <img onError={(ev) => ev.target.src = "team_logos/empty.png"} src={("team_logos/" + match.homeTeamLogo)} width="25" height="25" />
                                </td>
                                <td className="match_item_same_day_team_name" style={{ padding: "2px 5px" }}>
                                    <span>{format('{0} {1} {2}', match.homeTeam, match.homeSquadName, match.homeCategoriesName)} </span>
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <td style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                        <div style={{ padding: '0 5px' }}>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            match.homeTeamPenaltiesScore
                                            || match.statusID == 10 &&
                                            "(0)"
                                        }
                                        </div>
                                    </td>
                                }
                                <td style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                    <div className="score_match_view">
                                        {
                                            (match.statusID < 14 && match.statusID > 1) &&
                                            <>
                                            {
                                                (match.homeTeamScore ?? 0) - (match.homeTeamPenaltiesScore ?? 0)
                                            }
                                            </>
                                            ||
                                            "-"
                                        }
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <img onError={(ev) => ev.target.src = "team_logos/empty.png"} src={("team_logos/" + match.awayTeamLogo)} width="25" height="25" />
                                </td>
                                <td className="match_item_same_day_team_name" style={{ padding: "2px 5px" }}>
                                    <span>{format('{0} {1} {2}', match.awayTeam, match.awaySquadName, match.awayCategoriesName)}</span>
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <td style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                        <div style={{ padding: '0 5px' }}>
                                        {
                                            ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                            match.awayTeamPenaltiesScore
                                            || match.statusID == 10 &&
                                            "(0)"
                                        }
                                        </div>
                                    </td>
                                }
                                <td style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                    <div className="score_match_view">
                                        {
                                            (match.statusID < 14 && match.statusID > 1) &&
                                            <>
                                            {
                                                (match.awayTeamScore ?? 0) - (match.awayTeamPenaltiesScore ?? 0)
                                            }
                                            </>
                                            ||
                                            "-"
                                        }
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </NavLink>
        );
    }
}
