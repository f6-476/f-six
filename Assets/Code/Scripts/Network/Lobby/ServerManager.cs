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

    private bool ValidateIPv4(string ipString)
    {
        if (ipString == null || ipString.Trim().Length == 0)
        {
            return false;
        }
 
        string[] sections = ipString.Split('.');
        if (sections.Length != 4)
        {
            return false;
        }

        foreach (string section in sections)
        {
            if (!byte.TryParse(section, out byte _))
            {
                return false;
            }
        }

        return true;
    }

    private bool SetConfig(Config config, ClientMode clientMode=ClientMode.PLAYER)
    {
        if (!ValidateIPv4(config.host)) return false;

        this.config = config;

        NetworkManager.Singleton.NetworkConfig.ConnectionData = ConnectionData.Write(config, clientMode);
        Unity.Netcode.NetworkTransport transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        if (transport is Unity.Netcode.Transports.UNET.UNetTransport)
        {
            Unity.Netcode.Transports.UNET.UNetTransport unet = (Unity.Netcode.Transports.UNET.UNetTransport)transport;
            unet.ConnectAddress = config.host;
            unet.ConnectPort = config.port;
        }
        else if (transport is Unity.Netcode.Transports.UTP.UnityTransport)
        {
            Unity.Netcode.Transports.UTP.UnityTransport utp = (Unity.Netcode.Transports.UTP.UnityTransport)transport;
            utp.ConnectionData.Address = config.host;
            utp.ConnectionData.Port = (ushort)config.port;
        }
        else
        {
            throw new System.Exception("Unhandled transport.");
        }

        return true;
    }

    private bool IsConnecting()
    {
        return NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer;
    }

    private bool JoinLobby(Config config, ClientMode clientMode=ClientMode.PLAYER)
    {
        if (IsConnecting()) return false;

        if (!SetConfig(config, clientMode)) return false;

        if(NetworkManager.Singleton.StartClient()) return true;

        NetworkManager.Singleton.Shutdown();

        return false;
    }

    private bool HostLobby(Config config, ClientMode clientMode=ClientMode.PLAYER)
    {
        if (IsConnecting()) return false;

        if (!SetConfig(config, clientMode)) return false;

        if (NetworkManager.Singleton.StartHost()) {
            IntegrationManager.Singleton.Connect();

            return true;
        }
 
        NetworkManager.Singleton.Shutdown();

        return false;
    }

    public void JoinUnlistedServer(string host, string password, ClientMode clientMode=ClientMode.PLAYER)
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
        }, clientMode);
    }

    public void HostUnlistedServer(string password, ClientMode clientMode=ClientMode.PLAYER)
    {
        HostLobby(new Config
        {
            name = "Unlisted",
            host = "0.0.0.0",
            port = 7777,
            password = password
        }, clientMode);
    }

    public void JoinServer(string id, string password, ClientMode clientMode=ClientMode.PLAYER)
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
                    }, clientMode);
                }
            );
        }
    }

    public void HostServer(string password, bool online, ClientMode clientMode=ClientMode.PLAYER)
    {
        if (RegistryManager.Singleton.IsConnected && online)
        {
            var data = new RegistryManager.InsertServerData
            {
                name = AuthManager.Singleton.Username,
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
                    }, clientMode);
                },
                () =>
                {
                    HostUnlistedServer(password, clientMode);
                }
            );
        }
        else
        {
            HostUnlistedServer(password, clientMode);
        }
    }

    public void UpdatePlayerCount(int count)
    {
        RegistryManager.Singleton.UpdateServerPlayerCount(config.id, count);
    }

    public void Disconnect()
    {
        IntegrationManager.Singleton.Disconnect();
        RegistryManager.Singleton.DeleteServer(config.id);
    }
}
