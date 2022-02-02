using UnityEngine;

public abstract class BezierSegment : MonoBehaviour
{
    public abstract Vector3 GetPoint(float t);
    public abstract Vector3 GetVelocity(float t);
    public Vector3 GetTangent(float t) => GetVelocity(t).normalized;
    public OrientedPoint GetOrientedPoint(float t) => new OrientedPoint(GetPoint(t),GetTangent(t));
}
