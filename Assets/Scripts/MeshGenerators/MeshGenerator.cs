using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGenerators
{
	public abstract class MeshGenerator
	{
		public readonly MeshGeneratorSettings Settings;

		public abstract MeshData Generate(int[,,] nodeMatrix);

		protected MeshGenerator(MeshGeneratorSettings settings)
		{
			Settings = settings;
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