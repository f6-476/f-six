using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class TrackMap : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TrackGenerator track;
    [Header("Checkpoints")]
    [SerializeField] private Transform checkpointParent;
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] [Range(1, 200)] private int checkpointCount = 50;
    private int widthToThicknessRatio = 5; // This controls the x scale of the checkpoint relative to the thickness of the track.
    [Header("Waypoints")]
    [SerializeField] private Transform waypointParent;
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] [Range(1, 200)] public int waypointCount = 50;
    [Header("Spawns")]
    [SerializeField] private Transform spawnParent;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] [Range(1, 200)] public int spawnCount = 8;
    [Header("Objects")]
    [SerializeField] private Transform placementParent;
    [SerializeField] private PlacementConfig[] placementConfigs;

    public void Clear()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in spawnParent) children.Add(child);
        foreach (Transform child in waypointParent) children.Add(child);
        foreach (Transform child in checkpointParent) children.Add(child);
        foreach (Transform child in placementParent) children.Add(child);

        foreach (Transform child in children) DestroyImmediate(child.gameObject);
    }

    public void Generate()
    {
        Clear();
        GenerateSpawns();
        GenerateWaypoints();
        GenerateCheckpoints();
        GenerateObjects();

        #if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        #endif
    }

    private GameObject GenerateGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        #if UNITY_EDITOR
        GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.transform.parent = parent;
        #else
        GameObject gameObject = Instantiate(prefab, position, rotation, parent);
        #endif

        return gameObject;
    }

    private void GenerateSpawns()
    {
        OrientedPoint orientedPoint = track.segment.GetOrientedPoint(0);
        float angleDelta = (2 * Mathf.PI) / spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            float angle = (float)i * angleDelta;
            Vector3 projectedPoint = orientedPoint.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness);

            GenerateGameObject(spawnPrefab, projectedPoint, orientedPoint.rotation, spawnParent);
        }
    }

    private void GenerateWaypoints()
    {
        for (int i = 0; i < waypointCount; i++)
        {
            float t = i / (float) waypointCount;
            OrientedPoint orientedPoint = track.segment.GetOrientedPoint(t);

            float angle = t * 2 * Mathf.PI;
            Vector3 projectedPoint = orientedPoint.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness);

            GenerateGameObject(waypointPrefab, projectedPoint, orientedPoint.rotation, waypointParent);
        }
    }

    private void GenerateCheckpoints()
    {
        for (int i = 0; i < checkpointCount; i++)
        {
            float t = i / (float) checkpointCount;
            OrientedPoint orientedPoint = track.segment.GetOrientedPoint(t);
 
            float angle = Mathf.PI * -1.5f;
            Vector3 innerPoint = orientedPoint.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness);
            Vector3 projectedPoint = innerPoint - Vector3.up * (track.thickness / 2);
            Quaternion rotation = Quaternion.Euler(0, orientedPoint.rotation.eulerAngles.y, orientedPoint.rotation.eulerAngles.z);

            GameObject gameObject = GenerateGameObject(checkpointPrefab, projectedPoint, rotation, checkpointParent);
            gameObject.transform.localScale = new Vector3(track.thickness * widthToThicknessRatio, gameObject.transform.localScale.y, gameObject.transform.localScale.z);

            Checkpoint checkpoint = gameObject.GetComponent<Checkpoint>();
            checkpoint.UseRespawn = true;
            checkpoint.RespawnPosition = innerPoint;
            checkpoint.RespawnRotation = orientedPoint.rotation;
        }
    }

    private void GenerateObjects()
    {
        foreach(PlacementConfig config in placementConfigs)
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
                GameObject gameObject = GenerateGameObject(config.Prefab, Vector3.zero, Quaternion.identity, placementParent);

                gameObject.transform.position = position + direction * config.Prefab.transform.localScale.y;
                gameObject.transform.up = direction;

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
