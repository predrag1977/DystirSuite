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
            import('./css/info_style.css?version=7');
            this.setTitle("Info - Dystir");
            this.setFavicon("https://www.in.fo/favicon.ico");
        } else if (url.includes("portal")) {
            import('./css/portal_style.css?version=7');
            this.setTitle("Portal - Dystir");
            this.setFavicon("https://e02e3c2e19a06eec1e84-9a0707245afee0d6f567aa2987845a0f.ssl.cf1.rackcdn.com/myfiles/1385390388_portal_favicon.ico");
        } else if (url.includes("roysni")) {
            import('./css/roysni_style.css?version=7');
            this.setTitle("Roysni - Dystir");
            this.setFavicon("https://roysni.fo/icons/favicon.ico");
        } else {
            import('./css/dystir_style.css?version=7');
            this.setTitle("Dystir");
            this.setFavicon("../favicon.ico");
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

    setTitle(title) {
        document.title = title;
    }

    setFavicon(favicon) {
        document.querySelector('[rel=icon]').href = favicon;
    }
}
