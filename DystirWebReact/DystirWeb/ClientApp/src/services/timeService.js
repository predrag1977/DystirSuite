var connected = true;

export default class TimeService {
    constructor() {
        let intervalID = setInterval(this.startMatchTime, 1000);
        let intervalSponsorsID = setInterval(this.startSponsorsTime, 10000);

        document.body.addEventListener('onConnected', this.onConnected.bind(this));
        document.body.addEventListener('onDisconnected', this.onDisconnected.bind(this));
    }

    startMatchTime() {
        document.body.dispatchEvent(new CustomEvent('onMatchTime', { detail: { connected: connected }}));
    }

    startSponsorsTime() {
        document.body.dispatchEvent(new CustomEvent("onSponsorsTime"));
    }

    onConnected() {
        connected = true;
    }

    onDisconnected() {
        connected = false;
    }
}
