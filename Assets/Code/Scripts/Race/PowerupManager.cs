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
    [Range(2, 20)] public float radiusWithNoPowerUp = 5;
    public GameObject powerUpPrefab;
    public TrackGenerator track;
    [HideInInspector] public List<PowerUp> powerUps;
    private float prefabRadius;
    void Start()
    {
        powerUps = new List<PowerUp>();
        prefabRadius = powerUpPrefab.transform.localScale.y;
        InitializePowerups();
    }

    public void InitializePowerups()
    {
        for (int i = 0; i < powerupCount; i++)
        {
            PlaceOnePowerup();
        }
    }

    public void PlaceOnePowerup(int maxRetires = 30, PowerUpType pType = PowerUpType.BOOST)
    {
        Vector3 randomizedPoint = FindAPosition();
        Collider[] hitColliders = Physics.OverlapSphere(randomizedPoint, radiusWithNoPowerUp, LayerMask.NameToLayer("PowerUp"));
        while (hitColliders.Length > 0 && maxRetires > 0)
        {
            randomizedPoint = FindAPosition();
            hitColliders = Physics.OverlapSphere(randomizedPoint, radiusWithNoPowerUp, LayerMask.NameToLayer("PowerUp"));
            maxRetires -= 1;
        }

        if (maxRetires > 0)
        {
            GameObject go = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(transform);
            go.transform.position = randomizedPoint;
            PowerUp addedPowerup = go.GetComponent<PowerUp>();
            addedPowerup.type = pType;
            powerUps.Add(addedPowerup);
        }
    }
    private Vector3 FindAPosition()
    {
        float t = Random.Range(0.0f, 0.99f);
        OrientedPoint op = track.segment.GetOrientedPoint(t);
        float angle = -(Mathf.PI * Random.Range(1.4f, 1.6f));
        
        Vector3 randomizedPoint = op.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness) + Vector3.up * prefabRadius;
        return randomizedPoint;
    }
}
