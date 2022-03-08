using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipInfo : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    public List<Transform> CurrentCheckpoints = new List<Transform>();
    public int LapsCompleted { get; set; }
    public int CurrentRank { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        // If checkpoint has not been reached once, add it to the list of checkpoints reached
        if (other.gameObject.CompareTag("Checkpoint") && !CurrentCheckpoints.Contains(other.transform))
        {
            CurrentCheckpoints.Add(other.transform);
        }

        // If lap finished, clear all checkpoints reached and increment number of laps
        if (HasFinishedLap())
        {
            LapsCompleted++;
            CurrentCheckpoints.Clear();
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
        return CurrentCheckpoints.All(c => CheckpointManager.Instance.checkpoints.ToList().Contains(c.gameObject));
    }
}
