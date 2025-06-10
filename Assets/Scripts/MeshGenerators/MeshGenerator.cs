using Caves.CaveMeshes;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGenerators
{
    public abstract class MeshGenerator
    {
        protected readonly BaseGeneratorSettings _settings;
        protected readonly CaveMesh _caveMesh;

        public abstract MeshData Generate(int[,,] nodeMatrix);

        protected MeshGenerator(BaseGeneratorSettings settings, CaveMesh caveMesh)
        {
            _settings = settings;
            _caveMesh = caveMesh;
        }

        public Mesh CreateMesh(MeshData data)
        {
            Mesh mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = data.Vertices,
                triangles = data.Triangles
            };

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            return mesh;
        }
    }
}