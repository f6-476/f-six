using System;
using UnityEngine;

// Code modified from:
// https://catlikecoding.com/unity/tutorials/curves-and-splines
public class BezierSpline : BezierSegment
{
    //_________FIELDS_________
    
    [SerializeField]
    private BezierControlPointMode[] modes;
    [Range(0, 1)] public float T = 0.5f;
    private Mesh mesh;
    //This variable should be part of the editor when using "AddCurve", not the actual spline script
    public float pointSpacing = 5;
    
    // _________PROPERTIES_________
    
    public int ControlPointCount 
    {
        get => points.Length; 
    }
    
    // _________METHODS_________

    public Vector3 GetControlPoint (int index) 
    {
        return points[index];
    }

    public BezierControlPointMode GetControlPointMode (int index) 
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPoint (int index, Vector3 point) 
    {
        points[index] = point;
    }

    public void SetControlPointMode (int index, BezierControlPointMode mode) 
    {
        modes[(index + 1) / 3] = mode;
    }
    
    private void EnforceMode (int index) 
    {
       // int modeIndex = (index + 1) / 3;
    }


    private Vector3 GetPos(int i) => transform.TransformPoint(points[i]);//controlPoints[i].position;
    
    public override Vector3 GetPoint (float t) 
    {
        int i;
        if (t >= 1f) 
        {
            t = 1f;
            i = points.Length - 4;
        }
        else 
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(
            points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public override Vector3 GetVelocity (float t) 
    {
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

    public void AddCurve () 
    {
        Vector3 point = points[points.Length - 1];
        var forward = transform.InverseTransformDirection(GetTangent(1));
        Array.Resize(ref points, points.Length + 3);
        point += forward * pointSpacing;
        points[points.Length - 3] = point;
        point += forward * pointSpacing;
        points[points.Length - 2] = point;
        point += forward * pointSpacing;
        points[points.Length - 1] = point;
        
        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
       //EnforceMode(points.Length - 4);
    }

    public void RemoveCurve()
    {
        
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
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetOrientedPoint(T).position,1);
    }
}
