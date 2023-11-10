import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';

export class MatchView extends Component {
    constructor(props) {
        super(props);
        this.state = {
            matchTime: this.getMatchTime(this.props.match),
            statusColor: this.getStatusColor(this.props.match.statusID)
        };
    }

    componentDidMount() {
        document.body.addEventListener('onMatchTime', this.onMatchTime.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onMatchTime', this.onMatchTime.bind(this));
    }

    onMatchTime() {
        var matchTime = this.getMatchTime(this.props.match);
        this.setState({ matchTime: matchTime, statusColor: this.getStatusColor(this.props.match.statusID) });
    }

    getMatchTime(match) {
        var timeNow = new MatchDate().dateUtc().getTime();
        var matchTime = new MatchDate(match.statusTime).getTime();
        var matchStart = new MatchDate(match.time).getTime();

        var totalMiliseconds = timeNow - matchTime;
        var seconds = Math.floor(totalMiliseconds / 1000);
        var minutes = Math.floor(seconds / 60);
        seconds -= minutes * 60;
        var milsecToStart = matchStart - timeNow;

        return this.getMatchPeriod(minutes, seconds, match.statusID, milsecToStart);
    }

    getMatchPeriod(minutes, seconds, matchStatus, milsecToStart) {
        var addtime = "";
        switch (matchStatus) {
            case 1:
                return this.getTimeToStart(milsecToStart, "00:00");
            case 2:
                if (minutes >= 45) {
                    addtime = "45+";
                    minutes = minutes - 45;
                }
                break;
            case 3:
                return "hálvleikur";
            case 4:
                minutes = minutes + 45;
                if (minutes >= 90) {
                    addtime = "90+";
                    minutes = minutes - 90;
                }
                break;
            case 5:
                return "liðugt";
            case 6:
                minutes = minutes + 90;
                if (minutes >= 105) {
                    addtime = "105+";
                    minutes = minutes - 105;
                }
                break;
            case 7:
                return "longd leiktíð hálvleikur";
            case 8:
                minutes = minutes + 105;
                if (minutes >= 120) {
                    addtime = "120+";
                    minutes = minutes - 120;
                }
                break;
            case 9:
                return "longd leiktíð liðugt";
            case 10:
                return "brotsspark";
            case 11:
            case 12:
            case 13:
                return "liðugt";
            default:
                return this.getTimeToStart(milsecToStart, "-- : --");
        }
            var min = minutes;
            var sec = seconds;
        if (minutes < 10)
            min = "0" + minutes;
        if (seconds < 10)
            sec = "0" + seconds;
        return addtime + " " + min + ":" + sec;
    }

    getTimeToStart(milsecToStart, defaultText) {
        var minutesToStart = Math.ceil(milsecToStart / 60000);
        if (minutesToStart > 0) {
            var days = Math.floor(minutesToStart / 1440);
            var hours = Math.floor((minutesToStart - days * 1440) / 60);
            var minutes = minutesToStart - days * 1440 - hours * 60;
            if (days > 0) {
                return `${days} d. ${hours} t.`;
            }
            else {
                var hoursText = hours > 0 ? hours + " t. " : "";
                return hoursText + minutes + " m.";
            }
        }
        else {
            return defaultText;
        }
    }

    getStatusColor(statusId) {
        switch (statusId) {
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                return "limegreen";
            case 11:
            case 12:
            case 13:
                return "salmon";
            default:
                return "khaki";
        }
    }

    render() {
        const match = this.props.match;
        return (
            <>
                <div key={match.matchID} className="match_item">
                    <table className="w-100">
                        <tbody>
                            <tr>
                                <td className="match_info text-start">
                                    <span>{(new MatchDate(Date.parse(match.time)).dateLocale().toDateTimeString())}</span>
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
                                    <span>{(new MatchDate(Date.parse(match.time)).dateLocale().toDateTimeString())}</span>
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
                                <td className="match_item_same_day_team_name">
                                    <img onError={(ev) => ev.target.src = "team_logos/empty.png"} src={("team_logos/" + match.homeTeamLogo)} width="25" height="25" />
                                    <span>
                                        {
                                            format('{0} {1} {2}', match.homeTeam, match.homeSquadName, match.homeCategoriesName)
                                        }
                                    </span>
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <td style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                        <div style={{ padding: '0 1px' }}>
                                            {
                                                ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                                format("( {0} )", match.homeTeamPenaltiesScore)

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
                                <td className="match_item_same_day_team_name">
                                    <img onError={(ev) => ev.target.src = "team_logos/empty.png"} src={("team_logos/" + match.awayTeamLogo)} width="25" height="25" />
                                    <span>
                                        {
                                            format('{0} {1} {2}', match.awayTeam, match.awaySquadName, match.awayCategoriesName)
                                        }
                                    </span>
                                </td>
                                {
                                    (match.statusID < 14 && match.statusID > 1) &&
                                    <td style={{ width: '0px' }, { whiteSpace: 'nowrap' }}>
                                        <div style={{ padding: '0 1px' }}>
                                            {
                                                ((match.homeTeamPenaltiesScore ?? 0) > 0 || (match.awayTeamPenaltiesScore ?? 0) > 0) &&
                                                format("( {0} )", match.awayTeamPenaltiesScore)
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
            </>
        );
    }
}
