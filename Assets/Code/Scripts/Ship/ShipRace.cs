using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Ship))]
public class ShipRace : MonoBehaviour
{
    [SerializeField] private Ship ship;
    public static Action<Ship> OnCheckpoint;
    private int checkpointIndex = 0;
    public int Checkpoint => checkpointIndex;
    private List<float> lapTimeList = new List<float>();
    private int lapCount = 0;
    public int Lap => lapCount;
    public bool Finished => lapCount >= RaceManager.Singleton.Laps;
    private float lapTime = 0;
    public float LapTime => lapTime;
    private int rank = 1;
    public int Rank 
    {
        get => (ship.IsMultiplayer) ? ship.Multiplayer.Rank : rank;
        set
        {
            if (ship.IsMultiplayer) ship.Multiplayer.Rank = value;
            else rank = value;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (!ship.IsServer) return;
        if (RaceManager.Singleton == null) return;

        if (other.TryGetComponent(out Checkpoint checkpoint))
        {
            if (checkpoint.index == 0 && checkpointIndex == RaceManager.Singleton.LastCheckpoint)
            {
                lapCount++;
                checkpointIndex = 0;

                lapTimeList.Add(Time.time - lapTime);
                lapTime = Time.time;
            } 
            else if (checkpointIndex + 1 == checkpoint.index)
            {
                checkpointIndex = checkpoint.index;
            }

            if(OnCheckpoint != null) OnCheckpoint(ship);
        }
    }
}
