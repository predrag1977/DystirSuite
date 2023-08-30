import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { Route, Routes } from 'react-router-dom';

export class LayoutShared extends Component {
    static displayName = LayoutShared.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Container>
                {this.props.children}
            </Container>
        );
    }
}
