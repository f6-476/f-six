using UnityEngine;
using Unity.Netcode;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

public class TwitchIntegration : Integration
{
    private Client client;

    public override void Connect()
    {
        ConnectionCredentials credentials = new ConnectionCredentials("thimblebot", Secrets.Twitch.BOT_ACCESS_TOKEN);

        client = new Client();
        client.Initialize(credentials, Secrets.Twitch.CHANNEL_TO_CONNECT);

        client.OnJoinedChannel += OnJoinedChannel;
        client.OnMessageReceived += OnMessageReceived;
        client.OnChatCommandReceived += OnChatCommandReceived;
    }

    public override void Disconnect()
    {
        client.Disconnect();
    }

    private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
        OnConnectedInner(new OnConnectedArgs {
            name="Twitch"
        });
    }

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {}

    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{}
}