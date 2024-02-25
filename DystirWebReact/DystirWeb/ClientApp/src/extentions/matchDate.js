import moment from 'moment';
import 'moment/locale/fo';

export default class MatchDate extends Date {

    dateUtc() {
        return new MatchDate(this.getTime() + this.getTimezoneOffset() * 60000);
    }

    addDays(days) {
        return new MatchDate(this.getTime() + days * 24 * 60 * 60000);
    }

    toTimeString() {
        return moment(this);
    }

    toDateTimeUtc(dateTime) {
        return moment.utc(dateTime);
    }

    toDateTimeLocale(dateTime) {
        let pattern = "ddd DD.MM. HH:mm";
        if (this.toDateTimeUtc(dateTime).format("HH:mm") === "00:00") {
            pattern = "ddd DD.MM.";
        }
        var dateTimeString = this.toDateTimeUtc(dateTime).local().locale("fo").format(pattern);
        if (dateTimeString.length > 0) {
            return dateTimeString.charAt(0).toUpperCase() + dateTimeString.slice(1);
        }
        return "";
    }

    timeLocale(dateTime) {
        if (this.toDateTimeUtc(dateTime).format("HH:mm") !== "00:00") {
             return this.toDateTimeUtc(dateTime).local().locale("fo").format("HH:mm");
        }
        return "";
    }
}