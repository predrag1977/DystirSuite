import React, { Component } from 'react';
import { PuffLoader } from 'react-spinners';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseCompetitions } from './ChooseCompetitions';
import { StatisticView } from './views/StatisticView';
import { Sponsors } from './Sponsors';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Statistics extends Component {
    static displayName = Statistics.name;

    constructor(props) {
        super(props);
        let statisticsData = dystirWebClientService.state.statisticsData;

        if (statisticsData.selectedStatisticsCompetitionId == "") {
            statisticsData.selectedStatisticsCompetitionId = window.location.pathname.split("/").pop();
        }
        if (isNaN(statisticsData.selectedStatisticsCompetitionId) || statisticsData.selectedStatisticsCompetitionId == "") {
            statisticsData.selectedStatisticsCompetitionId = 0
        }
        window.history.replaceState(null, null, "/statistics/" + statisticsData.selectedStatisticsCompetitionId);
        
        this.state = {
            statistics: statisticsData.statistics,
            selectedStatisticsCompetitionId: statisticsData.selectedStatisticsCompetitionId,
            isLoading: true
        }

        dystirWebClientService.loadStatisticsDataAsync(this.state.selectedStatisticsCompetitionId);

        window.onpopstate = () => {
            this.onClickCompetition();
        }
    }

    componentDidMount() {
        document.body.addEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onUpdateStandings', this.onReloadData.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateStandings', this.onReloadData.bind(this));
    }

    onReloadData() {
        this.setState({
            statistics: dystirWebClientService.state.statisticsData.statistics,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadStatisticsDataAsync(this.state.selectedStatisticsCompetitionId);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickCompetition() {
        let competitionId = window.location.pathname.split("/").pop();
        dystirWebClientService.state.statisticsData.selectedStatisticsCompetitionId = competitionId;
        this.setState({
            selectedStatisticsCompetitionId: competitionId
        });
    }

    render() {
        const statisticsGroup = this.state.statistics.groupBy(statistic => { return statistic.competitionName });
        const competitions = [];
        Object.keys(statisticsGroup).map((group) => {
            competitions.push(group);
        });
        const selectedStatisticsCompetitionId = this.state.selectedStatisticsCompetitionId !== ""
            ? this.state.selectedStatisticsCompetitionId : (competitions.length > 0 ? 0 : "");
        let contents =
            <>
                <ChooseCompetitions onClickCompetition={() => this.onClickCompetition()}
                    competitions={competitions}
                    page="statistics"
                    selectedCompetition={competitions[selectedStatisticsCompetitionId]} />
                <div className="main_container">
                    {
                        this.state.isLoading &&
                        <PuffLoader className="loading-spinner-parent" color="lightGray" height="50" width="50" />
                    }
                    {
                        this.renderStatistics(this.state.statistics)
                    }
                    <Sponsors />
                </div>
            </>
        return (
            <LayoutDystir page={PageName.STATISTICS}>
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderStatistics(statistics) {
        let statistic = statistics.filter(
            (statistic) => statistic.statisticCompetitionId == this.state.selectedStatisticsCompetitionId
        )[0] ?? (statistics.length > 0 ? statistics[0] : []);
        if (statistic.length === 0) return;
        return (
            <StatisticView key={statistic.statisticCompetitionName} statistic={statistic} />
        );
    }
}