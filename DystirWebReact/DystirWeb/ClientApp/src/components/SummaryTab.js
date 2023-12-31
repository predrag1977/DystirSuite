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
        if (match == undefined) {
            return
        }
        const eventsOfMatch = (match?.statusID ?? 0) >= 12 ? this.props.eventsOfMatch : [...this.props.eventsOfMatch].reverse();

        let contents =
        <>
            <table className="lineups content_table">
                <tbody>
                    <tr>
                        <td style={{ textAlign: "center" }, { verticalAlign: "top" }, { padding: "5px 0" }}>
                            {
                                eventsOfMatch.filter((event) => this.filterEvent(event)).map((event) =>
                                    <table key={event.eventOfMatchId} className="event_list_item">
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
                                                                    <div className="score d-inline-block">{event.homeTeamScore}
                                                                    </div>
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
                                                            <div className="score_match_view d-inline-block">{event.awayTeamPenaltiesScore}</div>
                                                        </>
                                                    }
                                                    {
                                                        (event.eventTeam == match.homeTeam) &&
                                                        <div className="summary_event text-center w-100">
                                                            {
                                                                event.eventName == EventName.GOAL &&
                                                                <>
                                                                    <div className="main_player">{this.getPlayer(event.mainPlayerOfMatchId)}</div>
                                                                    {
                                                                        this.getAssistPlayerId(event) > 0 &&
                                                                        <div className="second_player">{this.getPlayer(this.getAssistPlayerId(event))}</div>
                                                                    }
                                                                </> ||
                                                                event.eventName == EventName.SUBSTITUTION &&
                                                                <>
                                                                    <div className="main_player">{this.getPlayer(event.secondPlayerOfMatchId)}</div>
                                                                    <div className="second_player">{this.getPlayer(event.mainPlayerOfMatchId)}</div>
                                                                </> ||
                                                                event.eventName == EventName.OWNGOAL &&
                                                                <div className="owngoal_player d-inline-block">{this.getPlayer(event.mainPlayerOfMatchId)}</div> ||
                                                                event.eventName == EventName.PENALTYMISSED &&
                                                                <div className="second_player d-inline-block">{this.getPlayer(event.mainPlayerOfMatchId)}</div> ||
                                                                <div className="main_player d-inline-block">{this.getPlayer(event.mainPlayerOfMatchId)}</div>
                                                            }
                                                        </div>
                                                    }
                                                    {
                                                        (event.eventTeam == match.awayTeam) &&
                                                        <div className="summary_event text-center w-100">
                                                            {
                                                                event.eventName == EventName.GOAL &&
                                                                <>
                                                                    <div className="main_player">{this.getPlayer(event.mainPlayerOfMatchId)}</div>
                                                                    {
                                                                        this.getAssistPlayerId(event) > 0 &&
                                                                        <div className="second_player">{this.getPlayer(this.getAssistPlayerId(event))}</div>
                                                                    }
                                                                </> ||
                                                                event.eventName == EventName.SUBSTITUTION &&
                                                                <>
                                                                    <div className="main_player">{this.getPlayer(event.secondPlayerOfMatchId)}</div>
                                                                    <div className="second_player">{this.getPlayer(event.mainPlayerOfMatchId)}</div>
                                                                </> ||
                                                                event.eventName == EventName.OWNGOAL &&
                                                                <div className="owngoal_player d-inline-block">{this.getPlayer(event.mainPlayerOfMatchId)}</div> ||
                                                                event.eventName == EventName.PENALTYMISSED &&
                                                                <div className="second_player d-inline-block">{this.getPlayer(event.mainPlayerOfMatchId)}</div> ||
                                                                <div className="main_player d-inline-block">{this.getPlayer(event.mainPlayerOfMatchId)}</div>
                                                            }
                                                        </div>
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
        </>
        return contents
    }

    filterEvent(event) {
        let eventName = event.eventName.toUpperCase();
        return eventName == EventName.GOAL
            || eventName == EventName.OWNGOAL
            || eventName == EventName.PENALTYSCORED
            || eventName == EventName.PENALTYMISSED
            || eventName == EventName.YELLOW
            || eventName == EventName.RED
            || eventName == EventName.SUBSTITUTION
            || eventName == EventName.PLAYEROFTHEMATCH;
    }

    getPlayer(playerOfMatchId) {
        let player = this.props.playersOfMatch.find((player) => player.playerOfMatchId == playerOfMatchId);
        return (player?.firstName ?? "").trim() ?? "";
    }

    getAssistPlayerId(event) {
        let goalEvent = this.props.eventsOfMatch.find((e) => e.eventOfMatchId == event.eventOfMatchId);
        let indexGoalEvent = this.props.eventsOfMatch.indexOf(goalEvent);
        let assistEvent = this.props.eventsOfMatch.filter((e) => e.eventName == EventName.ASSIST && this.props.eventsOfMatch.indexOf(e) > indexGoalEvent)[0];
        return assistEvent?.mainPlayerOfMatchId ?? 0;
    }

}