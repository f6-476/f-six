using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private List<Ship> _playersShips = new List<Ship>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Ranks players based on their laps completed and amount of checkpoints passed in current lap
    /// First, all players are ranked by laps completed
    /// For sure, a lot of players will have same number of laps (most of the time, all of them)
    /// So on each lap level, those players are sorted by the amount of checkpoints passed
    /// The more checkpoints passed, the higher the rank among the same lapped players
    /// Lists are ordered by descending (highest rank first)
    ///
    /// Method is called each time a player triggers a checkpoint (not putting it on Update() for better optimization)
    /// </summary>
    public void RankPlayers()
    {
        var sortedByLap = _playersShips.OrderByDescending(p => p.Info.LapsCompleted).ToList();
        for (var i = sortedByLap[0].Info.LapsCompleted; i >= 1; i--)
        {
            // If at most one instance of this lap is found, no need to sort it, so skip
            if (sortedByLap.FindAll(s => s.Info.LapsCompleted == i).Count <= 1) continue;

            var sameLapPlayers = _playersShips.FindAll(p => p.Info.LapsCompleted == i);
            var firstIndex = sortedByLap.FindIndex(p => p.Info.LapsCompleted == i);
            var orderedSameLapPlayers = sameLapPlayers.OrderByDescending(p => p.Info.CurrentCheckpoints.Count).ToList();
            // Rearrange same-lap-players based on their checkpoint values 
            for (var j = 0; j < sameLapPlayers.Count; j++)
            {
                sortedByLap[j + firstIndex] = orderedSameLapPlayers[j];
            }
        }

        // Players with most laps and checkpoints completed (in that lap) have the smallest index number in list
        foreach (var ship in _playersShips.Where(ship => sortedByLap.Contains(ship)))
        {
            ship.Info.CurrentRank = sortedByLap.FindIndex(s => s == ship) + 1;
        }

        _playersShips = _playersShips.OrderByDescending(s => s.Info.CurrentRank).ToList();
    }
}
