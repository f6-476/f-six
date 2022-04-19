using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : AbstractManager<LobbyManager>
{
    [SerializeField] private GameObject lobbyPlayerPrefab;

    [SerializeField] private GameObject gamePlayerPrefab;
    [SerializeField] private MapConfig[] mapConfigs;
    private int mapIndex = 0;
    public MapConfig MapConfig => mapConfigs[mapIndex];

    public LobbyPlayer LocalPlayer;

    public HashSet<LobbyPlayer> Players { get; set; }

    private void Reset()
    {
        Players = new HashSet<LobbyPlayer>();
    }

    protected void Start()
    {
        this.Reset();

        SceneManager.sceneLoaded += OnLocalSceneLoaded;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnServerStarted()
    {
        this.Reset();

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventComplete;
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
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
                Spawn spawn = spawns[spawnIndex++];
                GameObject playerObject = Instantiate(gamePlayerPrefab, spawn.transform.position, spawn.transform.rotation);
                Ship ship = playerObject.GetComponent<Ship>();
                ship.Multiplayer.Lobby = player;
                playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId, true);
                RaceManager.Singleton.AddShip(ship);
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
        foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
        {
            if (!player.Ready) return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(MapConfig.sceneName, LoadSceneMode.Single);
    }

    public void OnPlayerUpdate(LobbyPlayer updatedPlayer)
    {
        LoadMapIfReady();
    }
}
