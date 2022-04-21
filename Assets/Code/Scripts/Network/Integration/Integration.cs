using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Integration
{
    public struct OnConnectedArgs
    {
        public Integration integration;
    }

    public struct OnDisconnectedArgs
    {
        public Integration integration;
    }

    public struct OnCommandArgs
    {
        public Integration integration;
        public string messageId;
        public string username;
        public string command;
        public List<string> arguments;
    }

    public abstract void Connect();
    public abstract void Disconnect();
    public abstract void SendMessage(string message);
    public abstract void SendReply(string messageId, string message);

    public static Action<OnConnectedArgs> OnConnected;
    protected void OnConnectedInner(OnConnectedArgs data)
    {
        if (OnConnected != null) OnConnected(data);
    }

    public static Action<OnDisconnectedArgs> OnDisconnected;
    protected void OnDisconnectedInner(OnDisconnectedArgs data)
    {
        if (OnDisconnected != null) OnDisconnected(data);
    }

    public static Action<OnCommandArgs> OnCommand;
    protected void OnCommandInner(OnCommandArgs data)
    {
        if (OnCommand != null) OnCommand(data);
    }
}
