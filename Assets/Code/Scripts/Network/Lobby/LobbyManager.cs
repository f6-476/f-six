using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : AbstractManager<LobbyManager>
{
    public System.Action<LobbyPlayer> OnLocal;

    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spectatorPrefab;
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private MapConfig[] mapConfigs;
    private int maxPlayers = 8;
    public int MaxPlayers => maxPlayers;
    public int MapIndex { get; set; }
    public MapConfig MapConfig => mapConfigs[MapIndex];
    private LobbyPlayer localPlayer;
    public LobbyPlayer LocalPlayer 
    { 
        get => localPlayer;
        set 
        {
            localPlayer = value;
            if (OnLocal != null) OnLocal(localPlayer);
        }
    }
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
            if (IsInGame()) approve = false;
            if (clientMode == ClientMode.PLAYER && !HasSpaceForNewPlayer()) approve = false;

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
            spawns[spawn.Index] = spawn;
        }

        return spawns;
    }

    private void OnLoadEventComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer) return;

        if (sceneName.StartsWith("Map")) InitializeServerMap();
    }

    private void InitializeServerMap()
    {
        FillMissingSlotsWithAIs();

        Spawn[] spawns = GetSpawns();
        int spawnIndex = 0;

        foreach (LobbyPlayer player in Players)
        {
            switch (player.ClientMode)
            {
                case ClientMode.PLAYER:
                    player.Ready = false;
                    Spawn playerSpawn = spawns[spawnIndex++];
                    GameObject playerObject = Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
                    Ship playerShip = playerObject.GetComponent<Ship>();
                    playerShip.Multiplayer.Lobby.Value = player;
                    playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId, true);
                    break;
                case ClientMode.SPECTATOR:
                    player.Ready = false;
                    Spawn spectatorSpawn = spawns[spawns.Length - 1];
                    GameObject specatorObject = Instantiate(spectatorPrefab, spectatorSpawn.transform.position, spectatorSpawn.transform.rotation);
                    specatorObject.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId, true);
                    break;
                case ClientMode.AI:
                    Spawn aiSpawn = spawns[spawnIndex++];
                    GameObject aiObject = Instantiate(aiPrefab, aiSpawn.transform.position, aiSpawn.transform.rotation);
                    Ship aiShip = aiObject.GetComponent<Ship>();
                    aiShip.Multiplayer.Lobby.Value = player;
                    aiObject.GetComponent<NetworkObject>().Spawn(true);
                    break;
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

    private void AddClientLobbyPlayer(ulong clientId)
    {
        if (!IsServer) return;

        GameObject lobbyGameObject = Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity);

        lobbyGameObject.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        lobbyGameObject.GetComponent<LobbyPlayer>().ClientMode = clientIdModeDictionary[clientId];

        ServerManager.Singleton.UpdatePlayerCount(NetworkManager.Singleton.ConnectedClientsIds.Count);
    }

    private void AddAILobbyPlayer()
    {
        if (!IsServer) return;

        GameObject lobbyGameObject = Instantiate(lobbyPlayerPrefab, Vector3.zero, Quaternion.identity);

        lobbyGameObject.GetComponent<NetworkObject>().Spawn();
        LobbyPlayer lobbyPlayer = lobbyGameObject.GetComponent<LobbyPlayer>();
        lobbyPlayer.ClientMode = ClientMode.AI;
        lobbyPlayer.ready.Value = true;

        if (!this.Players.Contains(lobbyPlayer)) this.Players.Add(lobbyPlayer);
    }

    private void FillMissingSlotsWithAIs()
    {
        int playerCount = 0;

        foreach (LobbyPlayer player in Players)
        {
            if (player.ClientMode == ClientMode.PLAYER || player.ClientMode == ClientMode.AI) playerCount++;
        }

        for (int i = 0; i < maxPlayers - playerCount; i++)
        {
            AddAILobbyPlayer();
        }
    }

    private void OnClientConnect(ulong clientId)
    {
        if (!IsServer) return;

        AddClientLobbyPlayer(clientId);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId || clientId == 0)
        {
            SceneManager.LoadScene("RaceMenu", LoadSceneMode.Single);
        }
    }

    private bool IsAllPlayersReady()
    {
        bool oneHumanReady = false;

        foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
        {
            switch (player.ClientMode)
            {
                case ClientMode.PLAYER:
                    if (player.Ready) oneHumanReady = true;
                    else return false;
                    break;
                case ClientMode.SPECTATOR:
                    if (player.Ready) oneHumanReady = true;
                    break;
            }
        }

        return oneHumanReady;
    }

    private bool HasSpaceForNewPlayer()
    {
        LobbyPlayer aiPlayer = null;
        int playerCount = 0;

        foreach (LobbyPlayer player in Players)
        {
            if (aiPlayer == null && player.ClientMode == ClientMode.AI) aiPlayer = player;
            if (player.ClientMode == ClientMode.PLAYER || player.ClientMode == ClientMode.AI) playerCount++;
        }

        if (playerCount < maxPlayers) return true;
        if (aiPlayer != null)
        {
            aiPlayer.DestroyMe();
            return true;
        }

        return false;
    }

    private void LoadMapIfReady()
    {
        if (IsInGame()) return;
        if (!IsAllPlayersReady()) return;

        NetworkManager.Singleton.SceneManager.LoadScene(MapConfig.sceneName, LoadSceneMode.Single);
    }

    private bool IsInGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName.StartsWith("Map");
    }

    public void OnPlayerUpdate(LobbyPlayer updatedPlayer)
    {
        LoadMapIfReady();
    }

    public int NextMap()
    {
        MapIndex = (MapIndex + 1) % mapConfigs.Length;

        return MapIndex;
    }

    public int PreviousMap()
    {
        if (MapIndex == 0) MapIndex = mapConfigs.Length - 1;
        else MapIndex -= 1;

        return MapIndex;
    }

    public MapConfig[] GetMaps()
    {
        return mapConfigs;
    }
}
