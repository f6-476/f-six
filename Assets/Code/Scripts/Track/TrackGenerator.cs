using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [Range(3, 128)] public int resolution = 10;
    [Range(2, 64)] public int edgeRingCount = 2;
    [Range(1, 20)] public float thickness = 3;
    private Mesh mesh;
    public Vector3[] vertices;
    public BezierSegment segment;
    public bool drawInEditor = true;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Segment";
        GetComponent<MeshFilter>().sharedMesh = mesh;
        var collider = GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.enabled = true;
        segment = GetComponent<BezierSegment>();
        vertices = GenerateVertices();
        GenerateMesh();
    }
    private void Update()
    {
        GenerateMesh();
    }

    private void OnValidate()
    {
        vertices = GenerateVertices();
        
        //if(drawInEditor) GenerateMesh();
        
    }
    
    public void GenerateMesh()
    {
        mesh.Clear();
        
        
        
        // Vertices
        List<Vector3> verts = new List<Vector3>();
        for (int ring = 0; ring < edgeRingCount; ring++)
        {
            
            /*Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Handles.DrawLine(point, point + spline.GetTangent(0f) * directionScale);
            int steps = stepsPerCurve * spline.CurveCount;
            for (int i = 1; i <= steps; i++) {
                point = spline.GetPoint(i / (float)steps);
                Handles.DrawLine(point, point + spline.GetTangent(i / (float)steps) * directionScale);
            }*/
            
            float t = ring / (edgeRingCount - 1f);

            OrientedPoint op = segment.GetOrientedPoint(t);
            
            for (int i = 0; i < vertices.Length; i++)
            {
                verts.Add(op.LocalToWorldPosition(vertices[i] * thickness));
            }
        }
   
        // Triangle
        List<int> triangleIndices = new List<int>();
        for (int ring = 0; ring < edgeRingCount - 1; ring++)
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
                
                /*Debug.Log("index currentA: " + currentA);
                Debug.Log("index currentB: " + currentB);
                Debug.Log("index nextA: " + nextA);
                Debug.Log("index nextB: " + nextB);*/
                
                triangleIndices.Add(currentA);
                triangleIndices.Add(nextA);
                triangleIndices.Add(nextB);
                triangleIndices.Add(currentA);
                triangleIndices.Add(nextB);
                triangleIndices.Add(currentB);

            }

        }
        mesh.SetVertices(verts);
        mesh.SetTriangles(triangleIndices,0);
        mesh.RecalculateNormals();
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
