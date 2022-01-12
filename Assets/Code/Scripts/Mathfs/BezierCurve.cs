using PathCreation;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{

    public List<Vector3> CirclePoints = new List<Vector3>();
    [Range(0, 1)] public float T = 0.5f;
    [Range(3, 50)] public int resolution = 3;
    [Range(2, 32)] public int edgeRingCount;
    [Range(1, 20)] public float thickness;
    [Range(3, 30)] public int segmentCount;
    [Range(50, 300)] public float trackRadius;
    [Range(20, 100)] public float noiseLevel;
    private Mesh mesh;
    private Vector3[] controlPoints = new Vector3[4];
    public Vector3[] vertices;

    private void Start()
    {
        PointsOnCircle(segmentCount);
        // Create a new bezier path from the waypoints.
        BezierPath bezierPath = new BezierPath(CirclePoints.ToArray(), true, PathSpace.xyz);
        GetComponent<PathCreator>().bezierPath = bezierPath;
        mesh = new Mesh();
        mesh.name = "Track";
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        vertices = GenerateVertices();
        GenerateMesh();

    }
    public void PointsOnCircle(int NumberOfPoints)
    {
        double slice = 2 * Mathf.PI / NumberOfPoints;
        for (int i = 0; i < NumberOfPoints; i++)
        {
            float x = trackRadius * Mathf.Cos((float)slice * i);
            float y = trackRadius * Mathf.Sin((float)slice * i);
            float noiseX = Random.Range(-noiseLevel, noiseLevel);
            float noiseY = Random.Range(-noiseLevel, noiseLevel);
            CirclePoints.Add(new Vector3(x + noiseX, 0, y + noiseY));
        }
    }
    public void GenerateMesh()
    {
        mesh.Clear();
        // Vertices
        List<Vector3> verts = new List<Vector3>();
        List<int> triangleIndices = new List<int>();
        for (int seg = 0; seg < segmentCount; seg++)
        {
            controlPoints = GetComponent<PathCreator>().bezierPath.GetPointsInSegment(seg);

            for (int ring = 0; ring < edgeRingCount; ring++)
            {
                float t = ring / (edgeRingCount - 1f);

                OrientedPoint op = GetBezierPoint(t);

                for (int i = 0; i < vertices.Length; i++)
                {
                    verts.Add(op.LocalToWorldPosition(vertices[i] * thickness));
                }
            }

            // Triangle

            for (int ring = 0; ring < segmentCount * edgeRingCount - 1; ring++)
            {
                int rootIndex = ring * vertices.Length;
                int rootIndexNext = (ring + 1) * vertices.Length;

                for (int line = 0; line < vertices.Length; line++)
                {
                    int lineIndexA = line;
                    int lineIndexB = line == vertices.Length - 1 ? 0 : line + 1;


                    int currentA = rootIndex + lineIndexA;
                    int currentB = rootIndex + lineIndexB;
                    int nextA = rootIndexNext + lineIndexA;
                    int nextB = rootIndexNext + lineIndexB;

                    triangleIndices.Add(currentA);
                    triangleIndices.Add(nextA);
                    triangleIndices.Add(nextB);
                    triangleIndices.Add(currentA);
                    triangleIndices.Add(nextB);
                    triangleIndices.Add(currentB);

                }

            }

        }
        mesh.SetVertices(verts);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.RecalculateNormals();
    }
    OrientedPoint GetBezierPoint(float t)
    {
        Vector3 p0 = controlPoints[0];
        Vector3 p1 = controlPoints[1];
        Vector3 p2 = controlPoints[2];
        Vector3 p3 = controlPoints[3];

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

    public Vector3[] GenerateVertices()
    {
        List<Vector3> verts = new List<Vector3>();
        verts.Clear();
        for (int i = 0; i < resolution; i++)
        {
            var angle = -(Mathf.PI * 2) / resolution;
            verts.Add(new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0));
        }

        return verts.ToArray();
    }
}
