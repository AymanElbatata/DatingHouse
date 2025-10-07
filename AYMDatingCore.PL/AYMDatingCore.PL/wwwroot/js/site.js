
    // Create connection

    // When like notification received
    connection.on("ReceiveLikeNotification", function () {
        alert("New Like: ");
        });
    connection.on("ReceiveViewNotification", function () {
        alert("New View: ");
        });
    connection.on("ReceiveMessageNotification", function () {
        alert("New Message: ");
        const currentUserCounter_Messages = document.getElementById("currentUserCounter_Messages");
        let count = parseInt(currentUserCounter_Messages.textContent) || 0;
        count++;
        currentUserCounter_Messages.textContent = count;
        });
    connection.on("ReceiveFavoriteNotification", function () {
        alert("New Favorite: ");
        });
    connection.on("ReceiveBlockNotification", function () {
        alert("New Block: ");
    });
