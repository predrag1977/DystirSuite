import React, { Component } from 'react';
import { DystirWebClientService } from '../services/dystirWebClientService';
import { MdEmail } from "react-icons/md";
import { FaFacebook } from "react-icons/fa";

const dystirWebClientService = DystirWebClientService.getInstance();

export class Sponsors extends Component {
    constructor(props) {
        super(props);

        this.state = {
            sponsors: dystirWebClientService.state.sponsorsData.sponsors
        }
        
        if (dystirWebClientService.state.sponsorsData.sponsors.length == 0) {
            dystirWebClientService.loadSponsorsDataAsync();
        }
    }

    componentDidMount() {
        document.body.addEventListener('onLoadedSponsorsData', this.onLoadedSponsorsData.bind(this));
        document.body.addEventListener('onSponsorsTime', this.onLoadedSponsorsData.bind(this));
    }

    componentWillUnmount() {
        document.body.removeEventListener('onLoadedSponsorsData', this.onLoadedSponsorsData.bind(this));
        document.body.removeEventListener('onSponsorsTime', this.onLoadedSponsorsData.bind(this));
    }

    onLoadedSponsorsData() {
        let sponsors = dystirWebClientService.state.sponsorsData.sponsors.sort(() => 0.5 - Math.random());
        this.setState({
            sponsors: sponsors
        });
    }

    render() {
        let sponsors = this.state.sponsors;
        return (
            <div className="sponsors_below_matches">
                <div className="box">
                    <div className="matches_list_same_day" style={{ backgroundColor: "transparent", textAlign: "center" }}>
                    {
                        sponsors.filter((sponsor) => sponsor.sponsorId < 100).map((sponsor) =>
                            <a key={sponsor.sponsorId} href={sponsor.sponsorWebSite} target="_blank">
                                <img className="sponsors-main-img" src={sponsor.sponsorsName}/>
                            </a>
                        )
                    }
                    </div>
                </div>
                <div className="box" style={{padding: "0.3rem 0"}} >
                    <div className="matches_list_same_day" style={{ backgroundColor: "transparent", textAlign: "center" }} >
                    {
                        sponsors.map((sponsor) =>
                            <a key={sponsor.sponsorId} href={sponsor.sponsorWebSite} target="_blank">
                                <img className="sponsors-img" src={sponsor.sponsorsName} />
                            </a>
                        )
                    }
                    </div>
                </div>
                <div id="store_buttons" style={{ cursor: "pointer", font: "bold", padding: "25px 0 15px 0", textAlign: "center" }}>
                    <span>
                        <a href='https://play.google.com/store/apps/details?id=fo.Dystir&pcampaignid=MKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1'>
                            <img alt='Get it on Google Play' src="images/icons/google-play-square.png" />
                        </a>
                    </span>
                    <span>
                        <a href='https://apps.apple.com/us/app/dystir/id1460781430?ls=1'>
                            <img alt='Download on Apple Store' src="images/icons/apple-store-square.png" />
                        </a>
                    </span>
                </div>
                <div style={{ cursor: "pointer", font: "bold", padding: "15px 0", textAlign: "center" }}>
                    <a style={{ color: "white", textDecoration: "none" }} target="_blank" href="https://www.facebook.com/profile.php?id=100041392795765">
                        <FaFacebook />
                        <span style={{ fontSize: "1em", textDecoration: "underline", paddingLeft: "4px"  }}>facebook - dystir</span>
                    </a>
                </div>
                <div style={{ cursor: "pointer", font: "bold", padding: "0 0 10px 0", textAlign: "center" }} >
                    <a style={{ color: "white", textDecoration: "none" }} href="mailto: dystir@dystir.fo">
                        <MdEmail />
                        <span style={{ fontSize: "1em", textDecoration: "underline", paddingLeft: "4px" }}>dystir@dystir.fo</span>
                    </a>
                </div>
            </div>
        );
    }
}
