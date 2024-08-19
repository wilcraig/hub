// wwwroot/js/chat-widget.js

(function () {

    const chatBox = document.createElement('div');
    chatBox.id = 'chat-box';
    chatBox.style.position = 'fixed';
    chatBox.style.bottom = '0';
    chatBox.style.right = '0';
    chatBox.style.width = '400px';
    chatBox.style.height = 'auto'; // Set height to automatically fit content
    chatBox.style.border = 'none'; // Remove borders
    chatBox.style.backgroundColor = 'transparent'; // Set background color to transparent
    chatBox.style.display = 'flex';
    chatBox.style.flexDirection = 'column';
    chatBox.style.zIndex = '9999';
    chatBox.style.borderTopLeftRadius = '10px'; // Add rounded corners to top left
    chatBox.style.borderTopRightRadius = '10px'; // Add rounded corners to top right
    chatBox.style.boxShadow = '0px 0px 10px rgba(0, 0, 0, 0.2)'; // Add drop shadow
    document.body.appendChild(chatBox);

    const chatBoxTitle = document.createElement('div');
    chatBoxTitle.classList.add('chat-box-title');
    chatBoxTitle.style.backgroundColor = '#ffb319'; // Change to a darker shade of orange
    chatBoxTitle.style.color = 'white';
    chatBoxTitle.style.padding = '10px';
    chatBoxTitle.style.cursor = 'pointer';
    chatBoxTitle.style.borderTopLeftRadius = '10px'; // Add rounded corners to top left
    chatBoxTitle.style.borderTopRightRadius = '10px'; // Add rounded corners to top right
    chatBoxTitle.style.fontWeight = 'bold';
    chatBoxTitle.style.fontSize = '1.2em';
    chatBoxTitle.innerHTML = 'Staynchat <span class="carat" style="float: right;">&#9650;</span>';
    chatBox.appendChild(chatBoxTitle);

    // Create chatBoxContent
    const chatBoxContent = document.createElement('div');
    chatBoxContent.classList.add('chat-box-content');
    chatBoxContent.style.display = 'flex';
    chatBoxContent.style.height = '100%';
    chatBoxContent.style.flexDirection = 'column';
    chatBoxContent.style.backgroundColor = 'white';
    chatBoxContent.style.display = 'none';
    chatBox.appendChild(chatBoxContent);

    // Inject Chat List UI
    const chatList = document.createElement('div');
    chatList.id = 'chat-list';
    chatList.style.flex = '1';
    chatList.style.overflowY = 'auto';
    chatBoxContent.appendChild(chatList);

    // Create Chat Input Container
    const chatInputContainer = document.createElement('div');
    chatInputContainer.id = 'chat-input-container';
    chatInputContainer.style.display = 'none';
    chatInputContainer.style.flexDirection = 'column';
    chatInputContainer.style.boxShadow = '0 1px 3px rgba(0, 0, 0, 0.1)';
    chatInputContainer.style.borderBottom = '1px solid #ccc';
    chatInputContainer.style.padding = '10px';
    chatInputContainer.style.flexGrow = '1'; // Fill the rest of the empty space
    chatInputContainer.style.position = 'relative'; // Position relative to the parent
    chatBoxContent.appendChild(chatInputContainer);

    // Create Back Arrow
    const backArrow = document.createElement('div');
    backArrow.innerHTML = '&larr; <span style="font-size: 0.5em; font-style: italic;">back</span>';
    backArrow.style.cursor = 'pointer';
    backArrow.style.marginBottom = '10px';
    backArrow.style.fontSize = '1.8em'; // Increase font size to make arrow larger
    chatInputContainer.appendChild(backArrow);

    // Create Input Wrapper
    const inputWrapper = document.createElement('div');
    inputWrapper.style.display = 'flex';
    inputWrapper.style.alignItems = 'center';
    inputWrapper.style.justifyContent = 'space-between'; // Align items to the ends
    inputWrapper.style.position = 'absolute'; // Pin the inputWrapper to the bottom
    inputWrapper.style.bottom = '0';
    inputWrapper.style.width = '100%';
    inputWrapper.style.backgroundColor = 'white';
    chatInputContainer.appendChild(inputWrapper);

    // Create Chat Input
    const chatInput = document.createElement('input');
    chatInput.id = 'chat-input';
    chatInput.type = 'text';
    chatInput.style.flex = '1';
    chatInput.style.marginRight = '3px';
    chatInput.style.borderRadius = '5px';
    chatInput.style.border = '1px solid #ccc';
    chatInput.placeholder = 'Type a message...';
    inputWrapper.appendChild(chatInput);

    // Create Send Button
    const sendButton = document.createElement('button');
    sendButton.id = 'chat-send';
    sendButton.style.padding = '10px';
    sendButton.style.borderRadius = '5px';
    sendButton.style.border = 'none';
    sendButton.style.backgroundColor = '#4CAF50';
    sendButton.style.color = 'white';
    sendButton.style.cursor = 'pointer';
    sendButton.style.marginRight = '12px';
    sendButton.textContent = 'Send';
    inputWrapper.appendChild(sendButton);

    // Add event listener to back arrow
    backArrow.addEventListener('click', () => {
        chatInputContainer.classList.remove('slide-in');
        chatInputContainer.classList.add('slide-out');
        setTimeout(() => {
            chatInputContainer.classList.add('hidden');
        }, 300); // Match the transition duration

        chatList.classList.remove('hidden');
        chatList.classList.remove('slide-out');
        chatList.classList.add('slide-in');
    });

    // Event listener for chat box title bar
    const chatBoxTitleBar = document.querySelector('.chat-box-title');
    chatBoxTitleBar.addEventListener('click', () => {
        const carat = document.querySelector('.carat');
        const chatBoxContent = document.querySelector('.chat-box-content');
        if (chatBoxContent.style.display === 'none' || chatBoxContent.classList.contains('slide-down')) {
            chatBoxContent.style.display = 'flex';
            chatBoxContent.style.height = '400px';
            chatBoxContent.classList.remove('slide-down');
            chatBoxContent.classList.add('slide-up');
            carat.innerHTML = '&#9660;'; // Change carat direction to up
        } else {
            chatBoxContent.classList.remove('slide-up');
            chatBoxContent.classList.add('slide-down');
            chatBoxContent.style.height = '0';
            setTimeout(() => {
                chatBoxContent.style.display = 'none';
            }, 500); // Wait for slide-down animation to complete
            carat.innerHTML = '&#9650;'; // Change carat direction to down
        }
    });

    // Create Back Arrow
    backArrow.addEventListener('click', () => {
        chatInputContainer.classList.remove('slide-in');
        chatInputContainer.classList.add('slide-out');
        chatList.classList.remove('slide-out');
        chatList.classList.add('slide-in');
        setTimeout(() => {
            chatInputContainer.style.display = 'none';
            chatList.style.display = 'block';
        }, 500); // Wait for slide-out animation to complete
    });

    // SignalR Setup
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5012/chatHub", {
            accessTokenFactory: () => localStorage.getItem('jwt')
        })
        .build();

    connection.start().then(async () => {
        console.log("Connected to SignalR hub");

        await connection.invoke("GetUserChats");

        // Handle the chats (e.g., display them in the chat widget)
    }).catch(err => console.error(err));

    // Function to build chat node
    function buildChatNode(chat) {
        const chatNode = document.createElement('div');
        chatNode.style.display = 'flex';
        chatNode.style.justifyContent = 'space-between';
        chatNode.style.alignItems = 'center';
        chatNode.style.padding = '10px';
        chatNode.style.borderBottom = '1px solid #ccc';
        chatNode.style.boxShadow = 'inset 0 1px 3px rgba(0, 0, 0, 0.1)'; // Add inner shadow
        chatNode.style.cursor = 'pointer'; // Change cursor to pointer

        const chatIcon = document.createElement('div');
        chatIcon.classList.add('chat-icon');
        chatIcon.style.marginRight = '5px';
        chatIcon.style.width = '40px'; // Set width to 50 pixels
        chatIcon.style.fontSize = '1.5em';
        chatIcon.innerHTML = chat.icon;

        const chatInfo = document.createElement('div');
        chatInfo.style.flex = '1'; // Take up all remaining space
        chatInfo.innerHTML = chat.chatName.length > 80 ? chat.chatName.substring(0, 80) + '...' : chat.chatName;

        const chatStats = document.createElement('div');
        chatStats.innerHTML = chat.unreadMessageCount > 0 ? `<span style="color: blue;">(${chat.unreadMessageCount})</span>` : '';
        chatStats.innerHTML += chat.onlineUserCount > 0 ? `<span style="color: green;">${chat.onlineUserCount} online</span>` : '';

        chatNode.appendChild(chatIcon);
        chatNode.appendChild(chatInfo);
        chatNode.appendChild(chatStats);

        chatNode.setAttribute('data-chat-id', chat.chatId); // Store chat id

        chatNode.addEventListener('click', () => {
            activeChat = chat.ChatId; // Set active chat
            chatList.classList.add('slide-out');
            chatInputContainer.classList.add('slide-in');
            chatList.style.display = 'none';
            chatInputContainer.style.display = 'flex';
        });

        return chatNode;
    }

    // Event listener for receiving user chats
    connection.on("ReceiveUserChats", (chats) => {
        const chatList = document.getElementById('chat-list');
        chatList.innerHTML = '';
        chats.forEach(chat => {
            const chatNode = buildChatNode(chat);
            chatList.appendChild(chatNode);
        });
    });

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

    // CSS for sliding effect
    const style = document.createElement('style');
    style.innerHTML = `
    .slide-out {
        animation: slideOut 0.5s forwards;
    }
    .slide-in {
        animation: slideIn 0.5s forwards;
    }
    @keyframes slideOut {
        from { transform: translateX(0); }
        to { transform: translateX(-100%); }
    }
    @keyframes slideIn {
        from { transform: translateX(100%); }
        to { transform: translateX(0); }
    }

    .chat-box-content {
        overflow: hidden;
        transition: height 0.5s ease;
        height: 0;
        display: none;
    }

    .chat-box-content.slide-up {
        height: auto; /* This will be overridden by JavaScript */
        display: flex;
    }

    .chat-box-content.slide-down {
        height: 0;
    }

    .hidden {
        display: none;
    }
`;
    document.head.appendChild(style);
})();
