import moment from 'moment';
import 'moment/locale/fo';

export default class MatchDate extends Date {
    dateUtc() {
        return new MatchDate(this.getTime() + this.getTimezoneOffset() * 60000);
    }

    addDays(days) {
        return new MatchDate(this.getTime() + days * 24 * 60 * 60000);
    }

    toDateTimeString() {
        let pattern = "ddd DD.MM. HH:mm";
        if (this.toTimeString() === "00:00") {
            pattern = "ddd DD.MM.";
        }
        const dateTimeString = moment(this.dateLocale()).locale("fo").format(pattern);
        return dateTimeString.charAt(0).toUpperCase() + dateTimeString.slice(1);
    }

    toTimeString() {
        return moment(this).locale("fo").format("HH:mm");
    }

    dateLocale() {
        return new MatchDate(this.getTime() - this.getTimezoneOffset() * 60000);
    }
}