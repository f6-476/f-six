using UnityEngine;
using Unity.Netcode;

public class IntegrationManager : NetworkBehaviour 
{
    private static readonly Integration[] INTEGRATIONS = new Integration[]
    {
        new TwitchIntegration()
    };

    private void OnConnected(Integration.OnConnectedArgs args)
    {
        
    }

    private void OnCommand(Integration.OnCommandArgs args)
    {

    }

    private void Start()
    {
        if (IsHost || IsServer)
        {
            Integration.onConnected += OnConnected;
            Integration.onCommand += OnCommand;

            foreach(Integration integration in INTEGRATIONS)
            {
                integration.Connect();
            }
        }
    }

    public override void OnDestroy() 
    {
        if (IsHost || IsServer)
        {
            Integration.onConnected -= OnConnected;
            Integration.onCommand -= OnCommand;

            foreach(Integration integration in INTEGRATIONS)
            {
                integration.Disconnect();
            }
        }

        base.OnDestroy();
    }
}
