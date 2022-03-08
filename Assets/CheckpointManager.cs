using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager _instance;

    public static CheckpointManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CheckpointManager>();
            }

            return _instance;
        }
    }
    
    [Range(2, 64)] public int checkpointCount = 2;
    [Range(2, 10)] public int WidthToThicknessRatio = 5; // This controls the x scale of the checkpoint relative to the thickness of the track.
    public GameObject[] checkpoints;
    public GameObject checkpointPrefab;
    private TrackGenerator track;

    void Start()
    {
        track = GetComponent<TrackGenerator>();
        SetCheckpoints();  
    }
    public void SetCheckpoints()
    {
        checkpoints = new GameObject[checkpointCount];
        for (int i = 0; i < checkpointCount; i++)
        {
            GameObject go = Instantiate(checkpointPrefab, Vector3.zero, Quaternion.identity);
            go.transform.localScale = new Vector3(track.thickness * WidthToThicknessRatio, go.transform.localScale.y, go.transform.localScale.z);
            go.transform.SetParent(transform);
            checkpoints[i] = go;
        }

        checkpointCount += 1;
        for (int point = 0; point < checkpointCount - 1; point++)
        {
            float t = point / (checkpointCount - 1f);
            OrientedPoint op = track.segment.GetOrientedPoint(t);
            if (point < checkpoints.Length)
            {
                float angle = Mathf.PI * -1.5f;

                Vector3 randomizedPoint = op.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness) - Vector3.up * (track.thickness / 2);
                checkpoints[point].transform.position = randomizedPoint;
                checkpoints[point].transform.rotation = Quaternion.Euler(0, op.rotation.eulerAngles.y, op.rotation.eulerAngles.z);
            }
        }
        checkpointCount -= 1;
    }
}
