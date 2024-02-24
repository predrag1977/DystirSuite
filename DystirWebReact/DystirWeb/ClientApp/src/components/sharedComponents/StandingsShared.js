import React, { Component } from 'react';
import { PuffLoader, ClipLoader } from 'react-spinners';
import { DystirWebClientService, PageName } from '../../services/dystirWebClientService';
import MatchDate from '../../extentions/matchDate';
import { MatchView } from "./../views/MatchView";
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './../layouts/LayoutDystir';
import { ChooseCompetitions } from './../ChooseCompetitions';
import { StandingView } from './../views/StandingView';
import { BsCaretLeftFill, BsCaretRightFill } from "react-icons/bs";

const dystirWebClientService = DystirWebClientService.getInstance();

export class StandingsShared extends Component {
    static displayName = StandingsShared.name;

    constructor(props) {
        super(props);
        let standingsData = dystirWebClientService.state.standingsData;
        
        this.state = {
            standings: standingsData.standings,
            selectedCompetition: "",
            isLoading: true
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

    onClickCompetition(competition) {
        this.setState({
            selectedCompetition: competition
        });
    }

    render() {
        var standings = (this.state.standings ?? []).filter((standing) => standing.standingTypeID == 2);
        var competitions = [];
        standings.map((group) => {
            competitions.push(group.standingCompetitionName);
        });
        if (this.state.selectedCompetition == "" && competitions.length > 0) {
            this.state.selectedCompetition = competitions[0];
        }
        return (
            <div className="matches_and_competition_selection">
                <div id="horizontal_matches_header" style={{ backgroundColor: "white" }}>
                    <div id="match_details_horizontal_menu" className="scroll-container" style={{ width: "100%"}}>
                        <div id="match_details_horizontal_menu_wrapper">
                        {
                            competitions.map(competition =>
                                <div key={competition}
                                    className={"competition_item tab " + (this.state.selectedCompetition == competition ? "selected_tab" : "")}
                                    onClick={() => this.onClickCompetition(competition)}>
                                    <div className="nav-link">{competition}</div>
                                </div>
                            )
                        }
                        </div>
                    </div>
                </div>
                <div className="main_container" style={{ height: "calc(100% - 53px)", maxWidth: "100%" }}>
                {
                    this.state.isLoading &&
                    <ClipLoader className="loading-spinner-parent" color="gray" height="50" width="50" />
                }
                {
                    this.renderStandings(standings)
                }
                </div>
            </div>
        );
    }

    renderStandings(standings) {
        let standing = standings.filter(
            (standing) => standing.standingCompetitionName == this.state.selectedCompetition);
        
        if (standing.length === 0) return;
        return (
            <StandingView key={standing.standingCompetitionName} standing={standing[0]} isSharedPage={ true } />
        );
    }
}