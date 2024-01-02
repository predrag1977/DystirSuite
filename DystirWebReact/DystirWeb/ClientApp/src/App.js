import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import AppRoutes from './AppRoutes';
import { Layout } from './components/layouts/Layout';
import { DystirWebClientService } from './services/dystirWebClientService';
import TimeService from './services/timeService';
//import './custom.css?version=3';

const dystirWebClientService = DystirWebClientService.getInstance()

export default class App extends Component {
    static displayName = App.name;
    

    constructor(props) {
        super(props);
        dystirWebClientService.init();
        this.timeService = new TimeService();
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
