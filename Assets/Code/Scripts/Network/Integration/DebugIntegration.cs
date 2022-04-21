using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

public class DebugIntegration : Integration
{
    public override void Connect()
    {
        Debug.Log("[Debug] Connected");

        OnConnectedInner(new OnConnectedArgs {
            integration = this
        });
    }

    public override void Disconnect()
    {
        Debug.Log("[Debug] Disconnected");
    }

    public override void SendMessage(string message)
    {
        Debug.Log($"[Debug] {message}");
    }

    public override void SendReply(string messageId, string message)
    {
        Debug.Log($"[Debug] {messageId} - {message}");
    }
}
