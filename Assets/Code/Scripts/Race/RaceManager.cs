using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class RaceManager : AbstractManager<RaceManager>
{
    [SerializeField] private PowerUpConfig[] powerUpConfigs;
    public PowerUpConfig[] PowerUpConfigs => powerUpConfigs;

    private List<Ship> ships = new List<Ship>();
    public List<Ship> Ships => ships;
    public int Laps { get; set; }

    private int _currentLap = -1;
    public int currentLap { get => _currentLap; private set => _currentLap = value; }

    private Checkpoint[] checkpoints;
    public Checkpoint[] Checkpoints => checkpoints;
    public int LastCheckpointIndex => checkpoints.Length - 1;
    private float startTime;
    public float GameTime => Time.time - startTime;
    private bool started = false;
    public bool Started => started;

    public static Action OnRaceBegin;
    public static Action<int> OnNewLap;
    public static Action<string> OnRaceWon;
    public static Action OnRaceOver;

    private void Reset()
    {
        ships = new List<Ship>();
    }

    private void Start()
    {
        Reset();

        ShipRace.OnCheckpoint += OnCheckpoint;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        SceneManager.sceneLoaded += OnLocalSceneLoaded;
        ShipRace.OnLapFinish += OnLapFinish;
    }

    private void OnServerStarted()
    {
        this.Reset();
    }

    private void OnLocalSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Map"))
        {
            this.Laps = LobbyManager.Singleton.MapConfig.lapCount;
            LoadCheckpoints();
        }
    }

    private void LoadCheckpoints()
    {
        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpoints = new Checkpoint[checkpointObjects.Length];

        foreach(GameObject checkpointObject in checkpointObjects)
        {
            Checkpoint checkpoint = checkpointObject.GetComponent<Checkpoint>();
            checkpoints[checkpoint.Index] = checkpoint;
        }
    }

    public void OnRaceStart()
    {
        started = true;
        startTime = Time.time;
        OnRaceBegin();
    }

    public void OnLapFinish(int lap)
    {
        if (currentLap < lap)
        {
            currentLap = lap;

            OnNewLap(currentLap);

            if (currentLap >= Laps)
            {
                ships = ships.OrderBy(ship => -(ship.Race.LapCount.Value * checkpoints.Length + ship.Race.CheckpointIndex)).ToList();
                OnRaceWon(ships[0].Multiplayer.Lobby.Username);
            }
        }
        
    }

    public void AddShip(Ship ship)
    {
        ships.Add(ship);
    }

    public Checkpoint GetNextCheckpoint(int checkpointIndex)
    {
        /// TODO: Consider no checkpoints?
        if (checkpointIndex + 1 >= checkpoints.Length) return checkpoints[0];
        else return checkpoints[checkpointIndex + 1];
    }

    private bool IsRaceDone()
    {
        foreach (Ship ship in ships)
        {
            if (ship.Race.LapCount.Value < Laps) return false;
        }

        return true;
    }

    private void OnCheckpoint(Ship ship)
    {
        if(!IsServer) return;

        UpdatePlayerRankings();

        if (IsRaceDone())
        {
            started = false;

            OnRaceOver();
            
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            
            
            this.Reset();
        }
    }

    private void UpdatePlayerRankings()
    {
        ships = ships.OrderBy(ship => -(ship.Race.LapCount.Value * checkpoints.Length + ship.Race.CheckpointIndex)).ToList();

        for(int i = 0; i < ships.Count; i++)
        {
            ships[i].Race.Rank.Value = i + 1;
        }
    }
}
