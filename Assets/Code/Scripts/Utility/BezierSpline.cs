using System;
using UnityEngine;

public class BezierSpline : BezierSegment
{
    //[SerializeField] private Transform[] controlPoints = new Transform[4];

    [SerializeField]
    private Vector3[] points = new []{
        new Vector3(50, 0, 0),
        new Vector3(25, 0, -20),
        new Vector3(-25, 0, 20),
        new Vector3(-50, 0, 0)
    };
    
    [Range(0, 1)] public float T = 0.5f;
    private Mesh mesh;
    public float nextCurveScale = 1;
    public int CurveCount {
        get {
            return (points.Length - 1) / 3;
        }
    }
    public int ControlPointCount {
        get {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint (int index) {
        return points[index];
    }

    public void SetControlPoint (int index, Vector3 point) {
        points[index] = point;
    }

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


    // https://catlikecoding.com/unity/tutorials/curves-and-splines/#a-array-resize
    public void AddCurve () {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        var forward = GetTangent(1);
        point += forward * nextCurveScale;
        points[points.Length - 3] = point;
        point += forward * nextCurveScale;
        points[points.Length - 2] = point;
        point += forward * nextCurveScale;
        points[points.Length - 1] = point;
    }

    public override Vector3 GetPoint (float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(
            points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public override Vector3 GetVelocity (float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(
            points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetOrientedPoint(T).position,1);
    }
}
