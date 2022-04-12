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

    private bool SetConfig(Config config)
    {
        if (!ValidateIPv4(config.host)) return false;

        this.config = config;

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(config.password);
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

    private bool JoinLobby(Config config)
    {
        if (IsConnecting()) return false;

        if (!SetConfig(config)) return false;

        if(NetworkManager.Singleton.StartClient()) return true;

        NetworkManager.Singleton.Shutdown();

        return false;
    }

    private bool HostLobby(Config config)
    {
        if (IsConnecting()) return false;

        if (!SetConfig(config)) return false;

        if (NetworkManager.Singleton.StartHost()) return true;
 
        NetworkManager.Singleton.Shutdown();

        return false;
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
