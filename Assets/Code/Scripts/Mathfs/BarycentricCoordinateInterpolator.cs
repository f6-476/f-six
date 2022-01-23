using UnityEngine;

public static class BarycentricCoordinateInterpolator
{
    public static Vector3 GetInterpolatedNormal(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<MeshCollider>(out MeshCollider meshCollider))
        {
            return CalculateInterpolatedMeshNormal(meshCollider.sharedMesh, hit);
        }
        else if (hit.transform.TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            return hit.normal;
        }
        else if (hit.transform.TryGetComponent<CapsuleCollider>(out CapsuleCollider capsuleCollider))
        {
            return hit.normal;
        }

        return Vector3.up;
    }
 
    private static Vector3 CalculateInterpolatedMeshNormal(Mesh mesh, RaycastHit hit)
    {
        if (mesh == null) return Vector3.up;

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
 
        return interpolatedNormal;
    }
}
