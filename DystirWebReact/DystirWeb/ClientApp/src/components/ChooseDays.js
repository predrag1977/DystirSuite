import React, { Component } from 'react';
import MatchDate from '../extentions/matchDate';
import { format } from 'react-string-format';

export class ChooseDays extends Component {
    constructor(props) {
        super(props);
        this.state = DystirWebClientService.matchesData;
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
                    <div className="tab">
                        <span>-10 dagar</span>
                    </div>
                    <div className="tab">
                        <span>Í gjár</span>
                    </div>

                    <div className="tab">
                        <span>Í dag</span>
                    </div>
                    <div className="tab">
                        <span>Í morgin</span>
                    </div>
                    <div class="tab">
                        <span>+10 dagar</span>
                    </div>
                </div> 
            </div>
        );
    }
}
