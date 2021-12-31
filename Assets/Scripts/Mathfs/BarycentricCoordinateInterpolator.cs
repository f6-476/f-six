using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BarycentricCoordinateInterpolator
{
    public static Vector3 GetInterpolatedNormal(RaycastHit hit)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
 
        if (!meshCollider || !meshCollider.sharedMesh)
        {
            //Debug.LogWarning("No MeshCollider attached to to the mesh!", hit.collider);
            return Vector3.up;
        }
 
        Mesh mesh = meshCollider.sharedMesh;
        Vector3 normal = CalculateInterpolatedNormal(mesh, hit);
     
        return normal;
    }
 
    private static Vector3 CalculateInterpolatedNormal(Mesh mesh, RaycastHit hit)
    {
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
 
        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];
 
        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = hit.barycentricCoordinate;
 
        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;
 
        // Transform local space normals to world space
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);
 
        // Display with Debug.DrawLine
        Debug.DrawRay(hit.point, interpolatedNormal);
 
        return interpolatedNormal;
    }
}
