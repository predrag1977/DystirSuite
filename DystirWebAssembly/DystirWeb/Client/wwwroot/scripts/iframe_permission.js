
if (window.top !== window.self) {
    var url = document.location.ancestorOrigins[0];
    document.body.innerHTML = "";
    if (url === "https://nsi.fo") {
        document.body.innerHTML = "";
    }
    else if (url === "https://www.dimma.fo") {
        document.body.innerHTML =
        `<div id="app">
            <div style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: white; "></div>
            <div class="loading-spinner-parent">
                <i class="fas fa-spin fa-spinner fa-2x" style="color:darkgrey"></i>
            </div>
        </div>`
    }
    else if (url === "https://www.w3schools.com") {
        document.body.innerHTML =
        `<div id="app">
            <div style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: red; "></div>
            <div class="loading-spinner-parent">
                <i class="fas fa-spin fa-spinner fa-2x" style="color:darkgrey"></i>
            </div>
        </div>`
    }
    else {
        document.body.innerHTML =
        `<div id="app">
            <div style='font-size:18px; width:100%;height:100vh;text-align:center;padding-top:30vh'>
                <a style='color:black;' href='mailto:dystir@dystir.fo'>
                    Please, send email to dystir@dystir.fo
                </a>
            </div>
        </div>`
    }
}
else {
    document.body.innerHTML =
    `<div id="app">
        <div style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgba(34, 34, 34, 0.9); "></div>
        <div class="loading-spinner-parent">
            <i class="fas fa-spin fa-spinner fa-2x" style="color:darkgrey"></i>
        </div>
    </div>`
}

