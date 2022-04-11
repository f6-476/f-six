using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{

    public static GameObject PlaceOneObject(float radiusWithNoObject, GameObject objectPrefab, Transform objectManager, float objectRadius, TrackGenerator track, int otherLayer, List<GameObject> objectHolder)
    {
        int maxRetires = 30;
        Vector3 randomizedPoint = FindAPosition(objectRadius, track);
        Collider[] hitColliders = Physics.OverlapSphere(randomizedPoint, objectRadius * 2, 1 << 8);
        bool closeFound = true;
        while (maxRetires > 0 && closeFound)
        {
            closeFound = false;
            randomizedPoint = FindAPosition(objectRadius, track);
            hitColliders = Physics.OverlapSphere(randomizedPoint, objectRadius * 2, 1 << 8);
            if (hitColliders.Length != 0)
            {
                maxRetires -= 1;
                closeFound = true;
                continue;
            }
            foreach (GameObject p in objectHolder)
            {
                if (Vector3.Distance(p.gameObject.transform.position, randomizedPoint) < radiusWithNoObject)
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
            go.transform.position = randomizedPoint;
            return go;
        }
        return null;
    }

    private static Vector3 FindAPosition(float objectRadius, TrackGenerator track)
    {
        float t = Random.Range(0.0f, 0.99f);
        OrientedPoint op = track.segment.GetOrientedPoint(t);
        float angle = -(Mathf.PI * Random.Range(1.4f, 1.6f));

        Vector3 randomizedPoint = op.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness) + Vector3.up * objectRadius;
        return randomizedPoint;
    }
}
