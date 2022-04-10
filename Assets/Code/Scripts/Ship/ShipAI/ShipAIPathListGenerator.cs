using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAIPathListGenerator : MonoBehaviour
{
    [Range(2, 64)] public int distanceBetweenWayPoints = 2;
    public List<OrientedPoint> waypoints;
    public TrackGenerator track;
    // Start is called before the first frame update
    void Start()
    {
        waypoints = new List<OrientedPoint>();
        GenerateWaypoints();
    }

    public void GenerateWaypoints()
    {
        int waypointCount = CalculateNumOfWaypoints();

        waypointCount += 1;
        for (int point = 0; point < waypointCount - 1; point++)
        {
            float t = point / (waypointCount - 1f);
            OrientedPoint op = track.segment.GetOrientedPoint(t);
            if (point < waypointCount)
            {
                float angle = Mathf.PI * -1.5f;

                Vector3 waypoint = op.LocalToWorldPosition(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * track.thickness) + Vector3.up;
                OrientedPoint o = new OrientedPoint(waypoint, op.rotation);
                waypoints.Add(o);
            }
        }
    }

    private int CalculateNumOfWaypoints()
    {
        int waypointCount = 100;    // Assume 100 points

        // calculate the distance between each point if we put 100 points
        float t = 1 / (waypointCount - 1f);
        Vector3 start = track.segment.GetOrientedPoint(0).position;
        Vector3 end = track.segment.GetOrientedPoint(t).position;
        start -= Vector3.up * start.y;
        end -= Vector3.up * end.y;
        float currentDistanceBetweenPoints = Vector3.Distance(start, end);

        // calculate the number of points we actually need based on the desired distance between points
        waypointCount = (int)(waypointCount * (currentDistanceBetweenPoints / distanceBetweenWayPoints));

        return waypointCount;
    }
}
