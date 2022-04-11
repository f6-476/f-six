using System;
using UnityEditor;
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

    [SerializeField]
    private bool loop;

    // _________PROPERTIES_________


    public int ControlPointCount 
    {
        get => points.Length; 
    }

    public bool Loop {
        get {
            return loop;
        }
        set {
            loop = value;
            if (value == true) {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
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
        if (index % 3 == 0) {
            Vector3 delta = point - points[index];
            if (loop) {
                if (index == 0) {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1) {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else {
                if (index > 0) {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length) {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }

    public void SetControlPointMode (int index, BezierControlPointMode mode) 
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop) {
            if (modeIndex == 0) {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1) {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }
    
    private void EnforceMode (int index) 
    {
       int modeIndex = (index + 1) / 3;
       BezierControlPointMode mode = modes[modeIndex];
       if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
           return;
       }
       
       int middleIndex = modeIndex * 3;
       int fixedIndex, enforcedIndex;
       if (index <= middleIndex) {
           fixedIndex = middleIndex - 1;
           if (fixedIndex < 0) {
               fixedIndex = points.Length - 2;
           }
           enforcedIndex = middleIndex + 1;
           if (enforcedIndex >= points.Length) {
               enforcedIndex = 1;
           }
       }
       else {
           fixedIndex = middleIndex + 1;
           if (fixedIndex >= points.Length) {
               fixedIndex = 1;
           }
           enforcedIndex = middleIndex - 1;
           if (enforcedIndex < 0) {
               enforcedIndex = points.Length - 2;
           }
       }
       Vector3 middle = points[middleIndex];
       Vector3 enforcedTangent = middle - points[fixedIndex];
       if (mode == BezierControlPointMode.Aligned) {
           enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
       }
       points[enforcedIndex] = middle + enforcedTangent;
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
       EnforceMode(points.Length - 4);
       
       if (loop) {
           points[points.Length - 1] = points[0];
           modes[modes.Length - 1] = modes[0];
           EnforceMode(0);
       }
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
        Gizmos.DrawSphere(GetOrientedPoint(T).position,10);
        Handles.PositionHandle(GetOrientedPoint(T).position, GetOrientedPoint(T).rotation);
    }
}
