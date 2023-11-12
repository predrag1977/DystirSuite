import React, { Component } from 'react';
import { DystirWebClientService, SelectPeriodName, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { ChooseDays } from './ChooseDays';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Matches extends Component {
    static displayName = Matches.name;

    constructor(props) {
        super(props);
        let matchesData = dystirWebClientService.state.matchesData;
        if (matchesData.selectedPeriod !== undefined && matchesData.selectedPeriod !== "") {
            window.history.replaceState(null, null, "/matches/" + matchesData.selectedPeriod);
        } else {
            matchesData.selectedPeriod = window.location.pathname.split("/").pop();
        }
        this.state = {
            matches: matchesData.matches,
            selectedPeriod: matchesData.selectedPeriod,
            isLoading: false
        }

        if (this.state.selectedPeriod !== undefined && this.state.selectedPeriod !== "") {
            window.history.replaceState(null, null, "/matches/" + this.state.selectedPeriod);
        }
        if (this.state.matches === null) {
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
        dystirWebClientService.state.matchesData.selectedPeriod = periodParameter;
        this.setState({
            selectedPeriod: periodParameter
        });
    }

    render() {
        let contents =
            <>
                <ChooseDays onClickPeriod={() => this.onClickPeriod()} selectedPeriod={this.state.selectedPeriod} />
                <div className="main_container">
                {
                    (this.state.matches === null || this.state.isLoading) &&

                    <div className="loading-spinner-parent spinner-border" />
                }
                {
                    this.renderMatches(this.filterMatches(this.state.matches))
                }
                </div>
            </>
        return (
            <LayoutDystir page={PageName.MATCHES}>
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderMatches(matches) {
        if (matches == null) return;
        const matchesGroup = matches.groupBy(match => { return match.matchTypeName });
        return (
            Object.keys(matchesGroup).map(group =>
                <div key={group}>
                    <div className="match-group-competition-name">{group}</div>
                    {
                        matchesGroup[group].map(match =>
                            <MatchView key={match.matchID} match={match} />
                        )
                    }
                </div>
            )
        );
    }

    filterMatches(matches) {
        if (matches == null) {
            return matches;
        }
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.dateUtc().addDays(0);
        var toDate = now.dateUtc().addDays(1);

        if (this.state.selectedPeriod === SelectPeriodName.BEFORE) {
            fromDate = now.dateUtc().addDays(-10);
            toDate = now.dateUtc().addDays(0);
        } else if (this.state.selectedPeriod === SelectPeriodName.YESTERDAY) {
            fromDate = now.dateUtc().addDays(-2);
            toDate = now.dateUtc().addDays(-1);
        } else if (this.state.selectedPeriod === SelectPeriodName.TODAY) {
            fromDate = now.dateUtc().addDays(0);
            toDate = now.dateUtc().addDays(1);
        } else if (this.state.selectedPeriod === SelectPeriodName.TOMORROW) {
            fromDate = now.dateUtc().addDays(1);
            toDate = now.dateUtc().addDays(2);
        } else if (this.state.selectedPeriod === SelectPeriodName.NEXT) {
            fromDate = now.dateUtc().addDays(0);
            toDate = now.dateUtc().addDays(10);
        }

        var list = matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
        );

        return list
            .sort((a, b) => a.matchID - b.matchID)
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => a.matchTypeID - b.matchTypeID)            ;
    }
}
