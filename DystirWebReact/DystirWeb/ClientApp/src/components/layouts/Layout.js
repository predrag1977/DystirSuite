import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class Layout extends Component {
    static displayName = Layout.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            this.props.children
        );
    }
}
