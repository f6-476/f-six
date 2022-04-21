using UnityEngine;
using System.Collections.Generic;

public class TestIntegration : Integration
{
    private bool isActive = false;
    private float lastCommandTime;

    public override void Connect()
    {
        isActive = true;
        
        Debug.Log("[Test] Connected");

        OnConnectedInner(new OnConnectedArgs {
            integration = this
        });
    }

    public override void Disconnect()
    {
        isActive = false;

        Debug.Log("[Test] Disconnected");

        OnDisconnectedInner(new OnDisconnectedArgs {
            integration = this
        });
    }

    public override void Update()
    {
        if (Time.time - lastCommandTime > 10.0f)
        {
            lastCommandTime = Time.time;

            OnCommandInner(new OnCommandArgs {
                integration = this,
                messageId = "test",
                username = "TestUser",
                command = "help",
                arguments = new List<string>()
            });
        }
    }

    public override void SendMessage(string message)
    {
        Debug.Log($"[Test] {message}");
    }

    public override void SendReply(string messageId, string message)
    {
        Debug.Log($"[Test] {messageId} -> {message}");
    }
}
