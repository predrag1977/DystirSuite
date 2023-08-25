import React, { Component } from 'react';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';

export class MatchView extends Component {

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        return (
            <div key={match.matchID} className="match_item">
                <table className="w-100">
                    <tbody>
                        <tr>
                            <td className="match_info text-left">
                                <span>{(new MatchDate(Date.parse(match.time)).toDateTimeString())}</span>
                                {match.matchTypeName.trim() != "" && <span> - </span>}
                                <span>{match.matchTypeName}</span>
                                {match.roundName.trim() != "" && <span> - </span>}
                                <span>{match.roundName}</span>
                                {match.location.trim() != "" && <span> - </span>}
                                <span>{match.location}</span>
                            </td>

                            <td className="match_time"></td>
                        </tr>
                    </tbody>
                </table>

                <table className="w-100 mb-1 mt-1">
                    <tbody>
                        <tr>
                            <td className="match_item_team_name pr-1 text-right">
                                <span>{
                                    format('{0} {1} {2}', match.homeTeam, match.homeSquadName, match.homeCategoriesName).trim()
                                }</span>
                            </td>
                            <td className="text-center" style={{ width: '30px' }}>
                                <img onError={this.addDefaultSrc} src={"team_logos/" + match.homeTeamLogo} width="25" height="25" />
                            </td>
                            <td className="text-center" style={{ width: '20px' }}>-</td>
                            <td className="text-center" style={{ width: '30px' }}>
                                <img onError={this.addDefaultSrc} src={("team_logos/" + match.awayTeamLogo)} width="25" height="25" />
                            </td>
                            <td className="match_item_team_name pl-1 text-left">
                                <span>{
                                    format('{0} {1} {2}', match.awayTeam, match.awaySquadName, match.awayCategoriesName).trim()
                                }</span>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <table className="content_table">
                    <tbody>
                        <tr className="match_item_text_color">
                            <td className="text-left">
                                {
                                    match.statusID < 14 && <div>00:00</div>
                                }
                            </td>

                            {() =>
                            {
                                console.log("test");
                                    if (match.statusID < 14 && match.statusID > 1) {
                                        return (<td>test</td>)
                                    }
                                    else {
                                        return (
                                            <>
                                                <td className="score_match_view">-</td>
                                                <td className="text-center" style={{ width: "20px" }}>:</td>
                                                <td className="score_match_view">-</td>
                                            </>

                                        )
                                    }
                                }
                            } 
                        </tr>
                    </tbody>
                </table>
            </div>
            //<tr key={match.matchID}>
            //    <td>{match.homeTeam}</td>
            //    <td>-</td>
            //    <td>{match.awayTeam}</td>
            //    <td>{match.homeTeamScore}</td>
            //    <td>:</td>
            //    <td>{match.awayTeamScore}</td>
            //    <td>{match.matchTypeName}</td>
            //    <td>{match.location}</td>
            //    <td>{match.time}</td>
            //</tr>
        );
    }

    addDefaultSrc(ev) {
        ev.target.src = "team_logos/empty.png"
    }
}
