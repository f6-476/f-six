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

    private float _stopWatch;
    private bool _isCounting;

    private void Start()
    {
        LapsCompleted = 1;
        _shipView.SetLapText(LapsCompleted);
    }

    private void Update()
    {
        if (_isCounting)
        {
            _stopWatch += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
                LapsCompleted++;
                CurrentCheckpoints.Clear();
                _shipView.SetLapText(LapsCompleted);
                _lapTimeList.Add(_stopWatch);
                _shipView.SetLapTimeText(_stopWatch);
                _stopWatch = 0.0f;
            }
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
        return CurrentCheckpoints.Count == CheckpointManager.Instance.checkpoints.Length && 
               CurrentCheckpoints.All(c => CheckpointManager.Instance.checkpoints.ToList().Contains(c.gameObject));
    }
}
