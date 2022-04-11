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

    private static IntegrationManager instance;
    public static IntegrationManager Singleton
    {
        get => instance;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
