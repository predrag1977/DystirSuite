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
        let contents =
        <>
            <table className="w-100" style={{ backgroundColor: "rgba(24, 24, 24, 0.85)" }}>
                <tbody>
                    <tr>
                        <td></td>
                        <td style={{ width: "50px", paddingTop: "15px"}}>
                            <img src={"team_logos/" + match.homeTeamLogo} width="50" height="50" />
                        </td>
                        <td className="football_field text-center" style={{ width: "270px", height: "90px", paddingTop: "7px", backgroundImage: "url(images/football_field.svg)" }}>
                            <div>
                                {
                                    (match.homeTeamPenaltiesScore > 0 || match.awayTeamPenaltiesScore > 0) &&
                                        <div className="d-inline-block" style={{ fontSize: "24px", marginRight: "5px" }}>{"(" + match.homeTeamPenaltiesScore + ")"}</div>
                                }
                                    <div className="d-inline-block">{match.homeTeamScore - match.homeTeamPenaltiesScore}</div>
                                    <div className="d-inline-block" style={{ width: "40px" }}>:</div>
                                    <div className="d-inline-block">{match.awayTeamScore - match.awayTeamPenaltiesScore}</div>
                                {
                                    (match.homeTeamPenaltiesScore > 0 || match.awayTeamPenaltiesScore > 0) &&
                                        <div className="d-inline-block" style={{ fontSize: "24px", marginLeft: "5px" }}>{"(" + match.awayTeamPenaltiesScore + ")"}</div>
                                }
                            </div>
                        </td>
                        <td style={{ width: "50px", paddingTop: "15px" }}>
                            <img src={"team_logos/" + match.awayTeamLogo} width="50" height="50" />
                        </td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
            <div style={{ borderBottom: "1px #404040 solid" }} />
        </>
            
        return contents
    }
}