using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class RaceManager : AbstractManager<RaceManager>
{
    private List<Ship> ships;
    public List<Ship> Ships => ships;
    public int Laps { get; set; }
    private int checkpointCount = 32;
    private HashSet<Checkpoint> checkpoints;
    public HashSet<Checkpoint> Checkpoints => this.checkpoints;
    public int LastCheckpoint => checkpoints.Count - 1;
    [Range(3, 200)] public int automaticCheckpointCount = 50;
    [Range(2, 10)] private int widthToThicknessRatio = 5; // This controls the x scale of the checkpoint relative to the thickness of the track.
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private TrackGenerator track;

    private void Reset()
    {
        ships = new List<Ship>();
        checkpoints = new HashSet<Checkpoint>();
    }

    private void Start()
    {
        Reset();

        ShipRace.OnCheckpoint += OnCheckpoint;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        this.Reset();
    }

    public void LoadCheckpoints()
    {
        if (track != null)
        {
            SetTrackCheckpoints(track);
        }
        else
        {
            foreach(GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
            {
                checkpoints.Add(checkpoint.GetComponent<Checkpoint>());
            }
        }
    }

    public void AddShip(Ship ship)
    {
        ships.Add(ship);
    }

    private void OnCheckpoint(Ship ship)
    {
        if(!IsServer) return;
        UpdatePlayerRankings();
    }

    private void UpdatePlayerRankings()
    {
        ships = ships.OrderBy(ship => -(ship.Race.Lap * checkpoints.Count + ship.Race.Checkpoint)).ToList();

        for(int i = 0; i < ships.Count; i++)
        {
            ships[i].Race.Rank = i + 1;
        }
    }

    private void SetTrackCheckpoints(TrackGenerator track)
    {
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

            checkpoints.Add(checkpoint);
        }
    }
}
