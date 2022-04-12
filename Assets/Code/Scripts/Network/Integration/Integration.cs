using System;
using UnityEngine;
using Unity.Netcode;

public abstract class Integration
{
    public struct OnConnectedArgs
    {
        public string name;
    }

    public struct OnCommandArgs
    {
        public string command;
    }

    public abstract void Connect();
    public abstract void Disconnect();

    public static Action<OnConnectedArgs> onConnected;
    protected void OnConnectedInner(OnConnectedArgs data)
    {
        if (onConnected != null) onConnected(data);
    }

    public static Action<OnCommandArgs> onCommand;
    protected void OnCommandInner(OnCommandArgs data)
    {
        if (onCommand != null) onCommand(data);
    }
}
