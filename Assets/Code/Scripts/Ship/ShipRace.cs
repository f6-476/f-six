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
    public int CheckpointIndex => checkpointIndex;
    private List<float> lapTimeList = new List<float>();
    public SyncVariable<int> LapCount = new SyncVariable<int>(0);
    public bool Finished => LapCount.Value >= RaceManager.Singleton.Laps;
    public SyncVariable<int> Rank = new SyncVariable<int>(1);

    public static Action<int> OnLapFinish;

    /// Gets the time difference between the previous lap and the best lap.
    public float GetLapDifference()
    {
        List<float> lapTimeList;
        if (ship.IsMultiplayer) 
        {
            lapTimeList = new List<float>();
            foreach(float time in ship.Multiplayer.LapTimeList) lapTimeList.Add(time);
        }
        else lapTimeList = this.lapTimeList;

        if (lapTimeList.Count < 2) return 0;

        float bestTime = lapTimeList[0];
        for (int i = 1; i < lapTimeList.Count - 1; i++)
        {
            float lapTime = lapTimeList[i] - lapTimeList[i - 1];

            if (lapTime < bestTime) bestTime = lapTime;
        }

        float lastLapTime = lapTimeList[lapTimeList.Count - 1] - lapTimeList[lapTimeList.Count - 2];
        return lastLapTime - bestTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ship.IsServer) return;
        if (RaceManager.Singleton == null) return;

        if (other.TryGetComponent(out Checkpoint checkpoint))
        {
            if (checkpoint.Index == 0 && checkpointIndex == RaceManager.Singleton.LastCheckpointIndex)
            {
                LapCount.Value++;
                OnLapFinish(LapCount.Value);
                
                checkpointIndex = 0;

                if (ship.IsMultiplayer) ship.Multiplayer.LapTimeList.Add(RaceManager.Singleton.GameTime);
                else lapTimeList.Add(RaceManager.Singleton.GameTime);
            } 
            else if (checkpointIndex + 1 == checkpoint.Index)
            {
                checkpointIndex = checkpoint.Index;
            }

            if(OnCheckpoint != null) OnCheckpoint(ship);
        }
    }

    public Checkpoint GetNextCheckpoint()
    {
        return RaceManager.Singleton.GetNextCheckpoint(checkpointIndex);
    }
}
