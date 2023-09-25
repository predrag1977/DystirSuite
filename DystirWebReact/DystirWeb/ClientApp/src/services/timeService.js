export default class TimeService {

    constructor() {
        this.intervalID = setInterval(this.startMatchTime, 1000);
    }
    

    startMatchTime() {
        document.body.dispatchEvent(new CustomEvent("onMatchTime"));
    }
}
