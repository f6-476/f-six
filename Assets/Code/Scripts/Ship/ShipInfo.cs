using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipInfo : MonoBehaviour
{
    [SerializeField] private ShipView _shipView;
    public List<Transform> CurrentCheckpoints = new List<Transform>();
    [SerializeField] private List<float> _lapTimeList = new List<float>();
    public int LapsCompleted { get; set; }
    public int CurrentRank { get; set; }

    public float _stopWatch;
    public bool _isCounting;
    public bool HasFinished { get; set; }

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
        HasFinished = HasFinishedRace();
        if (HasFinished)
        {
            _isCounting = false;
            ResetTimer();
            var ship = GetComponent<Ship>();
            GameManager.Instance._playersShips.Remove(ship);
            GameManager.Instance._scoredShips.Add(ship);
            return;
        }
        
        // If checkpoint has not been reached once, add it to the list of checkpoints reached
        if (other.gameObject.CompareTag("Checkpoint") || other.gameObject.CompareTag("FinishLine") && !CurrentCheckpoints.Contains(other.transform))
        {
            CurrentCheckpoints.Add(other.transform);
        }

        if (other.gameObject == CheckpointManager.Instance.finishLine)
        {
            ResetTimer();
            _isCounting = true;
        }

        // Update Rankings if necessary
        GameManager.Instance.RankPlayers();
        _shipView.SetRankText(CurrentRank);
        _shipView.SetRankingsText();
    }

    private void TickStopWatch()
    {
        if (_isCounting && !HasFinished)
        {
            _stopWatch += Time.deltaTime;
        }
        else
        {
            _stopWatch = 0f;
        }
        _shipView.SetStopwatchText(_stopWatch);
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

        if (LapsCompleted < GameManager.Instance.MapLaps && _isCounting)
        {
            LapsCompleted++;
            _shipView.SetLapText(LapsCompleted);
        }

        var lapTimeDifference = _lapTimeList.Last() - _stopWatch;
        if (LapsCompleted > 1)
        {
            lapTimeDifference = _stopWatch - _lapTimeList.Last();
        }
        _shipView.SetLapTimeText(lapTimeDifference);
        _lapTimeList.Add(lapTimeDifference);

        _stopWatch = 0.0f;
    }

    private bool HasFinishedRace()
    {
        return LapsCompleted > GameManager.Instance.MapLaps;
    }
}
