import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import { HeaderMatchDetailsShared } from '../sharedComponents/HeaderMatchDetailsShared';

export class LayoutMatchDetailsShared extends Component {
    static displayName = LayoutMatchDetailsShared.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <HeaderMatchDetailsShared match={this.props.match} />
                <Container tag="main">
                    {this.props.children}
                </Container>
            </>
        );
    }
}
