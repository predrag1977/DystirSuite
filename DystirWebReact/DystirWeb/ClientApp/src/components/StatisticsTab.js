import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { PlayerView } from './views/PlayerView';
import { Statistics } from './Statistics';

export class StatisticsTab extends Component {
    static displayName = StatisticsTab.name;

    constructor(props) {
        super(props);
    }

    render() {
        const match = this.props.match;
        const statistic = this.props.statistic;
        if (statistic === null) return;
        let contents =
        <>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.goal}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            MÁL
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.goal}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.goalProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" style={{ width: statistic.awayTeamStatistic.goalProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.onTarget}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            ROYNDIR Á MÀL
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.onTarget}
                        </td>
                        </tr>
                    </tbody>
                </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.onTargetProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.onTargetProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.offTarget}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            ROYNDIR FRAMVIÐ MÀL
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.offTarget}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.offTargetProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.offTargetProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.blockedShot}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            BLOKERA SKOT
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.blockedShot}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.blockedShotProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.blockedShotProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.corner}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            HORNA
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.corner}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.cornerProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.cornerProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.bigChance}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            STÓRUR MØGULEIKI
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.bigChance}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.bigChanceProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.bigChanceProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.yellowCard}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            GUL KORT
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.yellowCard}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.yellowCardProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.yellowCardProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td className="statistic_cell_number">
                            {statistic.homeTeamStatistic.redCard}
                        </td>
                        <td colSpan="2" className="statistic_cell">
                            REYÐ KORT
                        </td>
                        <td className="statistic_cell_number">
                            {statistic.awayTeamStatistic.redCard}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table className="content_table statistic_table">
                <tbody>
                    <tr>
                        <td colSpan="2" className="statistic-home" style={{ width: statistic.homeTeamStatistic.redCardProcent + "%" }}></td>
                        <td colSpan="2" className="statistic-away" sstyle={{ width: statistic.awayTeamStatistic.redCardProcent + "%" }}></td>
                    </tr>
                </tbody>
            </table>
        </>
            
        return contents
    }
}