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
        if (match == undefined) {
            return
        }
        let contents =
        <>
            <div style={{ backgroundColor: "rgba(24, 24, 24, 0.85)", height: "150px" }}>
                <table className="w-100" >
                    <tbody>
                        <tr>
                            <td className="match_info text-center" style={{ paddingTop: "4px" }}>
                            {
                                (new MatchDate(Date.parse(match.time)).dateLocale().toDateTimeString()) + 
                                ((match.matchTypeName?.trim() !== undefined && match.matchTypeName?.trim() !== "") && " - ") +
                                match.matchTypeName
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
                                <img src={"team_logos/" + match.homeTeamLogo} width="50" height="50" />
                            </td>
                            <td className="football_field text-center" style={{ width: "270px", height: "90px", paddingTop: "8px", backgroundImage: "url(images/football_field.svg)" }}>
                                <div>
                                    {
                                        (match.homeTeamPenaltiesScore > 0 || match.awayTeamPenaltiesScore > 0) &&
                                        <div className="d-inline-block" style={{ fontSize: "22px", marginRight: "5px", color: "beige" }}>{"( " + match.homeTeamPenaltiesScore + " )"}</div>
                                    }
                                    <div className="d-inline-block">{(match.homeTeamScore ?? 0) - (match.homeTeamPenaltiesScore ?? 0)}</div>
                                    <div className="d-inline-block" style={{ width: "40px" }}>:</div>
                                    <div className="d-inline-block">{(match.awayTeamScore ?? 0) - (match.awayTeamPenaltiesScore ?? 0)}</div>
                                    {
                                        (match.homeTeamPenaltiesScore > 0 || match.awayTeamPenaltiesScore > 0) &&
                                        <div className="d-inline-block" style={{ fontSize: "22px", marginLeft: "5px", color: "beige" }}>{"( " + match.awayTeamPenaltiesScore + " )"}</div>
                                    }
                                </div>
                            </td>
                            <td style={{ width: "50px" }}>
                                <img src={"team_logos/" + match.awayTeamLogo} width="50" height="50" />
                            </td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
                <table className="w-100">
                    <tbody>
                        <tr>
                            <td className="match_info text-center" style={{ paddingBottom: "4px" }}>
                                <span>{match.roundName}</span>
                                {
                                    (match.location?.trim() !== undefined && match.location?.trim() !== "") && <span> - </span>
                                }
                                <span>{match.location}</span>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div style={{ borderBottom: "1px #404040 solid" }} />
            </div>
        </>
            
        return contents
    }
}