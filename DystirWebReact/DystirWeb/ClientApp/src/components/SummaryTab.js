import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { PlayerView } from './views/PlayerView';

export class SummaryTab extends Component {
    static displayName = SummaryTab.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        const eventsOfMatch = this.props.eventsOfMatch;
        let contents =
            <table className="lineups content_table">
                <tbody>
                    <tr>
                        <td style={{textAlign: "center"}, {verticalAlign: "top"}, {padding: "5px 0"}}>
                        {
                            eventsOfMatch.map((event) =>
                                <div key={event.eventOfMatchId}>{event.eventText}</div>
                            )
                        }
                        </td>
                    </tr>
                </tbody>
            </table>

        return contents
    }
}