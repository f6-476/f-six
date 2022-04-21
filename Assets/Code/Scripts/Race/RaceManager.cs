using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class RaceManager : AbstractManager<RaceManager>
{
    private List<Ship> ships = new List<Ship>();
    public List<Ship> Ships => ships;
    public int Laps { get; set; }
    private int checkpointCount = 32;
    private Checkpoint[] checkpoints;
    public Checkpoint[] Checkpoints => checkpoints;
    public int LastCheckpointIndex => checkpoints.Length - 1;
    [Range(3, 200)] public int automaticCheckpointCount = 50;
    [Range(2, 10)] private int widthToThicknessRatio = 5; // This controls the x scale of the checkpoint relative to the thickness of the track.
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private TrackGenerator track;
    private float startTime;
    public float GameTime => Time.time - startTime;
    private bool started = false;
    public bool Started => started;

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
        if (track != null)
        {
            SetTrackCheckpoints(track);
        }
        else
        {
            GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
            checkpoints = new Checkpoint[checkpointObjects.Length];

            foreach(GameObject checkpointObject in checkpointObjects)
            {
                Checkpoint checkpoint = checkpointObject.GetComponent<Checkpoint>();
                checkpoints[checkpoint.index] = checkpoint;
            }
        }
    }

    public void OnRaceStart()
    {
        started = true;
        startTime = Time.time;
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
            if (ship.Race.LapCount < Laps) return false;
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
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            this.Reset();
        }
    }

    private void UpdatePlayerRankings()
    {
        ships = ships.OrderBy(ship => -(ship.Race.LapCount * checkpoints.Length + ship.Race.CheckpointIndex)).ToList();

        for(int i = 0; i < ships.Count; i++)
        {
            ships[i].Race.Rank = i + 1;
        }
    }

    private void SetTrackCheckpoints(TrackGenerator track)
    {
        checkpoints = new Checkpoint[automaticCheckpointCount];
        for (int i = 0; i < automaticCheckpointCount; i++)
        {
            GameObject gameObject = Instantiate(checkpointPrefab, Vector3.zero, Quaternion.identity);
            gameObject.transform.localScale = new Vector3(track.thickness * widthToThicknessRatio, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            gameObject.transform.SetParent(track.transform);

            Checkpoint checkpoint = gameObject.GetComponent<Checkpoint>();
            checkpoint.index = i;

            float trackOffset = i / (float) automaticCheckpointCount;
            OrientedPoint orientedPoint = track.segment.GetOrientedPoint(trackOffset);
 
            float angle = Mathf.PI * -1.5f;
            Vector3 projectedPoint = orientedPoint.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness) - Vector3.up * (track.thickness / 2);
            gameObject.transform.position = projectedPoint;
            gameObject.transform.rotation = Quaternion.Euler(0, orientedPoint.rotation.eulerAngles.y, orientedPoint.rotation.eulerAngles.z);

            checkpoints[i] = checkpoint;
        }
    }
}
