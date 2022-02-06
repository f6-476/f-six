using UnityEngine;

//TODO: Separate this into 3 classes: segment, spline, and track
public class BezierCurve : BezierSegment
{
    //[SerializeField] private Transform[] controlPoints = new Transform[4];

    [Range(0, 1)] public float T = 0.5f;

    private Mesh mesh;

    private void Reset()
    {
        points = new[]
        {
            new Vector3(50, 0, 0),
            new Vector3(25, 0, -20),
            new Vector3(-25, 0, 20),
            new Vector3(-50, 0, 0)
        };
    }
    

    private Vector3 GetPos(int i) => transform.TransformPoint(points[i]);//controlPoints[i].position;
    
    
    public override Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;

    }

    public override Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
    }

}
