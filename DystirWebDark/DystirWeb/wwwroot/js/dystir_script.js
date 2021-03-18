var doRefresh = false;
var connection = new signalR.HubConnectionBuilder().withUrl("/dystirHub").build(); 
var isSponsorsLoaded = false;

showLoadingSpinner();
startHubConnection();

function startHubConnection() {
    connection.start().then(function () {
        console.log("HubConnection started " + new Date());
        if (doRefresh === true) {
            refresh();
        }
        doRefresh = false;
    }).catch(function (err) {
        console.log(err.toString() + " " + new Date());
        tryToConnect();
    });
}

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var matchID = msg.split(";")[0];
    if (pageindex === 3 || pageindex === 4) {
        refresh();
    } 
});

connection.on("ReceiveFullMatchesData", function (matchID, matchDetailsJson, matchesListJson) {
    if (pageindex === 5) {
        if (typeof selectedMatchID !== "undefined" && typeof matchID !== "undefined" && selectedMatchID + "" === matchID + "") {
            console.log("Refresh match " + selectedMatchID);
            getMatchDetails2(matchDetailsJson, matchesListJson);
        } else {
            getMatchesListDetails2(selectedMatchID, matchesListJson);
        }
    } else {
        refresh2(matchesListJson);
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

function refresh() {
    jQuery.ajaxSetup({ async: true });
    console.log("Refresh " + new Date());
    if (pageindex === 0) {
        getMatches(sessionStorage["selectedDate"]);
    } else if (pageindex === 1) {
        $.get("/Results/LoadResults",
            function (response) {
                getResults(sessionStorage["resultsCompetition"]);
            });
    } else if (pageindex === 2) {
        $.get("/Fixtures/LoadFixtures",
            function (response) {
                getFixtures(sessionStorage["fixturesCompetition"]);
            });
    }
    else if (pageindex === 3) {
        getStandings(sessionStorage["selectedStandings"]);
    } else if (pageindex === 4) {
        getStatistics(sessionStorage["selectedStatistics"]);
    } else if (pageindex === 5) {
        getMatchDetails(selectedMatchID);
    }
}


function getMatches(selectedDate) {
    jQuery.ajaxSetup({ async: true });
    if (selectedDate === undefined) {
        selectedDate = "";
    }
    sessionStorage["selectedDate"] = selectedDate;
    var totalMinutes = new Date().getTimezoneOffset();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var path = "/Matchesview/MatchesList";
    if (pageindex === 6) {
        path = "/InfoMatches/MatchesList";
        selectedDate = "";
    }
    $.get(path, {
        "selecteddate": selectedDate,
        "totalMinutes": totalMinutes
    }, function (response) {
            hideLoadingSpinner();
            $("main").html(response);
            $(".matches-view").scrollTop(previousScrollTop);
            $(".matches-tab").css("border-bottom", "3px solid darkkhaki");
            loadSponsors(sponsorshtml, mainsponsorshtml);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}

function getMatchesFromCompetition(selectedCompetition) {
    if (pageindex === 1) {
        getResults(selectedCompetition);
    } else if (pageindex === 2) {
        getFixtures(selectedCompetition);
    }
}

function getResults(resultsCompetition) {
    if (resultsCompetition === undefined) {
        resultsCompetition = "";
    }
    sessionStorage["resultsCompetition"] = resultsCompetition;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var url = "Results/MatchesList";
    $.get(url, {
        "selectedCompetition": resultsCompetition
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
            $(".results-tab").css("border-bottom", "3px solid darkkhaki");
            loadSponsors(sponsorshtml, mainsponsorshtml);
    });
}

function getFixtures(fixturesCompetition) {
    if (fixturesCompetition === undefined) {
        fixturesCompetition = "";
    }
    sessionStorage["fixturesCompetition"] = fixturesCompetition;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var url = "Fixtures/MatchesList";
    $.get(url, {
        "selectedCompetition": fixturesCompetition
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
        $(".fixtures-tab").css("border-bottom", "3px solid darkkhaki");
        loadSponsors(sponsorshtml, mainsponsorshtml);
    });
}

function getStandings(selectedStandings) {
    if (selectedStandings === undefined) {
        selectedStandings = "";
    }
    sessionStorage["selectedStandings"] = selectedStandings;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var path = "StandingsView/MatchesList";
    if (pageindex === 8) {
        path = "/InfoStandings/MatchesList";
        selectedDate = "";
    }
    $.get(path, {
        "selectedCompetition": selectedStandings
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
            $(".standings-tab").css("border-bottom", "3px solid darkkhaki");
            loadSponsors(sponsorshtml, mainsponsorshtml);
    });
}

function getStatistics(selectedStatistics) {
    if (selectedStatistics === undefined) {
        selectedStatistics = "";
    }
    sessionStorage["selectedStatistics"] = selectedStatistics;
    //var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var path = "StatisticsView/StatisticsList";
    //if (pageindex === 8) {
    //    path = "/InfoStandings/MatchesList";
    //    selectedDate = "";
    //}
    $.get(path, {
        "selectedCompetition": selectedStatistics
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        //$("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
            $(".statistics-tab").css("border-bottom", "3px solid darkkhaki");
            loadSponsors(sponsorshtml, mainsponsorshtml);
    });
}

function getMatchDetails(matchID, titleMain) {
    if (typeof titleMain !== "undefined") {
        var titleOne = titleMain.replace(/\s/g, "").replace(/\//g, "").replace(/\./g, "");
    }
    var previousScrollLeft = $("#matches-list-view").scrollLeft();
    var previousScrollTop = $("#match_details_view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    selectedMatchID = matchID;
    $.get("/MatchDetailsView/SelectedMatchDetails", {
        "selectedMatchID": matchID
    },
        function (response) {
            hideLoadingSpinner();
            $("main").html(response);
            showSelectedTab(selectedTabIndex);
            $("#match_details_view").scrollTop(previousScrollTop);
            $("#matches-list-view").scrollLeft(previousScrollLeft);
            if (typeof matchID !== "undefined" && typeof titleMain !== "undefined") {
                var pageName = "DystarGreiningar";
                var url = "/" + pageName + "/" + matchID + "/" + titleOne;
                history.replaceState("", titleOne, url);
                $(document).prop('title', titleMain);
                $("#header-title").text(titleMain);
            }
            loadSponsors(sponsorshtml, mainsponsorshtml);
        }).done(function () {
            console.log("Success " + new Date());
        }).fail(function () {
            console.log("Error " + new Date());
        });
}

function getMatchesListDetails(selectedMatchID) {
    var previousScrollLeft = $("#matches-list-view").scrollLeft();
    $.get("/MatchDetailsView/MatchesList", {
        "selectedMatchID": selectedMatchID
    },
        function (response) {
            $("#matches-list-details").html(response);
            $("#matches-list-view").scrollLeft(previousScrollLeft);
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
    var backgroundColor = pageindex === 7 ? "#5b6aa1" : "dimgray";
    var textColor = pageindex === 7 ? "white" : "white";
    var selectedBackgroundColor = pageindex === 7 ? "#253978" : "#2F4F2F";
    
    $('.tab-match-details > tr td').css("background-color", backgroundColor);
    $('.tab-match-details > tr td').css("color", textColor);
    $('.tab-match-details > tr td:nth-child(' + tabIndex + ')').css("background-color", selectedBackgroundColor);
    $('.tab-match-details > tr td:nth-child(' + tabIndex + ')').css("color", "white");
    $('.details-tab > tr').css("display", "none");
    $('.details-tab > tr:nth-child(' + tabIndex + ')').css("display", "table-row");
}

function convertToLocalTime(elementID, utcDate) {
    var dateString = utcDate;
    var momentString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").local().format("DD.MM.YYYY - HH:mm");
    var timeString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").format("HH");
    if (timeString === "00") {
        momentString = moment.utc(dateString, "DD.MM.YYYY HH:mm Z").local().format("DD.MM.YYYY");
    }
    $("#" + elementID).text(momentString);
}

// MATCHES TIME AND PERIODS
function calculateMatchTime(matchStatus, elementTime, timeDiff,  timeToStart) {
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
            $(elementTime).css("color", "limegreen");
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

function createMatchDays() {
    var today = moment();
    for (i = -3; i < 4; i++) {
        var fullDayFormat = moment(today).add(i, 'days').format("DD.MM.YYYY HH:mm");
        var date = moment(today).add(i, 'days').format("DD.MM.");
        var day = moment(today).locale("fo").add(i, 'days').format("ddd");
        if (moment(today).format("DD.MM.") === date) {
            $(".matches-days").append('<td style="padding:0"><div class="day-view" onclick="getMatches(&quot;' + fullDayFormat + '&quot;);"><span class="day-view-text">' + date + '</span><br/>Í DAG</div></td>');
        } else {
            $(".matches-days").append('<td style="padding:0"><div class="day-view" onclick="getMatches(&quot;' + fullDayFormat + '&quot;);"><span class="day-view-text">' + date + '</span><br/>' + day.toUpperCase() + '</div></td>');
        }
    }
}

function selectDay(selectedDate) {
    $(".day-view").each(function () {
        if ($(this).children(".day-view-text").text() === selectedDate) {
            $(this).addClass("selected-day");
        } else {
            $(this).removeClass("selected-day");
        }
    });
}

function loadSponsors(sponsorshtml, mainsponsorshtml) {
    if (isSponsorsLoaded === false) {
        $.get("/SponsorsView/GetSponsors",
            function (response) {
                loadMainSponsors();
                $("#sponsors").html(response);
                showSponsorsToFit();
                setInterval(function () {
                    $("#sponsors a:first").insertAfter($("#sponsors a:last"));
                    showSponsorsToFit();
                }, 10000);
                
            });
    } else {
        $("#sponsors").html(sponsorshtml);
        $("#sponsors-main").html(mainsponsorshtml);
    }
}

function loadMainSponsors() {
    $.get("/SponsorsView/GetMainSponsors",
        function (response) {
            $("#sponsors-main").html(response);
            $("#sponsors-main a").each(function (index) {
                if (index > 1) {
                    $(this).hide();
                } else {
                    $(this).show();
                }
            });
            setInterval(function () {
                $("#sponsors-main a:first").insertAfter($("#sponsors-main a:last"));
                $("#sponsors-main a").each(function (index) {
                    if (index > 1) {
                        $(this).hide();
                    } else {
                        $(this).show();
                    }
                });
            }, 10000);
            isSponsorsLoaded = true;
        });
}

$(window).resize(function () {
    showSponsorsToFit();
});

function showSponsorsToFit() {
    var fitNumber = Math.floor($("#sponsors").width() / ($("#sponsors a").first().width() + 3));
    $("#sponsors a").each(function (index) {
        if (index < fitNumber) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

function openNav() {
    if (document.getElementById("mySidenav").style.width === "250px") {
        closeNav();
    }
    else {
        document.getElementById("mySidenav").style.width = "250px";
    }
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}


/*------------------*/
/* UPDATE FROM HUB */
/*----------------*/

function refresh2(matchesListJson) {
    jQuery.ajaxSetup({ async: true });
    console.log("Refresh " + new Date());
    if (pageindex === 0) {
        getMatches2(sessionStorage["selectedDate"], matchesListJson);
    } else if (pageindex === 1) {
        getResults2(sessionStorage["resultsCompetition"], matchesListJson);
    } else if (pageindex === 2) {
        getFixtures2(sessionStorage["fixturesCompetition"], matchesListJson);
    }
}

function getMatches2(selectedDate, matchesListJson) {
    jQuery.ajaxSetup({ async: true });
    if (selectedDate === undefined) {
        selectedDate = "";
    }
    sessionStorage["selectedDate"] = selectedDate;
    var totalMinutes = new Date().getTimezoneOffset();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var path = "/Matchesview/GetMatchesListView";
    $.post(path, {
        "selecteddate": selectedDate,
        "totalMinutes": totalMinutes,
        "matchesList": matchesListJson
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $(".matches-view").scrollTop(previousScrollTop);
        $(".matches-tab").css("border-bottom", "3px solid darkkhaki");
        loadSponsors(sponsorshtml, mainsponsorshtml);
    }).done(function () {
        console.log("Success " + new Date());
    }).fail(function () {
        console.log("Error " + new Date());
    });
}

function getResults2(resultsCompetition, matchesListJson) {
    if (resultsCompetition === undefined) {
        resultsCompetition = "";
    }
    sessionStorage["resultsCompetition"] = resultsCompetition;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var url = "Results/GetMatchesListView";
    $.post(url, {
        "selectedCompetition": resultsCompetition,
        "matchesList": matchesListJson
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
        $(".results-tab").css("border-bottom", "3px solid darkkhaki");
        loadSponsors(sponsorshtml, mainsponsorshtml);
    });
}

function getFixtures2(fixturesCompetition, matchesListJson) {
    if (fixturesCompetition === undefined) {
        fixturesCompetition = "";
    }
    sessionStorage["fixturesCompetition"] = fixturesCompetition;
    var previousScrollLeft = $("#competition-view").scrollLeft();
    var previousScrollTop = $(".matches-view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    var url = "Fixtures/GetMatchesListView";
    $.post(url, {
        "selectedCompetition": fixturesCompetition,
        "matchesList": matchesListJson
    }, function (response) {
        hideLoadingSpinner();
        $("main").html(response);
        $("#competition-view").scrollLeft(previousScrollLeft);
        $(".matches-view").scrollTop(previousScrollTop);
        $(".fixtures-tab").css("border-bottom", "3px solid darkkhaki");
        loadSponsors(sponsorshtml, mainsponsorshtml);
    });
}

function getMatchesListDetails2(selectedMatchID, matchesListJson) {
    var previousScrollLeft = $("#matches-list-view").scrollLeft();
    $.post("/MatchDetailsView/GetMatchesList", {
        "selectedMatchID": selectedMatchID,
        "matchesList": matchesListJson
    },
        function (response) {
            $("#matches-list-details").html(response);
            $("#matches-list-view").scrollLeft(previousScrollLeft);
        }).done(function () {
            console.log("Success " + new Date());
        }).fail(function () {
            console.log("Error " + new Date());
        });
}

function getMatchDetails2(matchDetailsJson, matchesListJson) {
    var previousScrollLeft = $("#matches-list-view").scrollLeft();
    var previousScrollTop = $("#match_details_view").scrollTop();
    var sponsorshtml = $("#sponsors").html();
    var mainsponsorshtml = $("#sponsors-main").html();
    $.post("/MatchDetailsView/GetMatchDetailsView", {
        "matchDetails": matchDetailsJson,
        "matchesList": matchesListJson
    },
        function (response) {
            hideLoadingSpinner();
            $("main").html(response);
            showSelectedTab(selectedTabIndex);
            $("#match_details_view").scrollTop(previousScrollTop);
            $("#matches-list-view").scrollLeft(previousScrollLeft);
            loadSponsors(sponsorshtml, mainsponsorshtml);
        }).done(function () {
            console.log("Success " + new Date());
        }).fail(function (e) {
            console.log("Error: " + e + " " + new Date());
        });
}