export default class TimeService {
    constructor() {
        let intervalID = setInterval(this.startMatchTime, 1000);
        let intervalSponsorsID = setInterval(this.startSponsorsTime, 10000);
    }

    startMatchTime() {
        document.body.dispatchEvent(new CustomEvent("onMatchTime"));
    }

    startSponsorsTime() {
        document.body.dispatchEvent(new CustomEvent("onSponsorsTime"));
    }
}
