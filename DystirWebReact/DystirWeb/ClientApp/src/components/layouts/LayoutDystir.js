import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import { NavMenu } from '../NavMenu';
import '../../css/dystir.css?version=4';

export class LayoutDystir extends Component {
    static displayName = LayoutDystir.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="background">
                <NavMenu />
                <Container tag="main">
                    {this.props.children}
                </Container>
            </div>
        );
    }
}
