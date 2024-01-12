import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import { Header } from '../Header';

export class LayoutDystir extends Component {
    static displayName = LayoutDystir.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <Header page={this.props.page} />
                <Container tag="main">
                    {this.props.children}
                </Container>
            </>
        );
    }
}
