using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
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

    [Range(2, 100)] public int powerupCount = 20;
    [Range(2, 500)] public float radiusWithNoPowerUp = 200;
    public GameObject powerUpPrefab;
    public TrackGenerator track;
    [HideInInspector] public List<GameObject> powerUps;
    private float prefabRadius;
    void Start()
    {
        powerUps = new List<GameObject>();
        prefabRadius = powerUpPrefab.transform.localScale.y;
        InitializePowerups();
    }

    public void InitializePowerups()
    {
        for (int i = 0; i < powerupCount; i++)
        {
            GameObject p = ObjectPlacement.PlaceOneObject(radiusWithNoPowerUp, powerUpPrefab, this.transform, prefabRadius, track, 8, powerUps);
            if (p)
            {
                powerUps.Add(p);
            }
        }
    }

}
