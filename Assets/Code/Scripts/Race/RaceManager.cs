using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceManager : AbstractManager<RaceManager>
{
    [SerializeField] private List<Ship> ships = new List<Ship>();
    public List<Ship> Ships => ships;
    [SerializeField] [Range(2, 64)] private int lapCount = 2;
    public int Laps => lapCount;
    private int checkpointCount = 32;
    private Checkpoint[] checkpoints;
    public int LastCheckpointIndex => checkpoints.Length - 1;
    [Range(3, 200)] public int automaticCheckpointCount = 50;
    [Range(2, 10)] private int widthToThicknessRatio = 5; // This controls the x scale of the checkpoint relative to the thickness of the track.
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private TrackGenerator track;

    private void Start()
    {
        ShipRace.OnCheckpoint += OnCheckpoint;

        if (track != null)
        {
            SetTrackCheckpoints(track);
        }
        else
        {
            LoadCheckpoints();
        }
    }

    public void LoadCheckpoints()
    {
        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpoints = new Checkpoint[checkpointObjects.Length];

        foreach(GameObject checkpointObject in checkpointObjects)
        {
            Checkpoint checkpoint = checkpointObject.GetComponent<Checkpoint>();
            checkpoints[checkpoint.index] = checkpoint;
        }
    }

    public Checkpoint GetNextCheckpoint(int checkpointIndex)
    {
        /// TODO: Consider no checkpoints?
        if (checkpointIndex + 1 >= checkpoints.Length) return checkpoints[0];
        else return checkpoints[checkpointIndex + 1];
    }

    private void OnCheckpoint(Ship ship)
    {
        if(!IsMaster) return;
        UpdatePlayers();
    }

    private void UpdatePlayers()
    {
        ships = ships.OrderBy(ship => ship.Race.Lap * checkpoints.Length + ship.Race.CheckpointIndex).ToList();

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
            gameObject.transform.rotation = orientedPoint.rotation;

            checkpoints[i] = checkpoint;
        }
    }
}
