using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerManager : AbstractManager<ServerManager>
{
    [System.Serializable]
    public struct Config
    {
        public string id;
        public string name;
        public string host;
        public int port;
        public string password;
    }

    [HideInInspector]
    public Config config;

    protected void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = System.Text.Encoding.ASCII.GetString(connectionData);

        // TODO: Check timing attack.
        bool approve = true;
        if (!ServerManager.Singleton.config.password.Equals(password)) approve = false;

        string sceneName = SceneManager.GetActiveScene().name;
        if (!(sceneName.Equals("RaceMenu") || sceneName.Equals("Lobby"))) approve = false;

        callback(false, null, approve, Vector3.zero, Quaternion.identity);
    }

    private void SetConfig(Config config)
    {
        this.config = config;

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(config.password);
        Unity.Netcode.NetworkTransport transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        if (transport is Unity.Netcode.Transports.UNET.UNetTransport)
        {
            Unity.Netcode.Transports.UNET.UNetTransport unet = (Unity.Netcode.Transports.UNET.UNetTransport)transport;
            unet.ConnectAddress = config.host;
            unet.ConnectPort = config.port;
        }
        else
        {
            throw new System.Exception("Unhandled transport.");
        }
    }

    private void JoinLobby(Config config)
    {
        SetConfig(config);

        NetworkManager.Singleton.StartClient();
    }

    private void HostLobby(Config config)
    {
        SetConfig(config);

        NetworkManager.Singleton.StartHost();
    }

    public void JoinUnlistedServer(string host, string password)
    {
        if (host.Trim().Length == 0)
        {
            host = "127.0.0.1";
        }

        JoinLobby(new Config
        {
            name = "Unlisted",
            host = host,
            port = 7777,
            password = password
        });
    }

    public void HostUnlistedServer(string password)
    {
        HostLobby(new Config
        {
            name = "Unlisted",
            host = "127.0.0.1",
            port = 7777,
            password = password
        });
    }

    public void JoinServer(string id, string password)
    {
        if (RegistryManager.Singleton.IsConnected)
        {
            var data = new RegistryManager.GetServerData
            {
                id = id,
                password = password
            };

            RegistryManager.Singleton.GetServer(data,
                (RegistryManager.GetServerResponse response) =>
                {
                    JoinLobby(new Config
                    {
                        name = response.name,
                        host = response.host,
                        port = response.port,
                        password = password
                    });
                }
            );
        }
    }

    public void HostServer(string password, bool online)
    {
        if (RegistryManager.Singleton.IsConnected && online)
        {
            var data = new RegistryManager.InsertServerData
            {
                name = AuthManager.Singleton.Username,
                host = "127.0.0.1",
                port = 7777,
                password = password
            };

            RegistryManager.Singleton.InsertServer(data,
                (RegistryManager.InsertServerResponse response) =>
                {
                    AuthManager.Singleton.ServerToken = response.token;

                    HostLobby(new Config
                    {
                        id = response.id,
                        name = response.name,
                        host = response.host,
                        port = response.port,
                        password = password
                    });
                },
                () =>
                {
                    HostUnlistedServer(password);
                }
            );
        }
        else
        {
            HostUnlistedServer(password);
        }
    }

    public void UpdatePlayerCount(int count)
    {
        RegistryManager.Singleton.UpdateServerPlayerCount(config.id, count);
    }

    public void Disconnect()
    {
        RegistryManager.Singleton.DeleteServer(config.id);
    }
}
