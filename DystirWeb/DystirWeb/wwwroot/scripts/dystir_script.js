var connection = new signalR.HubConnectionBuilder().withUrl("/dystirHub").build();
var isConnected = true;

startHubConnection();

function startHubConnection() {
    connection.start().then(function () {
        console.log("HubConnection started " + new Date());
        $(".no-connection").css("display", "none");
        if (!isConnected) {
            isConnected = true;
            location.reload();
        }
    }).catch(function (err) {
        console.log(err.toString() + " " + new Date());
        refresh();
    });
}

connection.onclose(function (e) {
    console.log("HubConnection close " + new Date());
    $(".no-connection").css("display", "block");
    refresh();
});

function refresh() {
    isConnected = false;
    setTimeout(function () {
        startHubConnection();
    }, 3000);
} 