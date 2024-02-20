import MatchDate from './matchDate';

export default class MatchTimeAndColor {
    getMatchTime(match) {
        var timeNow = new MatchDate().dateUtc().getTime();
        var matchTime = new MatchDate(match?.statusTime?.replace('Z','') ?? 0).getTime();
        var matchStart = new MatchDate(match?.time?.replace('Z', '') ?? 0).getTime();

        var totalMiliseconds = timeNow - matchTime;
        var seconds = Math.floor(totalMiliseconds / 1000);
        var minutes = Math.floor(seconds / 60);
        seconds -= minutes * 60;
        var milsecToStart = matchStart - timeNow;

        return this.getMatchPeriod(minutes, seconds, match?.statusID ?? 0, milsecToStart);
    }

    getMatchPeriod(minutes, seconds, matchStatus, milsecToStart) {
        var addtime = "";
        switch (matchStatus) {
            case 1:
                return this.getTimeToStart(milsecToStart, "00:00");
            case 2:
                if (minutes >= 45) {
                    addtime = "45+";
                    minutes = minutes - 45;
                }
                break;
            case 3:
                return "hálvleikur";
            case 4:
                minutes = minutes + 45;
                if (minutes >= 90) {
                    addtime = "90+";
                    minutes = minutes - 90;
                }
                break;
            case 5:
                return "liðugt";
            case 6:
                minutes = minutes + 90;
                if (minutes >= 105) {
                    addtime = "105+";
                    minutes = minutes - 105;
                }
                break;
            case 7:
                return "longd leiktíð hálvleikur";
            case 8:
                minutes = minutes + 105;
                if (minutes >= 120) {
                    addtime = "120+";
                    minutes = minutes - 120;
                }
                break;
            case 9:
                return "longd leiktíð liðugt";
            case 10:
                return "brotsspark";
            case 11:
            case 12:
            case 13:
                return "liðugt";
            default:
                return this.getTimeToStart(milsecToStart, "-- : --");
        }
        var min = minutes;
        var sec = seconds;
        if (minutes < 10)
            min = "0" + minutes;
        if (seconds < 10)
            sec = "0" + seconds;
        return addtime + " " + min + ":" + sec;
    }

    getTimeToStart(milsecToStart, defaultText) {
        var minutesToStart = Math.ceil(milsecToStart / 60000);
        if (minutesToStart > 0) {
            var days = Math.floor(minutesToStart / 1440);
            var hours = Math.floor((minutesToStart - days * 1440) / 60);
            var minutes = minutesToStart - days * 1440 - hours * 60;
            if (days > 0) {
                return `${days} d. ${hours} t.`;
            }
            else {
                var hoursText = hours > 0 ? hours + " t. " : "";
                return hoursText + minutes + " m.";
            }
        }
        else {
            return defaultText;
        }
    }

    getStatusColor(statusId) {
        let url = window.location.href.toLowerCase();
        let isSharedPage = url.includes("info") || url.includes("portal") || url.includes("roysni");
        switch (statusId) {
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                return "limegreen";
            case 11:
            case 12:
            case 13:
                return "salmon";
            default:
                return isSharedPage ? "darkKhaki" : "khaki";
        }
    }
}