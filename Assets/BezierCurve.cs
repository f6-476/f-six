using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using System.Linq;
//TODO: Separate this into 3 classes: curve, spline, and track
public class BezierCurve : MonoBehaviour
{
    [SerializeField] private Transform[] controlPoints = new Transform[4];
    [Range(0, 1)] public float T = 0.5f;
    [Range(3, 50)] public int resolution = 3;
    [Range(2, 32)] public int edgeRingCount;
    [Range(1, 20)] public float thickness;

    private Mesh mesh;
    public Vector3[] vertices;


    private void OnValidate()
    {
        vertices = GenerateVertices();
        //GenerateMesh();
        
    }


    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Segment";
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        vertices = GenerateVertices();
        GenerateMesh();
    }

    private void Update()
    {
        GenerateMesh();
    }

    public Vector3 GetPos(int i) => controlPoints[i].position;
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (var point in controlPoints)
        {
            Gizmos.DrawSphere(point.position,1f);
        }
        Handles.DrawBezier(GetPos(0),GetPos(3),GetPos(1),GetPos(2),
            Color.white, EditorGUIUtility.whiteTexture, 1f);
        Gizmos.DrawLine(GetPos(0),GetPos(1));
        Gizmos.DrawLine(GetPos(2),GetPos(3));
        
       var bPoint =  GetBezierPoint(T);
       Handles.PositionHandle(bPoint.position, bPoint.rotation);
       //mesh.vertices.Select(v => bPoint.LocalToWorldPosition(v.point));
       Vector3[] verts = vertices.Select(v => bPoint.LocalToWorldPosition(v * thickness)).ToArray();
       for(int i = 0; i < verts.Length; i++)
       {
           Vector3 a;
           Vector3 b;
           if (i == verts.Length - 1)
           {
               a = verts[i];
               b = verts[0];
           }
           else
           {
               a = verts[i];
               b = verts[i + 1];
           }
           Gizmos.DrawLine(a,b);
       }

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
    
    public void GenerateMesh()
    {
        mesh.Clear();
        
        // Vertices
        List<Vector3> verts = new List<Vector3>();
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

    OrientedPoint GetBezierPoint(float t)
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

    Vector3 GetBezierTangent(float t)
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

    Quaternion GetBezierOrientation(float t)
    {
        Vector3 tangent = GetBezierTangent(t);
        return Quaternion.LookRotation(tangent , transform.up);
    }
     
}
