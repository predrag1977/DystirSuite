import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import { HeaderMatchDetails } from '../HeaderMatchDetails';
import '../../css/dystir.css?version=4';

export class LayoutMatchDetails extends Component {
    static displayName = LayoutMatchDetails.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <HeaderMatchDetails match={this.props.match} />
                <Container tag="main">
                    {this.props.children}
                </Container>
            </>
        );
    }
}
