using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
   [System.Serializable] // what does this do?
   public class Vertex
   {
      public Vector2 point;
      public Vector2 normal;
      public float u; // The U portion of UV's --> V is generated
      // bitangent
      // vertex color
   }

   public Vertex[] vertices;
   public int[] lineIndices;
   

}
