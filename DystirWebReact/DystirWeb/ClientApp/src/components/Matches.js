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
            isLoading: matchesData.isLoading,
            selectedPeriod: matchesData.selectedPeriod
        }

        if (this.state.selectedPeriod !== undefined && this.state.selectedPeriod !== "") {
            window.history.replaceState(null, null, "/matches/" + this.state.selectedPeriod);
        }
        if (this.state.matches === null) {
            dystirWebClientService.loadMatchesDataAsync(this.state.selectedPeriod);
        }
    }

    componentDidMount() {
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.addEventListener('onMatchesDataLoaded', this.onMatchesDataLoaded.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onReceiveMatchDetails', this.onReceiveMatchDetails.bind(this));
        document.body.removeEventListener('onMatchesDataLoaded', this.onMatchesDataLoaded.bind(this));
    }

    onConnected() {
        dystirWebClientService.loadMatchesDataAsync(this.state.selectedPeriod);
    }

    onDisconnected() {
    }

    onReceiveMatchDetails(event) {
        this.setState({
            matches: dystirWebClientService.state.matchesData.matches,
            isLoading: dystirWebClientService.state.matchesData.isLoading
        });
    }

    onMatchesDataLoaded() {
        this.setState({
            matches: dystirWebClientService.state.matchesData.matches,
            isLoading: dystirWebClientService.state.matchesData.isLoading
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
                <div >
                    {
                        (this.state.matches === null || this.state.isLoading) &&

                        <div style={{ height: '100vh' }}>
                            <div className="loading-spinner-parent spinner-border" />
                        </div>
                        
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
            <div className="main_container"  >
            {
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
            }
            </div>
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

        return list;
    }
}
