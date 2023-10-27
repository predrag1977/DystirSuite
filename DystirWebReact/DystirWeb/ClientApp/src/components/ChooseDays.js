import React, { Component } from 'react';
import DystirWebClientService from '../services/dystirWebClientService';
import MatchDate from '../extentions/matchDate';
import { format } from 'react-string-format';

export class ChooseDays extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
    }

    componentWillUnmount() {
    }

    render() {
        const match = this.props.match;
        return (
            <div id="days_selection">
                <div id="horizontal_menu_days">
                    <div className="tab" onClick={() => this.props.periodSelected(0)}>
                        <span>-10 dagar</span>
                    </div>
                    <div className="tab" onClick={() => this.props.periodSelected(1)}>
                        <span>Í gjár</span>
                    </div>
                    <div className="tab" onClick={() => this.props.periodSelected(2)}>
                        <span>Í dag</span>
                    </div>
                    <div className="tab" onClick={() => this.props.periodSelected(3)}>
                        <span>Í morgin</span>
                    </div>
                    <div className="tab" onClick={() => this.props.periodSelected(4)}>
                        <span>+10 dagar</span>
                    </div>
                </div>
            </div>
        );
    }
}
