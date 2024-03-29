import React, { Component } from 'react';
import { PuffLoader } from 'react-spinners';
import { DystirWebClientService, PageName, TabName } from '../services/dystirWebClientService';
import { scrollButtonVisibility, scrollOnClick } from '../extentions/scrolling';
import { LayoutMatchDetails } from './layouts/LayoutMatchDetails';
import { LayoutMatchDetailsShared } from './layouts/LayoutMatchDetailsShared';
import { Lineups } from './Lineups';
import { SummaryTab } from './SummaryTab';
import { CommentaryTab } from './CommentaryTab';
import { StandingsTab } from './StandingsTab';
import { StatisticsTab } from './StatisticsTab';
import { MatchDetailsTabs } from './MatchDetailsTabs';
import { MatchDetailsInfo } from './MatchDetailsInfo';
import { LiveMatchView } from "./views/LiveMatchView";
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import MatchDate from '../extentions/matchDate';
import { BsCaretLeftFill, BsCaretRightFill } from "react-icons/bs";

const dystirWebClientService = DystirWebClientService.getInstance();

export class MatchDetails extends Component {
    static displayName = MatchDetails.name;

    constructor(props) {
        super(props);
        this.url = window.location.href.toLowerCase();
        this.parameterIndex = 3;
        if (this.url.includes("info") ||
            this.url.includes("portal") ||
            this.url.includes("roysni")
        ) {
            this.parameterIndex = 4
        }

        let matchDetailsData = dystirWebClientService.state.matchDetailsData;
        const lenght = window.location.pathname.split("/").length;
        matchDetailsData.matchId = window.location.pathname.split("/")[this.parameterIndex - 1];
        if (lenght > this.parameterIndex) {
            matchDetailsData.selectedTab = window.location.pathname.split("/")[this.parameterIndex];
        }
        if (matchDetailsData.selectedTab.length == 0) {
            matchDetailsData.selectedTab = TabName.SUMMARY;
        }

        let match = dystirWebClientService.state.matchesData.matches.find((m) => m.matchID == matchDetailsData.matchId);
        this.state = {
            matches: matchDetailsData.matches,
            match: match,
            matchId: matchDetailsData.matchId,
            selectedTab: matchDetailsData.selectedTab,
            isLoading: true,
            collapsed: true
        }
        dystirWebClientService.loadMatchDetailsDataAsync(this.state.matchId, this.state.selectedTab);

        window.onpopstate = () => {
            this.onClickTab();
        }
    }

    componentDidMount() {
        document.body.addEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onUpdateMatchDetails', this.onReloadData.bind(this));
        window.addEventListener('resize', this.onResize.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateMatchDetails', this.onReloadData.bind(this));
        window.removeEventListener('resize', this.onResize.bind(this));
    }

    componentDidUpdate() {
        scrollButtonVisibility("match_details");
    }

    onReloadData() {
        let matchDetailsData = dystirWebClientService.state.matchDetailsData;
        this.setState({
            matches: matchDetailsData.matches,
            match: matchDetailsData.match,
            matchId: matchDetailsData.matchId,
            selectedTab: matchDetailsData.selectedTab,
            isLoading: false,
            collapsed: true
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

    onResize() {
        scrollButtonVisibility("match_details");
    }

    onClickTab() {
        let selectedTabParameter = "";
        const lenght = window.location.pathname.split("/").length;
        if (lenght > this.parameterIndex) {
            selectedTabParameter = window.location.pathname.split("/")[this.parameterIndex];
        }
        if (selectedTabParameter.length == 0) {
            selectedTabParameter = TabName.SUMMARY
        }
        dystirWebClientService.state.matchDetailsData.selectedTab = selectedTabParameter;
        this.setState({
            selectedTab: selectedTabParameter
        });
        this.scrollToTop();

        this.setState({
            collapsed: true
        });
    }

    onMoreLiveMatchClick(e) {
        this.scrollToTop();
        this.setState({
            collapsed: !this.state.collapsed
        });
        e.stopPropagation();
    }

    onCloseMoreLiveMatches() {
        this.setState({
            collapsed: true
        });
    }

    onLiveMatchClick(matchID) {
        this.onCloseMoreLiveMatches();
        this.setState({
            isLoading: true
        });
        this.scrollToTop();
        dystirWebClientService.loadMatchDetailsDataAsync(matchID, this.state.selectedTab);
    }

    scrollToTop() {
        var mainContainer = document.getElementsByClassName('main_container_match_details')[0];
        if (mainContainer !== undefined) {
            mainContainer.scrollTo(0, 0);
        }
    }

    render() {
        if (this.state.match == undefined) {
            return;
        }
        let page = ""
        if (this.url.includes("info")) {
            page = "info";
        } else if (this.url.includes("portal")) {
            page = "portal";
        } else if (this.url.includes("roysni")) {
            page = "roysni";
        }
        var eventsOfMatch = this.state.match?.matchDetails?.eventsOfMatch ?? [];
        var playersOfMatch = this.state.match?.matchDetails?.playersOfMatch ?? [];
        var statistic = this.state.match?.matchDetails?.statistic ?? null;
        var standings = this.state.match?.matchDetails?.standings ?? [];
        var liveMatches = this.filterMatches(this.state.matches ?? []);

        let noLiveMatches = liveMatches.length == 0;
        if (page == "") {
            liveMatches = liveMatches.filter((match) => 
                match.statusID < 14
            );
        }
        noLiveMatches = liveMatches.length == 0 || (liveMatches.length == 1 && liveMatches[0].matchID == this.state.match.matchID);
        if (noLiveMatches) {
            liveMatches = [];
        }
        
        this.liveMatchesHeight(noLiveMatches);

        let contents =
            <>
                <div id="live_matches_container">
                    <div id="scroll_button_left" onClick={() => scrollOnClick('left', "match_details")}>
                        <BsCaretLeftFill />
                    </div>
                    <div id="match_details_horizontal_menu">
                        <div id="match_details_horizontal_menu_wrapper">
                        {
                            liveMatches.map(match =>
                                <div key={match.matchID} className="match_item_same_day" onClick={() => this.onLiveMatchClick(match.matchID)}>
                                    <LiveMatchView match={match} page={page} />
                                </div>
                            )
                        }
                        </div>
                    </div>
                    <div id="scroll_button_right" onClick={() => scrollOnClick("right", "match_details")}>
                        <BsCaretRightFill />
                    </div>
                </div>
                <div className="main_container_match_details">
                    {
                        this.state.isLoading &&
                        <PuffLoader className="loading-spinner-parent" color="lightGray" height="50" width="50" />
                    }
                    <MatchDetailsInfo match={this.state.match} page={page} />
                    <MatchDetailsTabs onClickTab={() => this.onClickTab()}
                        onMoreLiveMatchClick={(e) => this.onMoreLiveMatchClick(e)}
                        match={this.state.match}
                        selectedTab={this.state.selectedTab}
                        standings={standings}
                        page={page} />
                    {
                        <>
                            <div className={this.state.selectedTab === TabName.SUMMARY ? "active_tab" : "no_active_tab"}>
                                <SummaryTab match={this.state.match} eventsOfMatch={eventsOfMatch} playersOfMatch={playersOfMatch} />
                            </div>
                            <div className={this.state.selectedTab === TabName.LINEUPS ? "active_tab" : "no_active_tab"}>
                                <Lineups match={this.state.match} playersOfMatch={playersOfMatch} />
                            </div>
                            <div className={this.state.selectedTab === TabName.COMMENTARY ? "active_tab" : "no_active_tab"}>
                                <CommentaryTab match={this.state.match} eventsOfMatch={eventsOfMatch} />
                            </div>
                            <div className={this.state.selectedTab === TabName.STATISTICS ? "active_tab" : "no_active_tab"}>
                                <StatisticsTab match={this.state.match} statistic={statistic} />
                            </div>
                            <div className={this.state.selectedTab === TabName.STANDINGS ? "active_tab" : "no_active_tab"}>
                                <StandingsTab match={this.state.match} standings={standings} />
                            </div>
                        </>
                    }
                </div>
            </>
        return (
            (this.url.includes("info") || this.url.includes("portal") || this.url.includes("roysni")) &&
            <LayoutMatchDetailsShared match={this.state.match}>
            {
                contents
            }
            </LayoutMatchDetailsShared> ||
            <LayoutMatchDetails match={this.state.match}>
            {
                contents
            }
            </LayoutMatchDetails>
        );
    }

    liveMatchesHeight(noLiveMatches) {
        var liveMatchesContainerID = document.getElementById('live_matches_container');
        if (liveMatchesContainerID !== null && liveMatchesContainerID !== undefined) {
            liveMatchesContainerID.style.visibility = noLiveMatches ? "hidden" : "visible";
            liveMatchesContainerID.style.height = noLiveMatches ? "0px" : "60px";
        }
        var fromTop = noLiveMatches ? "80px" : "140px";
        var matchDetailsID = document.getElementsByClassName('main_container_match_details')[0];
        if (matchDetailsID !== undefined) {
            matchDetailsID.style.top = fromTop;
            matchDetailsID.style.height = "calc(100% - " + fromTop + ")";
        }
    }

    filterMatches(matches) {
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.addDays(0);
        var toDate = now.addDays(1);

        var list = matches.filter((match) =>
            MatchDate.parse(match.time) >= MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
        );
        return list
            .sort((a, b) => a.matchID - b.matchID)
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => a.matchTypeID - b.matchTypeID);
    }
}
