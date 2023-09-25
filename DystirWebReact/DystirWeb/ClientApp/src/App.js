import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import AppRoutes from './AppRoutes';
import { Layout } from './components/layouts/Layout';
import DystirWebClientService from './services/dystirWebClientService';
//import './custom.css?version=3';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.dystirWebClientService = new DystirWebClientService();
        this.dystirWebClientService.init();
    }

    render() {
        return (
            <Layout>
                <Routes>
                {
                    AppRoutes.map((route, index) => {
                        const { element, ...rest } = route;
                        return <Route key={index} {...rest} element={element} />;
                    })
                }
                </Routes>
            </Layout>
        );
    }
}
