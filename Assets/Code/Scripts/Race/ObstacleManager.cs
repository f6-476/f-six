using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private static ObstacleManager _instance;

    public static ObstacleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObstacleManager>();
            }

            return _instance;
        }
    }

    [Range(2, 100)] public int obstacleCount = 20;
    [Range(2, 500)] public float obstacleWithNoPowerUp = 5;
    public GameObject obstaclePrefab;
    public TrackGenerator track;
    [HideInInspector] public List<GameObject> obstacles;
    private float prefabRadius;
    void Start()
    {
        obstacles = new List<GameObject>();
        prefabRadius = obstaclePrefab.transform.localScale.y * 2;
        InitializePowerups();
    }

    public void InitializePowerups()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            GameObject obstacle = ObjectPlacement.PlaceOneObject(obstacleWithNoPowerUp, obstaclePrefab, this.transform, prefabRadius, track, 6, obstacles);
            if (obstacle)
            {
                obstacles.Add(obstacle);
            }
        }
    }
}
