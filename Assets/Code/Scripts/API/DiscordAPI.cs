using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DiscordAPI
{
    private static readonly string BASE_URL = "https://discord.com/api";
    private static readonly int VERSION = 9;

    private string lastMessage;
    private string botToken;
    private string channelId;

    public void Initialize(string botToken, string channelId)
    {
        this.botToken = botToken;
        this.channelId = channelId;
    }

    private UnityWebRequest GetBaseRequest(string endpoint, string method)
    {
        UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/v{VERSION}{endpoint}", method);

        request.SetRequestHeader("Authorization", $"Bot {botToken}");
        request.SetRequestHeader("User-Agent", "DiscordAPI/0.0.1");
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();

        return request;
    }

    [System.Serializable]
    public struct Author
    {
        public string id;
        public string username;
    }

    [System.Serializable]
    public struct Message
    {
        public string id;
        public Author author;
        public string content;
    }

    [System.Serializable]
    public struct MessagesResponse
    {
        public List<Message> messages;
    }

    public System.Action<List<Message>> OnMessages;
    public System.Action<Message> OnMessage;
    public IEnumerator UpdateMessagesAsync()
    {
        bool firstLoad = lastMessage == null;

        UnityWebRequest request;
        if (lastMessage != null)
        {
            request = GetBaseRequest($"/channels/{channelId}/messages?after={lastMessage}", "GET");
        }
        else
        {
            request = GetBaseRequest($"/channels/{channelId}/messages", "GET");
        }

        yield return request.SendWebRequest();

        string data = request.downloadHandler.text;
        if (request.responseCode == 200)
        {
            MessagesResponse response = JsonUtility.FromJson<MessagesResponse>("{\"messages\":" + data + "}");
            
            if (response.messages.Count > 0)
            {
                lastMessage = response.messages[0].id;

                if (!firstLoad) 
                {
                    if (OnMessages != null) OnMessages(response.messages);
                    if (OnMessage != null)
                    {
                        foreach (Message message in response.messages) OnMessage(message);
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"{request.responseCode} - {data}");
        }
    }

    [System.Serializable]
    public struct MessageReference
    {
        public string message_id;
    }

    [System.Serializable]
    public struct MessageRequest
    {
        public string content;
    }

    [System.Serializable]
    public struct ReplyRequest
    {
        public string content;
        public MessageReference message_reference;
    }

    public IEnumerator SendMessageRequestAsync(object data)
    {
        UnityWebRequest request = GetBaseRequest($"/channels/{channelId}/messages", "POST");

        string jsonData = JsonUtility.ToJson(data);
        byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(requestBody);

        yield return request.SendWebRequest();
    }

    public IEnumerator SendMessageAsync(string message)
    {
        yield return SendMessageRequestAsync(new MessageRequest
        {
            content = message
        });
    }

    public IEnumerator SendReplyAsync(string messageId, string reply)
    {
        yield return SendMessageRequestAsync(new ReplyRequest
        {
            content = reply,
            message_reference = new MessageReference
            {
                message_id = messageId
            }
        });
    }
}
