import React, { Component } from 'react';
import { SelectPeriodName } from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { format } from 'react-string-format';
import { BsCaretLeftFill, BsCaretRightFill } from "react-icons/bs";

export class ChooseCompetitions extends Component {
    constructor(props) {
        super(props);
        
    }

    componentDidMount() {
        window.addEventListener('resize', this.scrollButtonVisibility);
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.scrollButtonVisibility);
    }

    componentDidUpdate() {
        this.scrollButtonVisibility();
    }

    render() {
        var page = this.props.page;
        return (
            <div id="competitions_selection">
                <div id="scroll_button_left" onClick={() => this.scrollOnClick('left')}>
                    <BsCaretLeftFill />
                </div>
                <div id="horizontal_menu">
                    <div id="horizontal_menu_wrapper">
                    {
                        this.props.competitions.map((competition) =>
                            <div key={competition}
                                className={"tab " + (this.props.selectedCompetition === competition ? "selected_tab" : "")}
                                onClick={() => this.props.onClickCompetition()}>
                                <NavLink
                                    tag={Link}
                                    to={"/" + page + "/" + this.props.competitions.indexOf(competition)}>
                                    <span>{competition}</span>
                                </NavLink>
                            </div>
                        )
                    }
                    </div>
                </div>
                <div id="scroll_button_right" onClick={() => this.scrollOnClick('right')}>
                    <BsCaretRightFill />
                </div>
            </div>
        );
    }

    scrollButtonVisibility() {
        var position = "";
        var horizontalMenu = document.getElementById('horizontal_menu' + position);
        var horizontalMenuScroll = document.getElementById('horizontal_menu_wrapper' + position);
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
        var horizontalMenu = document.getElementById('horizontal_menu');
        if (direction == 'left') {
            horizontalMenu.scrollLeft -= horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
        } else {
            horizontalMenu.scrollLeft += horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
        }
    }
}
