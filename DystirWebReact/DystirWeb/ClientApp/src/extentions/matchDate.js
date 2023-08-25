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
        const dateTimeString = moment(this).locale("fo").format("ddd DD.MM. HH:mm");
        return dateTimeString.charAt(0).toUpperCase() + dateTimeString.slice(1);
    }
}