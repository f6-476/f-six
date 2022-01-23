using System;
using UnityEngine;

public class BezierSpline : BezierSegment
{
    //[SerializeField] private Transform[] controlPoints = new Transform[4];

    public Vector3[] points = new []{
        new Vector3(50, 0, 0),
        new Vector3(25, 0, -20),
        new Vector3(-25, 0, 20),
        new Vector3(-50, 0, 0)
    };
    [Range(0, 1)] public float T = 0.5f;

    private Mesh mesh;

    public int CurveCount {
        get {
            return (points.Length - 1) / 3;
        }
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
    
    // public void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.white;
    //     foreach (var point in points)//controlPoints)
    //     {
    //         Gizmos.DrawSphere(transform.TransformPoint(point),1f);
    //     }
    //     Handles.DrawBezier(GetPos(0),GetPos(3),GetPos(1),GetPos(2),
    //         Color.white, EditorGUIUtility.whiteTexture, 1f);
    //     Gizmos.DrawLine(GetPos(0),GetPos(1));
    //     Gizmos.DrawLine(GetPos(2),GetPos(3));
    //     
    // }


    public override OrientedPoint GetBezierPoint(float t)
    {
        Vector3 p0 = GetPos(0);
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);
        
        Vector3 pos = Vector3.Lerp(d, e, t);
        
        Gizmos.color = Color.red;

        var tangent = (e - d).normalized;
        OrientedPoint point = new OrientedPoint(pos, tangent);
       
        return point;

    }

    public override Vector3 GetBezierTangent(float t)
    {
        Vector3 p0 = GetPos(0);  
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);
        
        return (e-d).normalized;
    }

    // https://catlikecoding.com/unity/tutorials/curves-and-splines/#a-array-resize
    public void AddCurve () {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;
    }
    
    
     
}
