import React, { Component } from 'react';
import { ThreeDots } from 'react-loading-icons'
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseCompetitions } from './ChooseCompetitions';
import { StandingView } from './views/StandingView';
import { Sponsors } from './Sponsors';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Standings extends Component {
    static displayName = Standings.name;

    constructor(props) {
        super(props);
        let standingsData = dystirWebClientService.state.standingsData;
        if (standingsData.selectedStandingsCompetitionId !== undefined && standingsData.selectedStandingsCompetitionId !== "") {
            window.history.replaceState(null, null, "/standings/" + standingsData.selectedStandingsCompetitionId);
        } else {
            standingsData.selectedStandingsCompetitionId = window.location.pathname.split("/standings").pop().replace('/', '');
        }
        
        this.state = {
            standings: standingsData.standings,
            selectedStandingsCompetitionId: standingsData.selectedStandingsCompetitionId,
            isLoading: true
        }
        if (this.state.selectedStandingsCompetitionId !== undefined && this.state.selectedStandingsCompetitionId !== "") {
            window.history.replaceState(null, null, "/standings/" + this.state.selectedStandingsCompetitionId);
        }
        dystirWebClientService.loadStandingsDataAsync(this.state.selectedStandingsCompetitionId);
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
        dystirWebClientService.loadStandingsDataAsync(this.state.selectedStandingsCompetitionId);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickCompetition() {
        let periodParameter = window.location.pathname.split("/").pop();
        dystirWebClientService.state.standingsData.selectedStandingsCompetitionId = periodParameter;
        this.setState({
            selectedStandingsCompetitionId: periodParameter
        });
    }

    render() {
        const standings = this.state.standings ?? [];
        const competitions = [];
        standings.map((group) => {
            competitions.push(group.standingCompetitionName);
        });
        const selectedStandingsCompetitionId = this.state.selectedStandingsCompetitionId !== ""
            ? this.state.selectedStandingsCompetitionId : (competitions.length > 0 ? 0 : "");

        let contents =
            <>
                <ChooseCompetitions onClickCompetition={() => this.onClickCompetition()}
                    competitions={competitions}
                    page="standings"
                    selectedCompetition={competitions[selectedStandingsCompetitionId]} />
                <div className="main_container">
                {
                    this.state.isLoading &&
                        <ThreeDots className="loading-spinner-parent" fill= 'khaki' height="50" width="50" />
                }
                {
                    this.renderStandings(standings)
                }
                <Sponsors />
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
        let standing = standings.filter(
            (standing) => standing.standingCompetitionId == this.state.selectedStandingsCompetitionId)[0]
            ?? (standings.length > 0 ? standings[0] : []);
        if (standing.length === 0) return;
        return (
            <StandingView key={standing.standingCompetitionName} standing={standing} />
        );
    }
}