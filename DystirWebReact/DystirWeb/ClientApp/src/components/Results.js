import React, { Component } from 'react';
import { DystirWebClientService, PageName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { MatchView } from "./views/MatchView";
import { groupBy } from "core-js/actual/array/group-by";
import { groupByToMap } from "core-js/actual/array/group-by-to-map";
import { LayoutDystir } from './layouts/LayoutDystir';
import { ChooseCompetitions } from './ChooseCompetitions';

const dystirWebClientService = DystirWebClientService.getInstance();

export class Results extends Component {
    static displayName = Results.name;

    constructor(props) {
        super(props);
        let resultsData = dystirWebClientService.state.resultsData;
        if (resultsData.selectedResultsCompetitionId !== undefined && resultsData.selectedResultsCompetitionId !== "") {
            window.history.replaceState(null, null, "/results/" + resultsData.selectedResultsCompetitionId);
        } else {
            resultsData.selectedResultsCompetitionId = window.location.pathname.split("/results").pop().replace('/','');
        }

        this.state = {
            matches: resultsData.matches,
            selectedResultsCompetitionId: resultsData.selectedResultsCompetitionId,
            isLoading: true
        }
        if (this.state.selectedResultsCompetitionId !== undefined && this.state.selectedResultsCompetitionId !== "") {
            window.history.replaceState(null, null, "/results/" + this.state.selectedResultsCompetitionId);
        }
        dystirWebClientService.loadResultDataAsync(this.state.selectedResultsCompetitionId);
    }

    componentDidMount() {
        document.body.addEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
    }

    onReloadData() {
        this.setState({
            matches: dystirWebClientService.state.resultsData.matches,
            isLoading: false
        });
    }

    onConnected() {
        dystirWebClientService.loadResultDataAsync(this.state.selectedResultsCompetitionId);
    }

    onDisconnected() {
        this.setState({
            isLoading: true
        });
    }

    onClickCompetition() {
        let periodParameter = window.location.pathname.split("/").pop();
        dystirWebClientService.state.resultsData.selectedResultsCompetitionId = periodParameter;
        this.setState({
            selectedResultsCompetitionId: periodParameter
        });
    }

    render() {
        const results = this.filterMatches(this.state.matches)
        const matchesGroup = results.groupBy(match => { return match.matchTypeName });
        const competitions = [];
        Object.keys(matchesGroup).map((group) => {
            competitions.push(group);
        });
        const selectedResultsCompetitionId = this.state.selectedResultsCompetitionId !== ""
            ? this.state.selectedResultsCompetitionId : (competitions.length > 0 ? 0 : "");
        let contents =
            <>
                <ChooseCompetitions onClickCompetition={() => this.onClickCompetition()}
                    competitions={competitions}
                    page={Results.name.toLowerCase()}
                    selectedCompetition={competitions[selectedResultsCompetitionId]} />
                <div className="main_container">
                    {
                        this.state.isLoading &&
                        <div className="loading-spinner-parent spinner-border" />
                    }
                    {
                        competitions.length > 0 &&
                        this.renderResults(matchesGroup, competitions[selectedResultsCompetitionId])
                    }
                </div>
            </>
        return (
            <LayoutDystir page={PageName.RESULTS}>
            {
                contents
            }
            </LayoutDystir>
        );
    }

    renderResults(matchesGroup, competition) {
        if (competition === undefined) {
            return
        }
        const matchesByRound = matchesGroup[competition].groupBy(match => { return match.roundName ?? "" })
        return (
            Object.keys(matchesByRound).map(group =>
                <div key={group}>
                    <div className="match-group-competition-name">{group ?? ""}</div>
                    {
                        matchesByRound[group].map(match =>
                            <MatchView key={match.matchID} match={match} />
                        )
                    }
                </div>
            )
        );
    }

    filterMatches(matches) {
        var fromDate = new MatchDate(new MatchDate().getFullYear(), 1, 1);
        var toDate = new MatchDate().dateUtc();

        return matches.filter((match) =>
            MatchDate.parse(match.time) > MatchDate.parse(fromDate)
            && MatchDate.parse(match.time) < MatchDate.parse(toDate)
            && match.statusID >= 12);
    }
}