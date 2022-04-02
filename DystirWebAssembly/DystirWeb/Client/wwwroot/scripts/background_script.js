//<div id="blazor-error-ui" style="background-color: darkkhaki; width: 100%; text-align: center; position: absolute; bottom: 0; z-index: 3000 ">
//    An unhandled error has occurred.
//    <a href="" class="reload" style="font-size:1.1rem; color:black; font-weight:600; margin: 5px auto; display: inline-block;  ">Reload</a>
//    <a class="dismiss">🗙</a>
//</div>

if (top !== self) {
    document.body.innerHTML =
        `<div id="app">
            <div style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: white; "></div>
            <div class="loading-spinner-parent">
                <i class="fas fa-spin fa-spinner fa-2x" style="color:darkgrey"></i>
            </div>
        </div>`;
}
else {
    document.body.innerHTML =
        `<div id="app">
            <div style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgba(34, 34, 34, 0.9); "></div>
            <div class="loading-spinner-parent">
                <i class="fas fa-spin fa-spinner fa-2x" style="color:darkgrey"></i>
            </div>
        </div>`;
}