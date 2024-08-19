// service to CRUD chat messages. Should get messages by chatId, update, delete, and create messages
using System;
using System.Collections.Generic;

public interface IMessageService
{
    List<string> GetMessagesByChatId(int chatId);
    void UpdateMessage(int chatId, int messageId, string newMessage);
    void DeleteMessage(int chatId, int messageId);
    void CreateMessage(int chatId, string message);
}

public class MessageService : IMessageService
{
    private Dictionary<int, List<string>> chatMessages;

    public MessageService()
    {
        chatMessages = new Dictionary<int, List<string>>();
    }

    public List<string> GetMessagesByChatId(int chatId)
    {
        if (chatMessages.ContainsKey(chatId))
        {
            return chatMessages[chatId];
        }
        else
        {
            return new List<string>();
        }
    }

    public void UpdateMessage(int chatId, int messageId, string newMessage)
    {
        if (chatMessages.ContainsKey(chatId))
        {
            List<string> messages = chatMessages[chatId];
            if (messageId >= 0 && messageId < messages.Count)
            {
                messages[messageId] = newMessage;
            }
            else
            {
                throw new IndexOutOfRangeException("Invalid message ID");
            }
        }
        else
        {
            throw new KeyNotFoundException("Chat ID not found");
        }
    }

    public void DeleteMessage(int chatId, int messageId)
    {
        if (chatMessages.ContainsKey(chatId))
        {
            List<string> messages = chatMessages[chatId];
            if (messageId >= 0 && messageId < messages.Count)
            {
                messages.RemoveAt(messageId);
            }
            else
            {
                throw new IndexOutOfRangeException("Invalid message ID");
            }
        }
        else
        {
            throw new KeyNotFoundException("Chat ID not found");
        }
    }

    public void CreateMessage(int chatId, string message)
    {
        if (chatMessages.ContainsKey(chatId))
        {
            chatMessages[chatId].Add(message);
        }
        else
        {
            chatMessages.Add(chatId, new List<string> { message });
        }
    }
}