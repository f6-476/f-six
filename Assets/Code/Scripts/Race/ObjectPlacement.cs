using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPlacement : MonoBehaviour
{
    [Range(2, 100)] public int objectCount = 20;
    [Range(2, 500)] public float radiusWithNoObject = 200;
    public GameObject objectPrefab;
    [Range(0, 2)] public float minPiAngleRange = 0;
    [Range(0, 2)] public float maxPiAngleRange = 2;
    [HideInInspector] public TrackGenerator track;
    [HideInInspector] public List<GameObject> objectList;
    [HideInInspector] public float prefabRadius;

    public void InitializeObjectPlacementVariables()
    {
        track = FindObjectOfType<TrackGenerator>();
        objectList = new List<GameObject>();
    }

    public GameObject PlaceOneObject(Transform objectManager)
    {
        int maxRetires = 30;
        (Vector3,Vector3) point = FindAPosition(prefabRadius, track);
        Collider[] hitColliders = Physics.OverlapSphere(point.Item1, prefabRadius * 2, LayerMask.GetMask("PowerUp", "Obstacle"));
        bool closeFound = true;
        while (maxRetires > 0 && closeFound)
        {
            closeFound = false;
            point = FindAPosition(prefabRadius, track);
            hitColliders = Physics.OverlapSphere(point.Item1, prefabRadius * 2, LayerMask.GetMask("PowerUp", "Obstacle"));
            if (hitColliders.Length != 0)
            {
                maxRetires -= 1;
                closeFound = true;
                continue;
            }
            foreach (GameObject p in objectList)
            {
                if (Vector3.Distance(p.gameObject.transform.position, point.Item1) < radiusWithNoObject)
                {
                    maxRetires -= 1;
                    closeFound = true;
                    break;
                }
            }

        }

        if (maxRetires > 0)
        {
            GameObject go = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(objectManager);
            go.transform.position = point.Item1 + point.Item2 * prefabRadius;
            go.transform.up = point.Item2;
            objectList.Add(go);
            return go;
        }
        return null;
    }

    private (Vector3,Vector3) FindAPosition(float objectRadius, TrackGenerator track)
    {
        float t = Random.Range(0.0f, 0.99f);
        OrientedPoint op = track.segment.GetOrientedPoint(t);
        float angle = -(Mathf.PI * Random.Range(minPiAngleRange, maxPiAngleRange));

        Vector3 randomizedPosition = op.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness);
        Vector3 direction = (randomizedPosition - op.position).normalized;
        return (randomizedPosition, direction);
    }

    public abstract void InitializeObjects();
}
