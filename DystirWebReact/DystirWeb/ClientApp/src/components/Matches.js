import React, { Component } from 'react';
import { ThreeDots } from 'react-loading-icons';
import { DystirWebClientService, SelectPeriodName, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
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
        if (!this.state.isMatchesLoaded) {
            this.state.isLoading = true;
            dystirWebClientService.loadMatchesDataAsync(this.state.selectedPeriod);
        }

        window.onpopstate = () => {
            this.onClickPeriod();
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
            <ChooseDays onClickPeriod={() => this.onClickPeriod()} selectedPeriod={this.state.selectedPeriod} />
            <div className="main_container">
            {
                this.state.isLoading &&

                <ThreeDots className="loading-spinner-parent" fill= 'dimGray' height="50" width="50" />
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
        const matchesGroup = matches.groupBy(match => { return match.matchTypeName ?? "" });
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
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.addDays(0);
        var toDate = now.addDays(1);

        if (this.state.selectedPeriod === SelectPeriodName.BEFORE) {
            fromDate = now.addDays(-10);
            toDate = now.addDays(0);
        } else if (this.state.selectedPeriod === SelectPeriodName.YESTERDAY) {
            fromDate = now.addDays(-1);
            toDate = now.addDays(0);
        } else if (this.state.selectedPeriod === SelectPeriodName.TODAY) {
            fromDate = now.addDays(0);
            toDate = now.addDays(1);
        } else if (this.state.selectedPeriod === SelectPeriodName.TOMORROW) {
            fromDate = now.addDays(1);
            toDate = now.addDays(2);
        } else if (this.state.selectedPeriod === SelectPeriodName.NEXT) {
            fromDate = now.addDays(0);
            toDate = now.addDays(10);
        }

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
