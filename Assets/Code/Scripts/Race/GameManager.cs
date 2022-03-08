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
    [SerializeField] private TrackGenerator _trackGenerator;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        SetPlayerRanks();
    }

    private void SetPlayerRanks()
    {
        var sortedShips = _playersShips.OrderBy(ship => ship.Info.TrackDistance()).ToList();
        for (var i = 0; i < sortedShips.Count; i++)
        {
            sortedShips[i].Info.CurrentRank = i;
        }
    }
}
