using UnityEngine;

public abstract class BezierSegment : MonoBehaviour
{
    public abstract Vector3 GetBezierTangent(float t);
    public abstract OrientedPoint GetBezierPoint(float t);
}
