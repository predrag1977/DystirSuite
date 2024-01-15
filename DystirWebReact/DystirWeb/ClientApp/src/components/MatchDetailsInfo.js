import React, { Component } from 'react';
import { DystirWebClientService, EventName, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import MatchTimeAndColor from '../extentions/matchTimeAndColor';

export class MatchDetailsInfo extends Component {
    static displayName = MatchDetailsInfo.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        let matchTime = match?.time != null ? new MatchDate(Date.parse(match.time)).dateLocale().toDateTimeString() : "";
        let matchTypeName = (match?.matchTypeName ?? "").trim();
        let hasMatchTypeName = (matchTypeName !== undefined && matchTypeName !== "") ? " - " : "";
        let homeTeamLogo = match?.homeTeamLogo != null ? "team_logos/" + match.homeTeamLogo : "team_logos/empty.png";
        let awayTeamLogo = match?.homeTeamLogo != null ? "team_logos/" + match.awayTeamLogo : "team_logos/empty.png";
        let contents =
        <>
            <div className="field_background">
                <table className="w-100" >
                    <tbody>
                        <tr>
                            <td className="match_info text-center" style={{ paddingTop: "4px" }}>
                            {
                                matchTime + hasMatchTypeName + matchTypeName
                            }
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table className="w-100">
                    <tbody>
                        <tr>
                            <td></td>
                            <td style={{ width: "50px"}}>
                                <img src={homeTeamLogo} width="50" height="50" />
                            </td>
                            <td className="football_field text-center" style={{ width: "270px", height: "90px", paddingTop: "8px", backgroundImage: "url(images/football_field.svg)" }}>
                                <div>
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
                                </div>
                            </td>
                            <td style={{ width: "50px" }}>
                                <img src={awayTeamLogo} width="50" height="50" />
                            </td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
                <table className="w-100">
                    <tbody>
                        <tr>
                            <td className="match_info text-center" style={{ paddingBottom: "4px" }}>
                                <span>{match?.roundName}</span>
                                {
                                    (match?.location?.trim() !== undefined && match?.location?.trim() !== "") && <span> - </span>
                                }
                                <span>{match?.location}</span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </>
            
        return contents
    }
}