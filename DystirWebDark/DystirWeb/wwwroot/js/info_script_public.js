function matchDetailsLoad() {
    $("body").ready(function () {
        $("#dystir-iframe").attr("src", "https://www.dystir.fo/InfoDystarGreiningar/" + sessionStorage["infoMatchDetails"]);
    });
}

function getMatchDetailsFromDystir(infoSelectedMatchID, title) {
    if (infoSelectedMatchID === undefined) {
        infoSelectedMatchID = "";
    }
    sessionStorage["infoMatchDetails"] = infoSelectedMatchID;
	//Info match detals page
    window.open("/InfoDystarGreiningar/" + infoSelectedMatchID);
}