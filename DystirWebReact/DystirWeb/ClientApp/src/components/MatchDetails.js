import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import { LayoutMatchDetails } from './layouts/LayoutMatchDetails';
import { Lineups } from './Lineups';
import { SummaryTab } from './SummaryTab';
import { CommentaryTab } from './CommentaryTab';
import { StandingsTab } from './StandingsTab';
import { StatisticsTab } from './StatisticsTab';
import { MatchDetailsTabs } from './MatchDetailsTabs';

const dystirWebClientService = DystirWebClientService.getInstance();

export class MatchDetails extends Component {
    static displayName = MatchDetails.name;

    constructor(props) {
        super(props);
        let matchDetailsData = dystirWebClientService.state.matchDetailsData;
        matchDetailsData.matchId = window.location.pathname.split("/").pop();

        this.state = {
            matches: matchDetailsData.matches,
            match: matchDetailsData.match,
            matchId: matchDetailsData.matchId,
            eventsOfMatch: matchDetailsData.eventsOfMatch,
            playersOfMatch: matchDetailsData.playersOfMatch,
            standings: matchDetailsData.standings,
            statistic: matchDetailsData.statistic,
            isLoading: true
        }
        dystirWebClientService.loadMatchDetailsDataAsync(this.state.matchId);
    }

    componentDidMount() {
        document.body.addEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onUpdateMatch', this.onReloadData.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateMatch', this.onReloadData.bind(this));
    }

    onReloadData() {
        let matchDetailsData = dystirWebClientService.state.matchDetailsData;
        this.setState({
            matches: matchDetailsData.matches,
            match: matchDetailsData.match,
            matchId: matchDetailsData.matchId,
            eventsOfMatch: matchDetailsData.eventsOfMatch,
            playersOfMatch: matchDetailsData.playersOfMatch,
            standings: matchDetailsData.standings,
            statistic: matchDetailsData.statistic,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadMatchDetailsDataAsync(this.state.matchId);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickTab() {
        //let periodParameter = window.location.pathname.split("/").pop();
        //dystirWebClientService.state.matchesData.selectedPeriod = periodParameter;
        //this.setState({
        //    selectedPeriod: periodParameter
        //});
    }

    render() {
        let contents =
            <>
                <MatchDetailsTabs onClickTab={() => this.onClickTab()} match={this.state.match} selectedTab={this.state.selectedTab} />
                <div className="main_container">
                    {
                        this.state.isLoading &&

                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        <>
                            <SummaryTab match={this.state.match} eventsOfMatch={this.state.eventsOfMatch} />
                            <Lineups match={this.state.match} playersOfMatch={this.state.playersOfMatch} />
                            <CommentaryTab match={this.state.match} eventsOfMatch={this.state.eventsOfMatch} />
                            <StatisticsTab match={this.state.match} statistic={this.state.statistic} />
                            <StandingsTab match={this.state.match} standings={this.state.standings} />
                        </>
                    }
                </div>
            </>
        return (
            <LayoutMatchDetails match={this.state.match}>
            {
                contents
            }
            </LayoutMatchDetails>
        );
    }
}
