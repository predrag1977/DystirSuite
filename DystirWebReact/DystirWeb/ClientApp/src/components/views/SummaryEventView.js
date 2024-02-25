import React, { Component } from 'react';
import { EventName } from '../../services/dystirWebClientService';
import MatchDate from '../../extentions/matchDate';
import { format } from 'react-string-format';
import { GiSoccerBall } from "react-icons/gi";
import { FaStar } from "react-icons/fa";

export class SummaryEventView extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        const event = this.props.event;
        
        let contents =
            <table className="summary_main_table">
                <tbody>
                    <tr>
                        <td className={"summary_event " + (event.eventTeam == match.homeTeam ? "visible" : "invisible")}>
                        {
                            event.eventName == EventName.GOAL &&
                            <>
                                <GiSoccerBall className="goal_icon" />
                                <span style={{ margin: "0 5px" }}>Mál</span>
                            </> ||
                            event.eventName == EventName.OWNGOAL &&
                            <>
                                <GiSoccerBall className="owngoal_icon"/>
                                <span style={{ margin: "0 5px" }}>Sjálvmál</span>
                            </> ||
                            event.eventName == EventName.PENALTYSCORED &&
                            <>
                                <GiSoccerBall className="goal_icon" />
                                <span style={{ margin: "0 5px" }}>Brotsspark skora</span>
                            </> ||
                            event.eventName == EventName.PENALTYMISSED &&
                                <span className="penalty_missed">Brotsspark brent</span> ||
                            event.eventName == EventName.BIGCHANCE &&
                                <span className="big_chance">Stórur møguleiki</span> ||
                            event.eventName == EventName.PENALTY &&
                                <span className="big_chance">Brotsspark</span> ||
                            event.eventName == EventName.YELLOW &&
                                <span className="yellow_card"></span> ||
                            event.eventName == EventName.RED &&
                                <span className="red_card"></span> ||
                            event.eventName == EventName.SUBSTITUTION &&
                            <>
                                <span className="sub_in">&#9650;</span>
                                <span className="sub_out">&#9660;</span>
                            </> ||
                            event.eventName == EventName.ASSIST &&
                                <div className="big_chance">Upplegg</div> ||
                            event.eventName == EventName.CORNER &&
                                <div className="secondary_event">Hornaspark</div> ||
                            event.eventName == EventName.ONTARGET &&
                                <div className="secondary_event">Roynd á mál</div> ||
                            event.eventName == EventName.OFFTARGET &&
                                <div className="secondary_event">Roynd framvið mál</div> ||
                            event.eventName == EventName.BLOCKEDSHOT &&
                                <div className="secondary_event">Blokera skot</div> ||
                            event.eventName == EventName.PLAYEROFTHEMATCH &&
                                <FaStar style={{ color: "gold" }} />
                        }
                        </td>

                        <td className="minutes_summary">
                        {
                            event.eventName == EventName.PLAYEROFTHEMATCH &&
                                <div className="player_of_the_match">Dagsins leikari</div> ||
                                <div>
                                {
                                    this.getEventMinute(event)
                                }
                                </div>
                        }
                        </td>

                        <td className={"summary_event " + (event.eventTeam == match.awayTeam ? "visible" : "invisible")}>
                        {
                            event.eventName == EventName.GOAL &&
                            <>
                                <span style={{ margin: "0 5px" }}>Mál</span>
                                <GiSoccerBall className="goal_icon" />
                            </> ||
                            event.eventName == EventName.OWNGOAL &&
                            <>
                                <span style={{ margin: "0 5px" }}>Sjálvmál</span>
                                <GiSoccerBall className="owngoal_icon" />
                            </> ||
                            event.eventName == EventName.PENALTYSCORED &&
                            <>
                                <span style={{ margin: "0 5px" }}>Brotsspark skora</span>
                                <GiSoccerBall className="goal_icon" />
                            </> ||
                            event.eventName == EventName.PENALTYMISSED &&
                                <span className="penalty_missed">Brotsspark brent</span> ||
                            event.eventName == EventName.BIGCHANCE &&
                                <span className="big_chance">Stórur møguleiki</span> ||
                            event.eventName == EventName.PENALTY &&
                                <span className="big_chance">Brotsspark</span> ||
                            event.eventName == EventName.YELLOW &&
                                <span className="yellow_card"></span> ||
                            event.eventName == EventName.RED &&
                                <span className="red_card"></span> ||
                            event.eventName == EventName.SUBSTITUTION &&
                            <>
                                <span className="sub_out">&#9660;</span>
                                <span className="sub_in">&#9650;</span>
                            </> ||
                            event.eventName == EventName.ASSIST &&
                                <div className="big_chance">Upplegg</div> ||
                            event.eventName == EventName.CORNER &&
                                <div className="secondary_event">Hornaspark</div> ||
                            event.eventName == EventName.ONTARGET &&
                                <div className="secondary_event">Roynd á mál</div> ||
                            event.eventName == EventName.OFFTARGET &&
                                <div className="secondary_event">Roynd framvið mál</div> ||
                            event.eventName == EventName.BLOCKEDSHOT &&
                                <div className="secondary_event">Blokera skot</div> ||
                            event.eventName == EventName.PLAYEROFTHEMATCH &&
                                <FaStar style={{ color: "gold" }} />
                        }
                        </td>
                    </tr>
                </tbody>
            </table>
        return contents
    }

    getEventMinute(event) {
        switch (event.eventPeriodId) {
            case 1:
                return "";
            case 3:
                return "46'";
            case 5:
                return "91'";
            case 7:
                return "106'";
            case 9:
                return "121'";
            case 10:
                return "brotsspark";
            case 11:
            case 12:
            case 13:
                return "";
            default:
                return event.eventMinute;
        }
    }
}
