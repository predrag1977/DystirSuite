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
        window.addEventListener('resize', this.scrollButtonVisibility);
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateMatch', this.onReloadData.bind(this));
        window.removeEventListener('resize', this.scrollButtonVisibility);
    }

    componentWillUnmount() {
        document.body.removeEventListener('onReloadData', this.onReloadData.bind(this));
        document.body.removeEventListener('onConnected', this.onConnected.bind(this));
        document.body.removeEventListener('onDisconnected', this.onDisconnected.bind(this));
        document.body.removeEventListener('onUpdateMatchDetails', this.onReloadData.bind(this));
    }

    componentDidUpdate() {
        this.scrollButtonVisibility();
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
                <div id="horizontal_matches">
                    <div id="scroll_button_left" onClick={() => this.scrollOnClick('left')}>
                        <BsCaretLeftFill />
                    </div>
                    <div id="match_details_horizontal_menu">
                        <div id="match_details_horizontal_menu_wrapper">
                        {
                            competitions.map(competition =>
                                <div className={"cometition_item tab " + (this.state.selectedCompetition == competition ? "selected_tab" : "")}
                                    onClick={() => this.onClickCompetition(competition)}>
                                    <div className="nav-link">{competition}</div>
                                </div>
                            )
                        }
                        </div>
                    </div>
                    <div id="scroll_button_right" onClick={() => this.scrollOnClick('right')}>
                        <BsCaretRightFill />
                    </div>
                </div>
                
                <div id="horizontal_matches" style={{ backgroundColor: "white" }} >
                {
                    matchesGroup[this.state.selectedCompetition]?.map(match =>
                        <div key={match.matchID} className="match_item_same_day match_item_same_day_horizontal">
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

    scrollButtonVisibility() {
        var position = "";
        var horizontalMenu = document.getElementById('match_details_horizontal_menu' + position);
        var horizontalMenuScroll = document.getElementById('match_details_horizontal_menu_wrapper' + position);
        if (horizontalMenu == null) {
            return;
        }
        var scrollButtonLeft = document.getElementById('scroll_button_left' + position);
        var scrollButtonRight = document.getElementById('scroll_button_right' + position);

        if (horizontalMenu.offsetWidth >= horizontalMenuScroll.offsetWidth) {
            scrollButtonLeft.style.visibility = "hidden";
            scrollButtonRight.style.visibility = "hidden";
        }
        else {
            scrollButtonLeft.style.visibility = "visible";
            scrollButtonRight.style.visibility = "visible";
        }
    }

    scrollOnClick(direction) {
        var horizontalMenu = document.getElementById('match_details_horizontal_menu');
        if (direction == 'left') {
            horizontalMenu.scrollLeft -= horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
        } else {
            horizontalMenu.scrollLeft += horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
        }
    }
}
