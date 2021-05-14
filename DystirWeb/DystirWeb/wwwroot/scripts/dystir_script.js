var connection = new signalR.HubConnectionBuilder().withUrl("/dystirHub").build();
var isConnected = true;

function start() {
    connection.start().then(function () {
        console.log("HubConnection started " + new Date());
        $(".no-connection").css("display", "none");
        if (!isConnected) {
            isConnected = true;
            tryToReload();
        }
    }).catch(function (err) {
        console.log(err.toString() + " " + new Date());
        refresh();
    });
}

connection.onclose(function (e) {
    console.log("HubConnection close " + new Date());
    //if (e !== undefined) {
    //    console.log("Error for reload: " + e);
    //    $(".no-connection").css("display", "block");
    //    tryToReload();
    //} else {
    //    console.log("Error for refresh: " + e);
    //    refresh();
    //}
    $(".no-connection").css("display", "block");
    refresh();
});

function refresh() {
    isConnected = false;
    setTimeout(function () {
        start();
    }, 2000);
} 

function tryToReload() {
    console.log("Try to reload");

    $.ajax({
        url: '',
        success: function (result) {
            location.reload(true);
        },
        error: function (result) {
            setTimeout(function () {
                tryToReload();
            }, 2000);
        }
    });
}

function reloadDataFromServer() {
    DotNet.invokeMethodAsync("DystirWeb", "ReloadData");
}

start();

var socket = new WebSocket("wss://www.dystir.fo/_blazor");
//var socket = new WebSocket("ws://localhost:64974/_blazor");

socket.onopen = function (e) {
    //alert("[open] Connection established");
};

socket.onclose = function (event) {
    if (event.wasClean) {
        //alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
    } else {
        //alert('[close] Connection died');
        tryToReload();
    }
};

function stopHub() {
    socket.close();
}