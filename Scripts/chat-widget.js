// wwwroot/js/chat-widget.js

(function() {
    // Inject Chat UI
    const chatBox = document.createElement('div');
    chatBox.innerHTML = `
        <div id="chat-box" style="z-index: 100; margin-bottom:35px; position: fixed; bottom: 0; right: 0; width: 300px; height: 400px; border: 1px solid #ccc; background: #fff;">
            <div id="chat-messages" style="height: 350px; overflow-y: auto; padding: 10px;"></div>
            <input id="chat-input" type="text" style="width: 80%; padding: 10px;" placeholder="Type a message..." />
            <button id="chat-send" style="width: 20%;">Send</button>
        </div>
    `;
    document.body.appendChild(chatBox);

    // SignalR Setup
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5012/chatHub", {
            accessTokenFactory: () => localStorage.getItem('jwt')
        })
        .build();

    // Method to call InitializeGroup on the server
    async function initializeGroup(userIds) {
        try {
            await connection.invoke("InitializeGroup", userIds);
        } catch (err) {
            console.error(err.toString());
        }
    }

    // Method to handle the ReceiveChatSummary event from the server
    connection.on("ReceiveChatSummary", function (chatSummary) {
        console.log("Chat Summary received:", chatSummary);
        // Update the UI with the chat summary information
        // Example: Display chat name and online user count
        document.getElementById("chatName").innerText = chatSummary.ChatName;
        document.getElementById("onlineUserCount").innerText = chatSummary.OnlineUserCount;
    });

    connection.on("ReceiveMessage", (user, message) => {
        const chatMessages = document.getElementById('chat-messages');
        const messageElement = document.createElement('div');
        messageElement.textContent = `${user}: ${message}`;
        chatMessages.appendChild(messageElement);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    });

    connection.start().catch(err => console.error(err.toString()));

    // Send Message
    document.getElementById('chat-send').addEventListener('click', () => {
        const chatInput = document.getElementById('chat-input');
        const message = chatInput.value;
        connection.invoke("SendMessageToUser", "targetUserId", message).catch(err => console.error(err.toString()));
        chatInput.value = '';
    });

    document.getElementById('chat-input').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            document.getElementById('chat-send').click();
        }
    });
})();
