var doRefresh = false;
var connection = new signalR.HubConnectionBuilder().withUrl("/dystirHub").build(); 

showLoadingSpinner();
startHubConnection();

function startHubConnection() {
    connection.start().then(function () {
        console.log("HubConnection started " + new Date());
        if (doRefresh === true) {
            refresh(true);
        }
        doRefresh = false;
    }).catch(function (err) {
        console.log(err.toString() + " " + new Date());
        tryToConnect();
    });
}

connection.on("ReceiveMessage", function (user, message) {
    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //var encodedMsg = user + " says " + msg;
    //var matchID = msg.split(";")[0];
    //var infoMatchID = sessionStorage["infoMatchDetails"];
    //if (typeof infoMatchID !== "undefined" && typeof matchID !== "undefined" && infoMatchID + "" === matchID + "") {
    //    console.log("Refresh match " + infoMatchID);
    //    refresh(true);
    //} else {
    //    refresh(false);
    //}
});

connection.on("ReceiveFullMatchesData", function (matchID, matchDetailsJson, matchesListJson) {
    var infoMatchID = sessionStorage["infoMatchDetails"];
    if (typeof infoMatchID !== "undefined" && typeof matchID !== "undefined" && infoMatchID + "" === matchID + "") {
        console.log("Refresh match " + infoMatchID);
        refresh2(true, matchDetailsJson, matchesListJson);
    } else {
        refresh2(false, matchDetailsJson, matchesListJson);
    }
});

connection.onclose(function (e) {
    console.log("Connection close");
    tryToConnect();
});

function tryToConnect() {
    setTimeout(function () {
        console.log("Try to connect " + new Date());
        doRefresh = true;
        startHubConnection();
    }, 3000);
}

function refresh(isSelectedMatch) {
    jQuery.ajaxSetup({ async: true });
    console.log("Refresh " + new Date());
    if (typeof isSelectedMatch === "undefined") {
        isSelectedMatch = true;
    }
    if (pageindex === 6) {
        $.get("/MatchesView/LoadMatches",
            function (response) {
                getInfoMatches();
            });
    } else if (pageindex === 8) {
        getStandings(sessionStorage["infoSelectedStandings"]);
    } else if (pageindex === 7) {
        getStatistics(sessionStorage["infoSelectedStatistics"]);
    } else if (pageindex === 9) {
        if (isSelectedMatch) {
            getMatchDetails(selectedMatchID, "");
        } else {
            getMatchDetailsSelectionList();
        }
    }
}

function getInfoMatches() {
    var t = "";
    //var previousScrollLeft = $("#header-match-list").scrollLeft();
    var previousScrollTop = $("#match_details_view").scrollTop();
    //var currentInfoDetails = $("#info-match-detail").html();
    var path = "/InfoMatches/InfoMatchesList";
    $.get(path, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        //$("#info-match-detail").html(currentInfoDetails);
        $(".info-matches-tab").css("border-bottom", "3px solid #d88802");
        $("#match_details_view").scrollTop(previousScrollTop);
        //$("#header-match-list").scrollLeft(previousScrollLeft);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}

function getStandings(infoSelectedStandings) {
    if (infoSelectedStandings === undefined) {
        infoSelectedStandings = "";
    }
    sessionStorage["infoSelectedStandings"] = infoSelectedStandings;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    path = "/InfoStandings/MatchesList";
    $.get(path, {
        "selectedCompetition": infoSelectedStandings
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
        $(".info-standings-tab").css("border-bottom", "3px solid #d88802");
    });
}

function getStatistics(infoSelectedStatistics) {
    if (infoSelectedStatistics === undefined) {
        infoSelectedStatistics = "";
    }
    sessionStorage["infoSelectedStatistics"] = infoSelectedStatistics;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var path = "/InfoStatistics/StatisticsList";
    $.get(path, {
        "selectedCompetition": infoSelectedStatistics
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
        $(".info-statistics-tab").css("border-bottom", "3px solid #d88802");
    });
}

function getMatchDetails(infoSelectedMatchID, title) {
    var previousScrollTop = $("#match_details_view").scrollTop();
    if (infoSelectedMatchID === undefined) {
        infoSelectedMatchID = "";
    }
    sessionStorage["infoMatchDetails"] = infoSelectedMatchID;
    selectedMatchID = infoSelectedMatchID;
    $.get("/InfoMatchDetails/SelectedMatchDetails", {
        "selectedMatchID": infoSelectedMatchID
    },function (response) {
            hideLoadingSpinner();
            $("main").html(response);
            showSelectedTab(selectedTabIndex);
            $("#match_details_view").scrollTop(previousScrollTop);
            var url = "/InfoDystarGreiningar/" + infoSelectedMatchID;
            history.replaceState("","", url);
        }).done(function () {
            console.log("Success " + new Date());
        }).fail(function () {
            console.log("Error " + new Date());
        });
}

function getMatchDetailsSelectionList() {
    $.get("/InfoMatchDetails/MatchListSelectionDetails", function (response) {
        //hideLoadingSpinner();
        $("#header-match-details-list").html(response);
        //showSelectedTab(selectedTabIndex);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}

function showLoadingSpinner() {
    $("#loading-image").css("display", "block");
}

function hideLoadingSpinner() {
    $("#loading-image").css("display", "none");
}

function showSelectedTab(tabIndex) {
    selectedTabIndex = tabIndex;
    var backgroundColor = "white";
    var textColor = "black";
    var selectedBackgroundColor = "#999999";
    
    $('.tab-match-details > tr td').css("background-color", backgroundColor);
    $('.tab-match-details > tr td').css("color", textColor);
    $('.tab-match-details > tr td:nth-child(' + tabIndex + ')').css("background-color", selectedBackgroundColor);
    $('.tab-match-details > tr td:nth-child(' + tabIndex + ')').css("color", "white");
    $('.details-tab > tr').css("display", "none");
    $('.details-tab > tr:nth-child(' + tabIndex + ')').css("display", "table-row");
}

function convertToLocalTime(elementID, utcDate) {
    var dateString = utcDate;
    var momentString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").local().format("HH:mm");
    var timeString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").format("HH");
    if (timeString === "00") {
        //momentString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").local().format("DD.MM.YYYY");
        momentString = "--:--";
    }
    $("#" + elementID).text(momentString);
}

function convertToShortLocalTime(elementID, utcDate) {

    var dateString = utcDate;
    var momentString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").local().format("HH:mm");
    var timeString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").format("HH");
    if (timeString === "00") {
        //momentString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").local().format("DD.MM.");
        momentString = "";
    }
    $("#" + elementID).text(momentString);
}


// MATCHES TIME AND PERIODS
function calculateMatchTime(matchStatus, elementTime, timeDiff, timeToStart) {
    console.log(matchStatus + " " + timeDiff + " " + timeToStart);
    mainFunctionCalculateMatchTime(elementTime, timeDiff, matchStatus, timeToStart);
    setInterval(function () {
        mainFunctionCalculateMatchTime(elementTime, timeDiff, matchStatus, timeToStart);
    }, 500);
}

function mainFunctionCalculateMatchTime(elementTime, timeDiff, matchStatus, timeToStart) {
    var d = new Date();
    var milDateNow = d.getTime();
    var num = milDateNow - timeDiff;
    var seconds = Math.floor(num / 1000);
    var minutes = Math.floor(seconds / 60);
    var milsecToStart = timeToStart - milDateNow;
    seconds = seconds - minutes * 60;
    var format = matchPeriod(minutes, seconds, matchStatus, milsecToStart);
    $(elementTime).html(format);
    getStatusColor(elementTime, matchStatus);
    if (format === "-- : --") {
        $(elementTime).css("color", "dimgray");
    }
}

function getStatusColor(elementTime, matchStatus) {
    switch (matchStatus) {
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
            $(elementTime).css("color", "darkgreen");
            break;
        case 11:
        case 12:
        case 13:
            $(elementTime).css("color", "salmon");
            break;
        default:
            $(elementTime).css("color", "darkyellow");
            break;
    }
}

function matchPeriod(minutes, seconds, matchStatus, milsecToStart) {
    var addtime = "";
    if (doRefresh === true && matchStatus < 11) {
        return "-- : --";
    }
    switch (matchStatus) {
        case 1:
            return getTimeToStart(milsecToStart, "00:00");
        case 2:
            if (minutes > 45) {
                addtime = "45+";
                minutes = minutes - 45;
            }
            break;
        case 3:
            return "hálvleikur";
        case 4:
            minutes = minutes + 45;
            if (minutes > 90) {
                addtime = "90+";
                minutes = minutes - 90;
            }
            break;
        case 5:
            return "liðugt";
        case 6:
            minutes = minutes + 90;
            if (minutes > 105) {
                addtime = "105+";
                minutes = minutes - 105;
            }
            break;
        case 7:
            return "longd leiktíð hálvleikur";
        case 8:
            minutes = minutes + 105;
            if (minutes > 120) {
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
            return getTimeToStart(milsecToStart, "-- : --");
    }
    if (minutes < 10)
        minutes = "0" + minutes;
    if (seconds < 10)
        seconds = "0" + seconds;
    return addtime + " " + minutes + ":" + seconds;
}

function getTimeToStart(milsecToStart, defaultText) {
    var minutesToStart = Math.ceil(milsecToStart / 60000);
    if (minutesToStart > 0) {
        var days = Math.floor(minutesToStart / 1440);
        var hours = Math.floor((minutesToStart - days * 1440) / 60);
        var minutes = minutesToStart - days * 1440 - hours * 60;

        if (days > 0) {
            return days + " dag."
        } else {
            var hoursText = hours > 0 ? hours + " t. " : "";
            return hoursText + minutes + " m.";
        }
    } else {
        return defaultText;
    }
}

function selectCompetition(competition) {
    competition = competition.replace(/&#xF8;/g, "ø").replace(/&#xF0;/g, "ð");
    sessionStorage["selectedCompetition"] = competition;
    $(".competition-item").css("background-color", "white").css("color", "black");
    $("#" + competition).css("background-color", "#dff0d8").css("color", "black");
    $('.selection_matches').hide();
    $('.' + competition).show();
}

function chooseCompetition(competition) {
    if (sessionStorage["selectedCompetition"] !== undefined) {
         competition = sessionStorage["selectedCompetition"];
    }
    selectCompetition(competition);
}


/*------------------*/
/* UPDATE FROM HUB */
/*----------------*/

function refresh2(isSelectedMatch, matchDetailsJson, matchesListJson) {
    jQuery.ajaxSetup({ async: true });
    console.log("Refresh " + new Date());
    if (typeof isSelectedMatch === "undefined") {
        isSelectedMatch = true;
    }
    if (pageindex === 6) {
        getInfoMatches2(matchesListJson);
    } else if (pageindex === 9) {
        if (isSelectedMatch) {
            getMatchDetails2(matchDetailsJson, matchesListJson);
        } else {
            getMatchDetailsSelectionList2(matchesListJson);
        }
    }
}

function getInfoMatches2(matchesListJson) {
    var previousScrollTop = $("#match_details_view").scrollTop();
    var path = "/InfoMatches/GetInfoMatchesListView";
    $.post(path, {
        "matchesList": matchesListJson
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $(".info-matches-tab").css("border-bottom", "3px solid #d88802");
        $("#match_details_view").scrollTop(previousScrollTop);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}

function getMatchDetails2(matchDetailsJson, matchesListJson) {
    var previousScrollTop = $("#match_details_view").scrollTop();
    $.post("/InfoMatchDetails/GetSelectedMatchDetailsView", {
        "matchDetails": matchDetailsJson,
        "matchesList": matchesListJson
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        showSelectedTab(selectedTabIndex);
        $("#match_details_view").scrollTop(previousScrollTop);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}

function getMatchDetailsSelectionList2(matchesListJson) {
    $.post("/InfoMatchDetails/GetMatchListSelectionDetailsView", {
        "matchesList": matchesListJson
    }, function (response) {
        $("#header-match-details-list").html(response);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}
