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
    [SerializeField] [Range(2, 64)] private int checkpointCount = 2;
    private HashSet<Checkpoint> checkpoints = new HashSet<Checkpoint>();
    public HashSet<Checkpoint> Checkpoints => this.checkpoints;
    public int LastCheckpoint => checkpointCount - 1;
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
    }

    private void OnCheckpoint(Ship ship)
    {
        if(!IsMaster) return;
        UpdatePlayers();
    }

    private void UpdatePlayers()
    {
        ships = ships.OrderBy(ship => ship.Race.Lap * checkpointCount + ship.Race.Checkpoint).ToList();

        for(int i = 0; i < ships.Count; i++)
        {
            ships[i].Race.Rank = i + 1;
        }
    }

    private void SetTrackCheckpoints(TrackGenerator track)
    {
        for (int i = 0; i < checkpointCount; i++)
        {
            GameObject gameObject = Instantiate(checkpointPrefab, Vector3.zero, Quaternion.identity);
            gameObject.transform.localScale = new Vector3(track.thickness * widthToThicknessRatio, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            gameObject.transform.SetParent(track.transform);

            Checkpoint checkpoint = gameObject.GetComponent<Checkpoint>();
            checkpoint.index = i;

            float trackOffset = i / (float) checkpointCount;
            OrientedPoint orientedPoint = track.segment.GetOrientedPoint(trackOffset);
 
            float angle = Mathf.PI * -1.5f;
            Vector3 projectedPoint = orientedPoint.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness) - Vector3.up * (track.thickness / 2);
            gameObject.transform.position = projectedPoint;
            gameObject.transform.rotation = Quaternion.Euler(0, orientedPoint.rotation.eulerAngles.y, orientedPoint.rotation.eulerAngles.z);

            checkpoints.Add(checkpoint);
        }
    }
}
