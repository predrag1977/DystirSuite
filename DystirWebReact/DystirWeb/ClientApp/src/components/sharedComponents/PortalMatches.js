import React, { Component } from 'react';
import { PuffLoader } from 'react-spinners';
import { DystirWebClientService, SelectPeriodName, PageName } from '../../services/dystirWebClientService';
import MatchDate from '../../extentions/matchDate';
import { MatchView } from "./../views/MatchView";
import { ChooseDays } from './../ChooseDays';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutShared } from './../layouts/LayoutShared';

const dystirWebClientService = DystirWebClientService.getInstance();

export class PortalMatches extends Component {
    static displayName = PortalMatches.name;

    constructor(props) {
        super(props);
        let matchesData = dystirWebClientService.state.matchesData;
        this.state = {
            matches: matchesData.matches,
            selectedPeriod: matchesData.selectedPeriod,
            isLoading: false
        }
        if (this.state.matches.length === 0) {
            this.state.isLoading = true;
            dystirWebClientService.loadMatchesDataAsync(this.state.selectedPeriod);
        }
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

    onClickPeriod() {
        let periodParameter = window.location.pathname.split("/").pop();
        if (periodParameter.length == 0) {
            periodParameter = SelectPeriodName.TODAY
        }
        dystirWebClientService.state.matchesData.selectedPeriod = periodParameter;
        this.setState({
            selectedPeriod: periodParameter
        });
    }

    render() {
        let contents =
            <>
                <div id="competitions_selection">
                    <div id="horizontal_menu">
                        <div id="horizontal_menu_wrapper">
                            <span>DYSTIR</span>
                        </div>
                    </div>
                </div>
                <div className="main_container_shared">
                    {
                        this.state.isLoading &&
                        <PuffLoader className="loading-spinner-parent" color="lightGray" height="50" width="50" />
                    }
                    {
                        this.renderMatches(this.filterMatches(this.state.matches))
                    }
                </div>
            </>
        return (
            <LayoutShared>
                {
                    contents
                }
            </LayoutShared>
        );
    }

    renderMatches(matches) {
        if (matches == null) return;
        const matchesGroup = matches.groupBy(match => { return match.matchTypeName ?? "" });
        return (
            Object.keys(matchesGroup).map(group =>
                <div key={group}>
                    {
                        matchesGroup[group].map(match =>
                            <MatchView key={match.matchID} match={match} page={"portal"} />
                        )
                    }
                </div>
            )
        );
    }

    filterMatches(matches) {
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.addDays(0);
        var toDate = now.addDays(1);

        var list = matches.filter((match) =>
            (new MatchDate(Date.parse(match.time))).dateLocale() > MatchDate.parse(fromDate)
            && (new MatchDate(Date.parse(match.time))).dateLocale() < MatchDate.parse(toDate)
        );

        return list
            .sort((a, b) => a.matchID - b.matchID)
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => a.matchTypeID - b.matchTypeID);
    }
}
