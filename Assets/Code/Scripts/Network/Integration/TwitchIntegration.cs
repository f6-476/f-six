using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

public class TwitchIntegration : Integration
{
    private Client client;
    private string channel;

    public override void Connect()
    {
        client = null;

        if (AuthManager.Singleton.TwitchChannel.Trim() == "") return;

        ConnectionCredentials credentials = new ConnectionCredentials("thimblebot", Secrets.Twitch.BOT_ACCESS_TOKEN);

        client = new Client();

        client.Initialize(credentials, AuthManager.Singleton.TwitchChannel);

        client.OnError += OnError;
        client.OnJoinedChannel += OnJoinedChannel;
        client.OnMessageReceived += OnMessageReceived;
        client.OnChatCommandReceived += OnChatCommandReceived;
        client.OnConnected += OnServerConnected;
        client.OnDisconnected += OnServerDisconnected;

        client.Connect();
    }

    public override void Disconnect()
    {
        if(client != null) client.Disconnect();
    }

    public override void SendMessage(string message)
    {
        if (client != null) client.SendMessage(channel, message);
    }

    public override void SendReply(string messageId, string message)
    {
        if (client != null) client.SendReply(channel, messageId, message);
    }

    private void OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
    {
        Debug.Log($"Error: {e}");
    }

    private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
        Debug.Log("Joined Channel");

        channel = e.Channel;

        OnConnectedInner(new OnConnectedArgs {
            integration=this
        });
    }

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log($"Message {e.ChatMessage.Username}: {e.ChatMessage.Message}");
    }

    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{
        Debug.Log($"Chat Command {e.Command.ChatMessage.Username} {e.Command.CommandText} {e.Command.ArgumentsAsString}");

        OnCommandInner(new OnCommandArgs {
            integration = this,
            username = e.Command.ChatMessage.Username,
            messageId = e.Command.ChatMessage.Id,
            command = e.Command.CommandText,
            arguments = e.Command.ArgumentsAsList
        });
    }

    private void OnServerConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
        Debug.Log($"OnConnected: {e}");
    }

    private void OnServerDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
    {
        OnDisconnectedInner(new OnDisconnectedArgs {
            integration = this
        });
    }
}