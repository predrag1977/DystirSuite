
//Create HubConnection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://www.dystir.fo/dystirHub")
    .withAutomaticReconnect([0, 100, 1000, 2000, 3000, 5000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

//Method for start hub connection
async function start() {
    try {
        await connection.start();
        console.log("Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

//Get signal from RefreshData event
connection.on('RefreshData', () => {
    //Only for testing
    alert('RefreshData');
    console.log('RefreshData');
});

//Get signal and data from ReceiveMessage event
connection.on('ReceiveMessage', (message, updatedMatch) => {
    //Only for testing
    alert(updatedMatch);
    console.log(updatedMatch);
});

//OnReconnected
connection.onreconnected(() => {
    console.log('Reconnected');
});

//Handle disconnected connection
connection.onclose(async () => {
    console.log("Disconnected.");
    await start();
});

// Start the connection.
start();