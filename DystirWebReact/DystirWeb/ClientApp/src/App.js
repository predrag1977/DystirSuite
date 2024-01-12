import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Container } from 'reactstrap';
import AppRoutes from './AppRoutes';
import { Layout } from './components/layouts/Layout';
import { LayoutShared } from './components/layouts/LayoutShared';
import { DystirWebClientService } from './services/dystirWebClientService';
import TimeService from './services/timeService';
import './css/app.css?version=6';

const dystirWebClientService = DystirWebClientService.getInstance();

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);

        dystirWebClientService.init();
        this.timeService = new TimeService();
    }

    render() {
        let url = window.location.href.toLowerCase();
        if (url.includes("info")) {
            import('./css/info.css?version=6');
        } else if (url.includes("portal")) {
            import('./css/portal.css?version=6');
        } else {
            import('./css/dystir.css?version=6');
        }
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
