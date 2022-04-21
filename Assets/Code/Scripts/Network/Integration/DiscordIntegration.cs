using UnityEngine;
using System.Collections.Generic;

public class DiscordIntegration : Integration
{
    private DiscordAPI client;
    private string channel;
    private float lastRequest;
    private static readonly float UPDATE_INTERVAL = 2.0f;

    public override void Connect()
    {
        client = null;

        if (AuthManager.Singleton.DiscordChannel.Trim() == "") return;

        client = new DiscordAPI();
        client.Initialize(Secrets.Discord.BOT_TOKEN, AuthManager.Singleton.DiscordChannel);

        client.OnMessage += OnMessage;

        UpdateMessages();

        OnConnectedInner(new OnConnectedArgs
        {
            integration = this
        });
    }

    public override void Disconnect()
    {
        client = null;
    }

    private void Update()
    {
        if (client == null) return;

        if (Time.time - lastRequest > UPDATE_INTERVAL)
        {
            lastRequest = Time.time;
            StartCoroutine(client.UpdateMessagesAsync());
        }
    }

    public override void SendMessage(string message)
    {
        if (client != null) StartCoroutine(client.SendMessageAsync(message));
    }

    public override void SendReply(string messageId, string message)
    {
        if (client != null) StartCoroutine(client.SendReplyAsync(messageId, message));
    }

    private void UpdateMessages()
    {
        StartCoroutine(client.UpdateMessagesAsync());
    }

    private void OnMessage(DiscordAPI.Message message)
    {
        TryOnCommand(message);
    }

    private void TryOnCommand(DiscordAPI.Message message)
    {
        if (!message.content.StartsWith("!")) return;

        List<string> commandSplit = new List<string>(message.content.Substring(1).Split(' '));

        string command = commandSplit[0];
        List<string> arguments = new List<string>(commandSplit);
        arguments.RemoveAt(0);

        OnCommandInner(new OnCommandArgs {
            integration = this,
            messageId = message.id,
            username = message.author.username,
            command = command,
            arguments = arguments
        });
    }
}