import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { NavMenu } from './NavMenu';
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseDays } from './ChooseDays';
import { StandingView } from './views/StandingView';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Standings extends Component {
    static displayName = Standings.name;

    constructor(props) {
        super(props);
        let standingsData = dystirWebClientService.state.standingsData;
        this.state = {
            standings: standingsData.standings,
            selectedStandingsCompetition: standingsData.selectedStandingsCompetition,
            isLoading: true
        }
        dystirWebClientService.loadStandingsDataAsync("Betri deildin");
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
            standings: dystirWebClientService.state.standingsData.standings,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadStandingsDataAsync("Betri deildin");
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    render() {
        let contents =
            <>
                <ChooseDays />
                <div className="main_container">
                    {
                        (this.state.standings === null || this.state.isLoading) &&
                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        this.renderStandings(this.state.standings)
                    }
                </div>
            </>
        
        return (
            <LayoutDystir page={PageName.STANDINGS}>
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderStandings(standings) {
        if (standings == null) return;
        const standingsGroup = standings.groupBy(standing => { return standing.standingCompetitionName });
        return (
            Object.keys(standingsGroup).map(group =>
                <div key={group}>
                    <div className="match-group-competition-name">{group ?? ""}</div>
                    {
                        standingsGroup[group].map(standing =>
                            <StandingView key={standing.standingCompetitionName} standing={standing} />
                        )
                    }
                </div>
            )
        );
    }

    filterMatches(matches) {
        if (matches == null || matches.lenght == 0) {
            return matches;
        }

        var fromDate = new MatchDate().dateUtc();

        var fixtures = matches.filter((match) =>
            MatchDate.parse(match.time) >= MatchDate.parse(fromDate)
            && match.statusID < 30);

        return fixtures.filter((match) => match.matchTypeName == fixtures[0].matchTypeName);
    }
}