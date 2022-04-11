using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : AbstractManager<LobbyManager>
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    private Config? config = null;
    public NetworkList<LobbyPlayer.Raw> players;
    public LobbyPlayer localPlayer = null;

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnServerStarted += OnServerConnected;
    }

    public struct Config 
    {
        public string id;
        public string name;
        public string host;
        public int port;
        public string password;
    }

    private void OnServerConnected()
    {
        if (!(IsServer || IsHost)) return;

        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!(IsServer || IsHost)) return;
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        if (sceneName == "Lobby")
        {
            OnClientConnect(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnClientConnect(ulong clientId)
    {
        if (!(IsServer || IsHost)) return;

        GameObject player = Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);

        int count = 1;
        if (players != null) count = players.Count;

        StartCoroutine(ServerManager.Singleton.UpdatePlayerCount(config.Value.id, NetworkManager.Singleton.ConnectedClientsIds.Count));
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (!(IsServer || IsHost)) return;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id == clientId)
            {
                players.RemoveAt(i);

                StartCoroutine(ServerManager.Singleton.UpdatePlayerCount(config.Value.id, NetworkManager.Singleton.ConnectedClientsIds.Count));

                break;
            }
        }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = connectionData.ToString();
        // TODO: Check timing attack.
        bool approve = this.config.HasValue && this.config.Value.password.Equals(password);

        callback(false, null, approve, Vector3.zero, Quaternion.identity);
    }

    private void SetConfig(Config config)
    {
        this.config = config;

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

    public void HostLobby(string password)
    {
        var serverConfig = ServerManager.Singleton.GetConfig(password);
        Config config = new Config {
            id = "0",
            name = serverConfig.name,
            host = serverConfig.host,
            port = serverConfig.port,
            password = password
        };
        HostLobby(config);
    }

    public void HostLobby(Config config)
    {
        SetConfig(config);

        NetworkManager.Singleton.StartHost();
    }

    public bool JoinLobby(Config config)
    {
        SetConfig(config);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(config.password);
        if (!NetworkManager.Singleton.StartClient()) return false;

        return true;
    }
}
