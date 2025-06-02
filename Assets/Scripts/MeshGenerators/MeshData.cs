using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerators
{
    public class MeshData
    {
        public Vector3[] Vertices;
        public int[] Triangles;

        public MeshData(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }

        public MeshData(List<Vector3> vertices, List<int> triangles)
        {
            Vertices = vertices.ToArray();
            Triangles = triangles.ToArray();
        }
    }
}