using UnityEngine;

public abstract class BezierSegment : MonoBehaviour
{
    [SerializeField]
    public  Vector3[] points = new []{
        new Vector3(50, 0, 0),
        new Vector3(25, 0, -20),
        new Vector3(-25, 0, 20),
        new Vector3(-50, 0, 0)
    };
    public abstract Vector3 GetPoint(float t);
    public abstract Vector3 GetVelocity(float t);
    public Vector3 GetTangent(float t) => GetVelocity(t).normalized;
    public OrientedPoint GetOrientedPoint(float t) => new OrientedPoint(GetPoint(t),GetTangent(t));
    public int CurveCount {
        get {
            return (points.Length - 1) / 3;
        }
    }
}
