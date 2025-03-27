const connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalrServer")
    .build();

connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err));

connection.on("ReceiveUpdate", function () {
    location.reload();
});