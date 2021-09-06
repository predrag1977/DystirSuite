//var connection = new signalR.HubConnectionBuilder().withUrl("/dystirHub").build();
//var isConnected = true;
//var clientService;

//function initClientService(clientServiceObject) {
//    clientService = clientServiceObject;
//}

//function start() {
//    connection.start()
//        .catch(function () {
//            connection.stop();
//            refresh();
//        })
//        .then(function () {
//            if (!isConnected) {
//                isConnected = true;
//                tryToReload();
//            }
//        });
//}

//function tryToReload() {
//    //console.log("Try to reload");

//    $.ajax({
//        url: '',
//        success: function (result) {
//            location.reload(true);
//        },
//        error: function (result) {
//            setTimeout(function () {
//                tryToReload();
//            }, 300);
//        }
//    });
//}

//connection.on("UpdateCommand", (matchID) => {
//    sendUpdate(matchID);
//});

//connection.onreconnected(function () {
//    //connectionReconnected();
//});

//connection.onclose(function () {
//    refresh();
//    connectionDisconnected();
//});

//function refresh() {
//    isConnected = false;
//    setTimeout(function () {
//        start();
//    }, 2000);
//} 

//function connectionConnected() {
//    console.log("connectionConnected " + new Date());
//    clientService.invokeMethodAsync('ConnectionConnected')
//        .catch(function (err) {
//            console.error(err + " " + new Date());
//            tryToReload();
//        }).then(data => {
//            console.log(data);
//        });
//}

//function connectionReconnected() {
//    clientService.invokeMethodAsync('ConnectionReconnected')
//        .catch(function (err) {
//            console.error(err + " " + new Date());
//            tryToReload();
//        }).then(data => {
//            console.log(data);
//        });
//}

//function connectionDisconnected() {
//    console.log("Connection closed " + new Date());
//    //clientService.invokeMethodAsync('ConnectionDisconnected')
//    //    .then(data => {
//    //        console.log(data);
//    //    })
//    //    .catch(function (err) {
//    //        console.log(err.toString() + " " + new Date());
//    //        //location.reload(true);
//    //    });
//}

//function sendUpdate(matchID) {
//    //console.log(matchID);
//    clientService.invokeMethodAsync('ConnectionUpdated', matchID)
//        .catch(function (err) {
//            console.log(err.toString() + " " + new Date());
//        });
//}

//start();

//var websocket = new WebSocket("wss://www.dystir.fo/_blazor");
////var websocket = new WebSocket("ws://localhost:64974/_blazor");


//websocket.onopen = function (e) {
//};

//websocket.onclose = function (event) {
//    if (event.code === 1006) {
//        console.log("Web socket closed " + event.code + " " + new Date());
//        setTimeout(function () {
//            //location.reload(true);
//        }, 2000);
//    }
//};

//function stopConnection() {
//    connection.stop();
//}