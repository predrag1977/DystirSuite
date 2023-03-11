var version = new Date().getTime();

if (top !== self) {
    includeCss("app_share");
}
else {
    var url = window.location.href.toLowerCase();
    
    if (url.indexOf("portal") > -1) {
        includeCss("app_share");
        includeCss("dystir");
        includeCss("portal");
    }
    else if (url.indexOf("info") > -1) {
        includeCss("app_share");
        includeCss("dystir");
        includeCss("info");
    }
    else if (url.indexOf("dimma") > -1) {
        includeCss("app_share");
        includeCss("dystir");
        includeCss("dimma");
    }
    else {
        includeCss("app");
        includeCss("dystir");
    }
}

function includeCss(page) {
    document.getElementsByTagName("head")[0].insertAdjacentHTML(
        "beforeend",
        "<link rel=\"stylesheet\" href=\"css/"+ page +".css?" + version + "\" />");
}

function includeCssShareMatches(page) {
    includeCss("dystir");
    includeCss(page);
}
