export default class TimeService {

    constructor() {
        this.intervalID = setInterval(this.startMatchTime, 1000);
        this.intervalSponsorsID = setInterval(this.startSponsorsTime, 10000);
    }

    startMatchTime() {
        document.body.dispatchEvent(new CustomEvent("onMatchTime"));
    }

    startSponsorsTime() {
        document.body.dispatchEvent(new CustomEvent("onSponsorsTime"));
    }
}
