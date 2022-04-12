using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipInfo : MonoBehaviour
{
    public List<Transform> CurrentCheckpoints = new List<Transform>();
    [SerializeField] private List<float> _lapTimeList = new List<float>();
    public int LapsCompleted { get; set; }
    public int CurrentRank { get; set; }

    private float _stopWatch;
    private bool _isCounting;

    private void Start()
    {
        LapsCompleted = 1;
        _lapTimeList.Add(0);
    }

    private void Update()
    {
        TickStopWatch();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasFinishedRace()) return;
        
        // If checkpoint has not been reached once, add it to the list of checkpoints reached
        if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("FinishLine") && !CurrentCheckpoints.Contains(other.transform))
        {
            CurrentCheckpoints.Add(other.transform);
        }

        if (other.gameObject == CheckpointManager.Instance.finishLine)
        {
            _isCounting = true;
            if (HasFinishedLap())
            {
                ResetTimer();
            }
        }

        // Update Rankings if necessary
        GameManager.Instance.RankPlayers();
    }

    private void TickStopWatch()
    {
        if (_isCounting && !HasFinishedRace())
        {
            _stopWatch += Time.deltaTime;
        }
        else
        {
            _stopWatch = 0f;
        }
    }

    /// <summary>
    /// Check if all the checkpoints have been reached by the player
    /// </summary>
    /// <returns></returns>
    private bool HasFinishedLap()
    {
        return CurrentCheckpoints.Count == CheckpointManager.Instance.checkpoints.Length && 
               CurrentCheckpoints.All(c => CheckpointManager.Instance.checkpoints.ToList().Contains(c.gameObject));
    }

    private void ResetTimer()
    {
        CurrentCheckpoints.Clear();

        if (LapsCompleted < GameManager.Instance.MapLaps)
        {
            LapsCompleted++;
        }

        var lapTimeDifference = _lapTimeList.Last() - _stopWatch;
        if (LapsCompleted > 1)
        {
            lapTimeDifference = _stopWatch - _lapTimeList.Last();
        }
        _lapTimeList.Add(lapTimeDifference);

        _stopWatch = 0.0f;
    }

    private bool HasFinishedRace()
    {
        return LapsCompleted > GameManager.Instance.MapLaps;
    }
}
