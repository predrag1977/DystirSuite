import React, { Component } from 'react';
import { DystirWebClientService, PageName, TabName } from '../services/dystirWebClientService';
import { LayoutMatchDetails } from './layouts/LayoutMatchDetails';
import { Lineups } from './Lineups';
import { SummaryTab } from './SummaryTab';
import { CommentaryTab } from './CommentaryTab';
import { StandingsTab } from './StandingsTab';
import { StatisticsTab } from './StatisticsTab';
import { MatchDetailsTabs } from './MatchDetailsTabs';
import { MatchDetailsInfo } from './MatchDetailsInfo';

const dystirWebClientService = DystirWebClientService.getInstance();

export class MatchDetails extends Component {
    static displayName = MatchDetails.name;

    constructor(props) {
        super(props);
        let matchDetailsData = dystirWebClientService.state.matchDetailsData;
        const lenght = window.location.pathname.split("/").length;
        matchDetailsData.matchId = window.location.pathname.split("/")[2];
        if (lenght > 3) {
            matchDetailsData.selectedTab = window.location.pathname.split("/")[3];
        }
        if (matchDetailsData.selectedTab.length == 0) {
            matchDetailsData.selectedTab = TabName.SUMMARY;
        }

        this.state = {
            matches: matchDetailsData.matches,
            match: matchDetailsData.match,
            matchId: matchDetailsData.matchId,
            eventsOfMatch: matchDetailsData.eventsOfMatch,
            playersOfMatch: matchDetailsData.playersOfMatch,
            standings: matchDetailsData.standings,
            statistic: matchDetailsData.statistic,
            selectedTab: matchDetailsData.selectedTab,
            isLoading: true
        }
        dystirWebClientService.loadMatchDetailsDataAsync(this.state.matchId, this.state.selectedTab);
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
            selectedTab: matchDetailsData.selectedTab,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadMatchDetailsDataAsync(this.state.matchId, this.state.selectedTab);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickTab() {
        let selectedTabParameter = window.location.pathname.split("/").pop();
        dystirWebClientService.state.matchesData.selectedTab = selectedTabParameter;
        this.setState({
            selectedTab: selectedTabParameter
        });
        var mainContainer = document.getElementsByClassName('main_container')[0];
        mainContainer.scrollTo(0, 0);
    }

    render() {
        let contents =
            <>
                <MatchDetailsInfo match={this.state.match} />
                <MatchDetailsTabs onClickTab={() => this.onClickTab()} match={this.state.match} selectedTab={this.state.selectedTab} />
                <div className="main_container">
                    {
                        this.state.isLoading &&
                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        <>
                            <div className={this.state.selectedTab === TabName.SUMMARY ? "active_tab" : "no_active_tab"}>
                                <SummaryTab match={this.state.match} eventsOfMatch={this.state.eventsOfMatch} playersOfMatch={this.state.playersOfMatch} />
                            </div>
                            <div className={this.state.selectedTab === TabName.LINEUPS ? "active_tab" : "no_active_tab"}>
                                <Lineups match={this.state.match} playersOfMatch={this.state.playersOfMatch} />
                            </div>
                            <div className={this.state.selectedTab === TabName.COMMENTARY ? "active_tab" : "no_active_tab"}>
                                <CommentaryTab match={this.state.match} eventsOfMatch={this.state.eventsOfMatch} />
                            </div>
                            <div className={this.state.selectedTab === TabName.STATISTICS ? "active_tab" : "no_active_tab"}>
                                <StatisticsTab match={this.state.match} statistic={this.state.statistic} />
                            </div>
                            <div className={this.state.selectedTab === TabName.STANDINGS ? "active_tab" : "no_active_tab"}>
                                <StandingsTab match={this.state.match} standings={this.state.standings} />
                            </div>
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
