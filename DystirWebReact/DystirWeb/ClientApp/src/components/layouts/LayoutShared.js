import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { Route, Routes } from 'react-router-dom';

export class LayoutShared extends Component {

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <Container tag="main">
                    {this.props.children}
                </Container>
            </>
        );
    }
}
