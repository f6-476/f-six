using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipInfo : MonoBehaviour
{
    public List<Transform> CurrentCheckpoints = new List<Transform>();
    [SerializeField] private List<float> _lapTimeList = new List<float>();
    [SerializeField] private ShipView _shipView;
    public int LapsCompleted { get; set; }
    public int CurrentRank { get; set; }

    private void Start()
    {
        LapsCompleted = 1;
        _shipView.SetLapText(LapsCompleted);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If checkpoint has not been reached once, add it to the list of checkpoints reached
        if (other.gameObject.CompareTag("Checkpoint") && !CurrentCheckpoints.Contains(other.transform))
        {
            CurrentCheckpoints.Add(other.transform);
        }

        // If lap finished, clear all checkpoints reached and increment number of laps
        if (HasFinishedLap() && other.gameObject == CheckpointManager.Instance.finishLine)
        {
            LapsCompleted++;
            CurrentCheckpoints.Clear();
            _shipView.SetLapText(LapsCompleted);
        }
        
        // Update Rankings if necessary
        GameManager.Instance.RankPlayers();
    }

    /// <summary>
    /// Check if all the checkpoints have been reached by the player
    /// </summary>
    /// <returns></returns>
    private bool HasFinishedLap()
    {
        return CurrentCheckpoints.Count > 0 && CurrentCheckpoints.All(c => CheckpointManager.Instance.checkpoints.ToList().Contains(c.gameObject));
    }
}
