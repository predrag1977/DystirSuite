import React, { Component } from 'react';
import { ThreeDots } from 'react-loading-icons'
import { DystirWebClientService, SelectPeriodName, PageName } from '../../services/dystirWebClientService';
import MatchDate from '../../extentions/matchDate';
import { MatchView } from "./../views/MatchView";
import { ChooseCompetitions } from './../ChooseCompetitions';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutShared } from './../layouts/LayoutShared';
import { MatchHorizontalView } from "./../views/MatchHorizontalView";
import { BsCaretLeftFill, BsCaretRightFill } from "react-icons/bs";

const dystirWebClientService = DystirWebClientService.getInstance();

export class InfoMatches extends Component {
    static displayName = InfoMatches.name;

    constructor(props) {
        super(props);
        let matchesData = dystirWebClientService.state.matchesData;
        //if (matchesData.selectedPeriod !== undefined && matchesData.selectedPeriod !== "") {
        //    window.history.replaceState(null, null, "/matches/" + matchesData.selectedPeriod);
        //} else {
        //    matchesData.selectedPeriod = window.location.pathname.split("/").pop();
        //}
        this.state = {
            matches: matchesData.matches,
            selectedCompetition: "",
            isLoading: false
        }
        //if (this.state.selectedPeriod !== undefined && this.state.selectedPeriod !== "") {
        //    window.history.replaceState(null, null, "/matches/" + this.state.selectedPeriod);
        //}
        if (this.state.matches.length === 0) {
            this.state.isLoading = true;
            dystirWebClientService.loadMatchesDataAsync();
        }

        //window.onpopstate = () => {
        //    this.onClickPeriod();
        //}
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
        this.setState({
            matches: dystirWebClientService.state.matchesData.matches,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadMatchesDataAsync(this.state.selectedPeriod);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickCompetition(competition) {
        //this.state.selectedCompetition = competition;
    }

    render() {
        const matches = this.filterMatches(this.state.matches);
        if (matches == null) return;
        const matchesGroup = matches.groupBy(match => { return match.matchTypeName });
        console.log(matchesGroup);
        const competitions = [];
        Object.keys(matchesGroup).map((group) => {
            competitions.push(group);
        });
        
        if (this.state.selectedCompetition == "" && competitions.length > 0) {
            this.state.selectedCompetition = competitions[0];
        }
        let contents =
            <>
            {
                this.state.isLoading &&
                <ThreeDots className="loading-spinner-parent" fill='dimGray' height="50" width="50" />
            }
            {
                Object.keys(matchesGroup).map(group =>
                    <div key={group}>
                        <div id="horizontal_matches">
                            <div className={"tab " + (this.state.selectedCompetition == group ? "selected_tab" : "")} onClick={() => this.onClickCompetition(group)}>
                                <div className="nav-link">{group}</div>
                            </div>
                        </div>
                        <div id="horizontal_matches" style={{ backgroundColor: "white" }} >
                        {
                            matchesGroup[group].map(match =>
                                <div key={match.matchID} className="match_item_same_day match_item_same_day_horizontal">
                                    <MatchHorizontalView match={match} page={"info"} />
                                </div>
                            )
                        }
                        </div>
                    </div>
                )
            }
            </>
        return (
            <LayoutShared>
            {
                contents
            }
            </LayoutShared>
        );
    }

    filterMatches(matches) {
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.addDays(-1);
        var toDate = now.addDays(32);

        var list = matches.filter((match) =>
            (new MatchDate(Date.parse(match.time))).dateLocale() > MatchDate.parse(fromDate) &&
            (new MatchDate(Date.parse(match.time))).dateLocale() < MatchDate.parse(toDate)
        );

        return list
            .sort((a, b) => a.matchID - b.matchID)
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => a.matchTypeID - b.matchTypeID);
    }
}
