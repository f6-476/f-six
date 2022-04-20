using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Ship))]
public class ShipRace : NetworkBehaviour
{
    public static Action<Ship> OnCheckpoint;
    private Ship ship;
    [SerializeField] private int checkpointIndex = 0;
    public int CheckpointIndex => checkpointIndex;
    private List<float> lapTimeList = new List<float>();
    private int lapCount = 0;
    public int Lap => lapCount;
    public bool Finished => lapCount >= RaceManager.Singleton.Laps;
    private float lapTime = 0;
    public float LapTime => lapTime;
    public int Rank { get; set; }

    public Checkpoint nextCheckpoint;

    private void Start()
    {
        this.Rank = 1;
        this.ship = GetComponent<Ship>();
    }

    /// Gets the time difference between the previous lap and the best lap.
    public float GetLapDifference()
    {
        if (lapTimeList.Count == 0) return 0;

        float minTime = lapTimeList[0];
        for (int i = 1; i < lapTimeList.Count - 1; i++)
        {
            float time = lapTimeList[i];
            if (time < minTime)
            {
                minTime = time;
            }
        }

        return lapTimeList[lapTimeList.Count - 1] - minTime;
    }

    public void ResetCheckpointIndex()
    {
        checkpointIndex = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: Check for online/offline.
        // if (!IsOwner) return;

        if (other.TryGetComponent(out Checkpoint checkpoint))
        {
            if (checkpoint.index == 0 && checkpointIndex == RaceManager.Singleton.LastCheckpointIndex)
            {
                lapCount++;
                checkpointIndex = 0;

                lapTimeList.Add(Time.time - lapTime);
                lapTime = Time.time;
                if(TryGetComponent<ShipAgent>(out ShipAgent shipAgent))
                    shipAgent.AddReward(5);
            } 
            else if (checkpointIndex + 1 == checkpoint.index)
            {
                if(TryGetComponent<ShipAgent>(out ShipAgent shipAgent))
                    shipAgent.AddReward(.5f);
                checkpointIndex = checkpoint.index;
                
            }
            else
            {
                if(TryGetComponent<ShipAgent>(out ShipAgent shipAgent))
                    shipAgent.SetReward(-1);
            }

            if(OnCheckpoint != null) OnCheckpoint(ship);
        }
    }
    
    

    public Checkpoint GetNextCheckpoint()
    {
        nextCheckpoint = RaceManager.Singleton.GetNextCheckpoint(checkpointIndex);
        return nextCheckpoint;
    }

    public void OnDrawGizmos()
    {
        if(nextCheckpoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, nextCheckpoint.transform.position);
        }
    }
}
