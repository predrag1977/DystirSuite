import React, { Component } from 'react';
import { PuffLoader } from 'react-spinners';
import { DystirWebClientService, SelectPeriodName, PageName } from '../../services/dystirWebClientService';
import { scrollButtonVisibility, scrollOnClick } from '../../extentions/scrolling';
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
        this.state = {
            matches: matchesData.matches,
            selectedCompetition: "",
            isLoading: false
        }
        if (this.state.matches.length === 0) {
            this.state.isLoading = true;
            dystirWebClientService.loadMatchesDataAsync();
        }
    }

    componentDidMount() {
        document.body.addEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.addEventListener('onUpdateMatch', this.onReloadData.bind(this));
        window.addEventListener('resize', this.onResize.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateMatch', this.onReloadData.bind(this));
        window.removeEventListener('resize', this.onResize.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateMatchDetails', this.onReloadData.bind(this));
    }

    componentDidUpdate() {
        scrollButtonVisibility();
        this.sizeOfMatchItem();
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
        this.setState({
            selectedCompetition: competition
        });
    }

    onResize() {
        scrollButtonVisibility();
        this.sizeOfMatchItem();
    }

    render() {
        const matches = this.filterMatches(this.state.matches);
        if (matches == null) return;
        const matchesGroup = matches.groupBy(match => { return match.matchTypeName });

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
                <PuffLoader className="loading-spinner-parent" color="lightGray" height="50" width="50" />
            }
                <div id="horizontal_matches_header">
                    <div id="scroll_button_left" onClick={() => scrollOnClick('left')}>
                        <BsCaretLeftFill />
                    </div>
                    <div id="match_details_horizontal_menu">
                        <div id="match_details_horizontal_menu_wrapper">
                        {
                            competitions.map(competition =>
                                <div key={competition}
                                    className={"cometition_item tab " + (this.state.selectedCompetition == competition ? "selected_tab" : "")}
                                    onClick={() => this.onClickCompetition(competition)}>
                                    <div className="nav-link">{competition}</div>
                                </div>
                            )
                        }
                        </div>
                    </div>
                    <div id="scroll_button_right" onClick={() => scrollOnClick('right')}>
                        <BsCaretRightFill />
                    </div>
                </div>
                
                <div id="horizontal_matches">
                {
                    matchesGroup[this.state.selectedCompetition]?.map(match =>
                        <div key={match.matchID} className="match_item_same_day_share">
                            <MatchHorizontalView match={match} page={"info"} />
                        </div>
                    )
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

    filterMatches(matches) {
        var now = new MatchDate();
        now.setHours(0, 0, 0, 0);

        var fromDate = now.addDays(0);
        var toDate = now.addDays(1);

        var list = matches.filter((match) =>
            (new MatchDate(Date.parse(match.time))).dateLocale() > MatchDate.parse(fromDate) &&
            (new MatchDate(Date.parse(match.time))).dateLocale() < MatchDate.parse(toDate)
        );

        return list
            .sort((a, b) => a.matchID - b.matchID)
            .sort((a, b) => Date.parse(new Date(a.time)) - Date.parse(new Date(b.time)))
            .sort((a, b) => a.matchTypeID - b.matchTypeID);
    }

    sizeOfMatchItem() {
        var elements = document.getElementsByClassName('match_item_same_day_share');
        var matchItemSameDayTeamNames = document.getElementsByClassName('match_item_same_day_team_name');
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.width = "calc(" + 100 / elements.length + "% - 2px)";
            if (elements[i].offsetWidth > 200) {
                elements[i].style.width = "200px";
            }

            for (let j = 0; j < matchItemSameDayTeamNames.length; j++) {
                if (elements[i].offsetWidth < 85) {
                    matchItemSameDayTeamNames[j].style.visibility = "hidden";
                } else {
                    matchItemSameDayTeamNames[j].style.visibility = "visible";
                }
            }
        }
    }
}
