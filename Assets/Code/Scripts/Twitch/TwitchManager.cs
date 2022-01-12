using UnityEngine;
using Unity.Netcode;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

/// <summary>
/// A Twitch integration using TwitchLib with Unity Netcode.
/// This code runs server-side only. Data should be passed to clients using ClientRpc.
/// </summary>
public class TwitchManager : NetworkBehaviour 
{
    private static readonly string CHANNEL_TO_CONNECT = "amouranth";
    private Client client;

    private void Start() 
    {
        BuildClient();
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        client.Connect();
    }

    private void BuildClient()
    {
        ConnectionCredentials credentials = new ConnectionCredentials("thimblebot", Secrets.BOT_ACCESS_TOKEN);

        client = new Client();
        client.Initialize(credentials, CHANNEL_TO_CONNECT);

        client.OnConnected += OnConnected;
        client.OnJoinedChannel += OnJoinedChannel;
        client.OnMessageReceived += OnMessageReceived;
        client.OnChatCommandReceived += OnChatCommandReceived;
    }

    [ClientRpc]
    private void MessageClientRpc(string message) 
    {
        // TODO: This is only a sample. 
        // You can use a similar function with a different signature to call to a client with other client data.
        Debug.Log($"Message from server: {message}");
    }

    private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
	{}

	private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
	{}

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log($"Message Received from {e.ChatMessage.Username}: {e.ChatMessage.Message}");

        if(e.ChatMessage.Message.Contains("ammo"))
        {
            MessageClientRpc(e.ChatMessage.Message);
        }
    }

    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{}
}
