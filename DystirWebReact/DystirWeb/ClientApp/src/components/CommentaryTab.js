import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';

export class CommentaryTab extends Component {
    static displayName = CommentaryTab.name;

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