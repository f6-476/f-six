using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : AbstractManager<PlacementManager>
{
    [SerializeField] private List<PlacementConfig> configs;
    private TrackGenerator track;
    private List<GameObject> objectList;
    private Transform placementParent;

    private void Start()
    {
        track = FindObjectOfType<TrackGenerator>();
        objectList = new List<GameObject>();
        placementParent = this.transform;
        InitializeObjects();
    }

    private void InitializeObjects()
    {
        foreach(PlacementConfig config in configs)
        {
            for (int i = 0; i < config.Count; i++)
            {
                PlaceObjectOnTrack(config);
            }
        }
    }

    private GameObject PlaceObjectOnTrack(PlacementConfig config)
    {
        int maxRetires = 30;
        while (--maxRetires > 0)
        {
            (Vector3, Vector3) point = RandomTrackPosition(track);
            Vector3 position = point.Item1;
            Vector3 direction = point.Item2;
            Collider[] hitColliders = Physics.OverlapSphere(position, config.MinDistance, LayerMask.GetMask("PowerUp", "Obstacle"));

            if (hitColliders.Length == 0)
            {
                GameObject gameObject = Instantiate(config.Prefab, Vector3.zero, Quaternion.identity);
                gameObject.transform.SetParent(placementParent);
                gameObject.transform.position = position + direction * config.Prefab.transform.localScale.y;
                gameObject.transform.up = direction;
                objectList.Add(gameObject);
                return gameObject;
            }
        }

        return null;
    }

    private (Vector3, Vector3) RandomTrackPosition(TrackGenerator track)
    {
        float t = Random.Range(0.0f, 0.99f);
        OrientedPoint op = track.segment.GetOrientedPoint(t);
        // TODO: Do we want to customize this?
        float angle = -(Mathf.PI * Random.Range(0.0f, 2.0f));

        Vector3 randomizedPosition = op.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness);
        Vector3 direction = (randomizedPosition - op.position).normalized;

        return (randomizedPosition, direction);
    }
}