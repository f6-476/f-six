using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : AbstractManager<LobbyManager>
{   
    [SerializeField] private GameObject lobbyPlayerPrefab;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spectatorPrefab;
    [SerializeField] private MapConfig[] mapConfigs;
    private int mapIndex = 0;
    public MapConfig MapConfig => mapConfigs[mapIndex];

    public LobbyPlayer LocalPlayer;

    public HashSet<LobbyPlayer> Players { get; set; }
    private Dictionary<ulong, ClientMode> clientIdModeDictionary;

    private void Reset()
    {
        Players = new HashSet<LobbyPlayer>();
        clientIdModeDictionary = new Dictionary<ulong, ClientMode>();
    }

    protected void Start()
    {
        this.Reset();

        SceneManager.sceneLoaded += OnLocalSceneLoaded;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void OnServerStarted()
    {
        this.Reset();

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventComplete;
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        bool approve = true;
        if (ConnectionData.TryRead(connectionData, out string password, out ClientMode clientMode))
        {
            // TODO: Prevent timing attack?
            if (!ServerManager.Singleton.config.password.Equals(password)) approve = false;

            string sceneName = SceneManager.GetActiveScene().name;
            if (!(sceneName.Equals("RaceMenu") || sceneName.Equals("Lobby"))) approve = false;

            if (approve)
            {
                clientIdModeDictionary[clientId] = clientMode;
            }
        }
        else
        {
            approve = false;
        }

        callback(false, null, approve, Vector3.zero, Quaternion.identity);
    }

    private Spawn[] GetSpawns()
    {
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("Spawn");
        Spawn[] spawns = new Spawn[spawnObjects.Length];

        foreach (GameObject spawnObject in spawnObjects)
        {
            Spawn spawn = spawnObject.GetComponent<Spawn>();
            spawns[spawn.index] = spawn;
        }

        return spawns;
    }

    private void OnLocalSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Map"))
        {
            RaceManager.Singleton.Laps = MapConfig.lapCount;
            RaceManager.Singleton.OnGameStarted();
        }
    }

    private void OnLoadEventComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName.StartsWith("Map"))
        {
            /// TODO: What to do when spawn count < player count?
            Spawn[] spawns = GetSpawns();
            int spawnIndex = 0;

            foreach (LobbyPlayer player in Players)
            {
                player.Ready = false;

                switch(player.ClientMode)
                {
                    case ClientMode.PLAYER:
                        Spawn playerSpawn = spawns[spawnIndex++];
                        GameObject playerObject = Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
                        Ship ship = playerObject.GetComponent<Ship>();
                        ship.Multiplayer.Lobby = player;
                        playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId, true);
                        break;
                    case ClientMode.SPECTATOR:
                        Spawn spectatorSpawn = spawns[spawns.Length - 1];
                        GameObject specatorObject = Instantiate(spectatorPrefab, spectatorSpawn.transform.position, spectatorSpawn.transform.rotation);
                        specatorObject.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId, true);
                        break;
                }
            }
        }
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();

        if (IsServer)
        {
            ServerManager.Singleton.Disconnect();
        }

        SceneManager.LoadScene("RaceMenu", LoadSceneMode.Single);
    }

    private void OnClientConnect(ulong clientId)
    {
        if (!IsServer) return;

        GameObject player = Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity);

        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        player.GetComponent<LobbyPlayer>().ClientMode = clientIdModeDictionary[clientId];

        ServerManager.Singleton.UpdatePlayerCount(NetworkManager.Singleton.ConnectedClientsIds.Count);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId || clientId == 0)
        {
            SceneManager.LoadScene("RaceMenu", LoadSceneMode.Single);
        }
    }

    private void LoadMapIfReady()
    {
        bool hasPlayers = false;

        foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
        {
            switch(player.ClientMode)
            {
                case ClientMode.PLAYER:
                    if (!player.Ready) return;
                    hasPlayers = true;
                    break;
            }
        }

        if (!hasPlayers) return;

        NetworkManager.Singleton.SceneManager.LoadScene(MapConfig.sceneName, LoadSceneMode.Single);
    }

    public void OnPlayerUpdate(LobbyPlayer updatedPlayer)
    {
        LoadMapIfReady();
    }
}
