import React, { Component } from 'react';
import { DystirWebClientService, EventName, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { SummaryEventView } from './views/SummaryEventView';

export class SummaryTab extends Component {
    static displayName = SummaryTab.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        const eventsOfMatch = this.props.eventsOfMatch;
        let homeTeamScore = 0;
        let awayTeamScore = 0;
        let contents =
            <table className="lineups content_table">
                <tbody>
                    <tr>
                        <td style={{textAlign: "center"}, {verticalAlign: "top"}, {padding: "5px 0"}}>
                        {
                            eventsOfMatch.filter((event) => this.checkSummaryEvent(event)).map((event) =>
                                <table key={event.eventOfMatchId} className="event_list_item summary_main_table">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <SummaryEventView match={match} event={event} />
                                                {
                                                    event.eventPeriodId != 10 &&
                                                    <>
                                                    {
                                                        (event.eventName == EventName.GOAL || event.eventName == EventName.OWNGOAL || event.eventName == EventName.PENALTYSCORED) &&
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
                                                    </>
                                                }
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

    checkSummaryEvent(event) {
        let eventName = event.eventName.toUpperCase();
        return eventName == EventName.GOAL
            || eventName == EventName.OWNGOAL
            || eventName == EventName.PENALTYSCORED
    }
}