using UnityEngine;
using Unity.Netcode;

public class IntegrationManager : AbstractManager<IntegrationManager>
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

    public void Connect()
    {
        Integration.onConnected += OnConnected;
        Integration.onCommand += OnCommand;

        foreach (Integration integration in INTEGRATIONS)
        {
            integration.Connect();
        }
    }

    public void Disconnect()
    {
        Integration.onConnected -= OnConnected;
        Integration.onCommand -= OnCommand;

        foreach (Integration integration in INTEGRATIONS)
        {
            integration.Disconnect();
        }
    }
}
