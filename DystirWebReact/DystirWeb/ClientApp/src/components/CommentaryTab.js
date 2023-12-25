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
        let match = this.props.match;
        let eventsOfMatch = [...this.props.eventsOfMatch].reverse();
        let contents =
            <table className="lineups content_table">
                <tbody>
                    <tr>
                        <td style={{textAlign: "center"}, {verticalAlign: "top"}, {padding: "5px 0"}}>
                        {
                            eventsOfMatch.map((event) =>
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
                                                            <div className="score d-inline-block">{event.homeTeamScore}</div>
                                                            <div className="d-inline-block" style={{ width: "10px" }}>:</div>
                                                            <div className="score d-inline-block">{event.awayTeamScore}</div>
                                                        </>
                                                    }
                                                    </> ||
                                                    (event.eventName == EventName.GOAL
                                                        || event.eventName == EventName.OWNGOAL
                                                        || event.eventName == EventName.PENALTYSCORED
                                                        || event.eventName == EventName.PENALTYMISSED) &&
                                                    <>
                                                        <div className="score_match_view d-inline-block">{event.homeTeamPenaltiesScore}</div>
                                                        <div className="d-inline-block" style={{ width: "10px" }}>:</div>
                                                        <div className="score_match_view d-inline-block">{event.awayTeamPenaltiesScore}
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