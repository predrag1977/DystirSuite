import React, { Component } from 'react';
import { DystirWebClientService, EventName, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import MatchTimeAndColor from '../extentions/matchTimeAndColor';

export class MatchDetailsInfo extends Component {
    static displayName = MatchDetailsInfo.name;

    constructor(props) {
        super(props);
        this.matchTimeAndColor = new MatchTimeAndColor();
        this.state = {
            matchTime: this.matchTimeAndColor.getMatchTime(this.props.match),
            statusColor: this.matchTimeAndColor.getStatusColor(this.props.match?.statusID)
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
        this.setState({
            matchTime: matchTime,
            statusColor: this.matchTimeAndColor.getStatusColor(this.props.match?.statusID)
        });
    }

    render() {
        const match = this.props.match;
        let matchDateTime = match?.time != null ? new MatchDate(Date.parse(match.time)).toDateTimeString() : "";

        let matchRoundName = (match?.roundName ?? "").trim();
        let hasMatchRoundName = matchRoundName !== undefined && matchRoundName !== "";

        let matchTypeName = (match?.matchTypeName ?? "").trim();
        let hasMatchTypeName = (hasMatchRoundName && (matchTypeName !== undefined && matchTypeName !== "")) ? " - " : "";

        let homeTeamLogo = match?.homeTeamLogo != null ? "team_logos/" + match.homeTeamLogo : "team_logos/empty.png";
        let awayTeamLogo = match?.homeTeamLogo != null ? "team_logos/" + match.awayTeamLogo : "team_logos/empty.png";

        var matchTime = this.matchTimeAndColor.getMatchTime(match);
        this.state.matchTime = matchTime;
        this.state.statusColor = this.matchTimeAndColor.getStatusColor(match?.statusID);

        let matchLocation = (match?.location ?? "").trim();
        let hasMatchLocation = (matchLocation !== undefined && matchLocation !== "") ? " - " : "";

        let contents =
        <>
            <div className="field_background">
                <table className="w-100 text-center">
                    <tbody>
                        <tr style={{ fontSize: "1.0rem" }}>
                            <td></td>
                            <td className="match_time" style={{ whiteSpace: 'nowrap' }}>
                                {
                                    match?.statusID < 14 &&
                                    <div style={{ color: this.state.statusColor }}>{this.state.matchTime}</div>
                                }
                            </td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
                <table className="w-100">
                    <tbody>
                        <tr>
                            <td></td>
                            <td style={{ width: "50px", paddingTop: "8px"}}>
                                <img src={homeTeamLogo} width="50" height="50" />
                            </td>
                            <td className="football_field text-center" style={{ width: "270px", height: "90px", paddingTop: "8px", backgroundImage: "url(images/football_field.svg)" }}>
                                {
                                (match?.statusID > 1 && match?.statusID < 14) &&
                                <>
                                    {
                                        (match?.homeTeamPenaltiesScore > 0 || match?.awayTeamPenaltiesScore > 0) &&
                                        <div className="d-inline-block" style={{ fontSize: "22px", marginRight: "5px", color: "beige" }}>{"( " + match?.homeTeamPenaltiesScore + " )"}</div>
                                    }
                                    <div className="match_details_field_text">{(match?.homeTeamScore ?? 0) - (match?.homeTeamPenaltiesScore ?? 0)}</div>
                                    <div className="match_details_field_text" style={{ width: "40px" }}>:</div>
                                    <div className="match_details_field_text">{(match?.awayTeamScore ?? 0) - (match?.awayTeamPenaltiesScore ?? 0)}</div>
                                    {
                                        (match?.homeTeamPenaltiesScore > 0 || match?.awayTeamPenaltiesScore > 0) &&
                                        <div className="d-inline-block" style={{ fontSize: "22px", marginLeft: "5px", color: "beige" }}>{"( " + match?.awayTeamPenaltiesScore + " )"}</div>
                                    }
                                </> ||
                                <>
                                    <div className="match_details_field_text">-</div>
                                    <div className="match_details_field_text" style={{ width: "40px" }}>:</div>
                                    <div className="match_details_field_text">-</div>
                                </>
                                }
                            </td>
                            <td style={{ width: "50px", paddingTop: "8px" }}>
                                <img src={awayTeamLogo} width="50" height="50" />
                            </td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
                <table className="w-100">
                    <tbody>
                        <tr style={{ fontSize: "16px" }}>
                            <td className="match_info text-center" style={{ paddingTop: "2px" }}>
                            {
                                matchDateTime
                            }
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table className="w-100" >
                    <tbody>
                        <tr style={{ fontSize: "16px" }}>
                            <td className="match_info text-center" style={{ paddingTop: "2px", whiteSpace: "normal" }}>
                            {
                                matchTypeName + hasMatchTypeName + matchRoundName + hasMatchLocation + matchLocation
                            }
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div className="line" />
        </>
            
        return contents
    }
}