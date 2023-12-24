import React, { Component } from 'react';
import { DystirWebClientService, EventName, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { SummaryEventView } from './views/SummaryEventView';

export class CommentaryTab extends Component {
    static displayName = CommentaryTab.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        const eventsOfMatch = this.props.eventsOfMatch;
        let homeTeamScore = 0;
        let awayTeamScore = 0;
        let homeTeamPenaltiesScore = 0;
        let awayTeamPenaltiesScore = 0;
        let contents =
            <table className="lineups content_table">
                <tbody>
                    <tr>
                        <td style={{textAlign: "center"}, {verticalAlign: "top"}, {padding: "5px 0"}}>
                        {
                            eventsOfMatch.reverse().map((event) =>
                                <table key={event.eventOfMatchId} className="event_list_item summary_main_table">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <SummaryEventView match={match} event={event} />
                                                {
                                                    event.eventPeriodId != 10 &&
                                                    <>
                                                    {
                                                        (event.eventName == EventName.GOAL
                                                            || event.eventName == EventName.OWNGOAL
                                                            || event.eventName == EventName.PENALTYSCORED) &&
                                                        <>
                                                            <div className="score d-inline-block">
                                                            {
                                                                homeTeamScore = event.eventTeam == match.homeTeam ? homeTeamScore + 1 : homeTeamScore
                                                            }
                                                            </div>
                                                            <div className="d-inline-block">:</div>
                                                            <div className="score d-inline-block">
                                                            {
                                                                awayTeamScore = event.eventTeam == match.awayTeam ? awayTeamScore + 1 : awayTeamScore
                                                            }
                                                            </div>
                                                        </>
                                                    }
                                                    </> ||
                                                    (event.eventName == EventName.GOAL
                                                        || event.eventName == EventName.OWNGOAL
                                                        || event.eventName == EventName.PENALTYSCORED
                                                        || event.eventName == EventName.PENALTYMISSED) &&
                                                    <>
                                                        <div className="score_match_view d-inline-block">
                                                        {
                                                            homeTeamPenaltiesScore = event.eventTeam == match.homeTeam ? homeTeamPenaltiesScore + 1 : homeTeamPenaltiesScore
                                                        }
                                                        </div>
                                                        <div className="d-inline-block">:</div>
                                                        <div className="score_match_view d-inline-block">
                                                        {
                                                            awayTeamPenaltiesScore = event.eventTeam == match.awayTeam ? awayTeamPenaltiesScore + 1 : awayTeamPenaltiesScore
                                                        }
                                                        </div>
                                                    </>
                                                }
                                                <div className="summary_event text-center w-100">{event.eventText?.replace("..",".")}</div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            )
                        }
                        </td>
                    </tr>
                </tbody>
            </table>
        return contents
    }
}