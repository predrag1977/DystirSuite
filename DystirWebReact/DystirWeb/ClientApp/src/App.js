import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import './custom.css';
import DystirWebClientService from './services/dystirWebClientService';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        const dystirWebClientService = new DystirWebClientService();
        dystirWebClientService.init();
    }

    render() {
        return (
            <Layout>
            <Routes>
                {AppRoutes.map((route, index) => {
                const { element, ...rest } = route;
                return <Route key={index} {...rest} element={element} />;
                })}
            </Routes>
            </Layout>
        );
    }
}
