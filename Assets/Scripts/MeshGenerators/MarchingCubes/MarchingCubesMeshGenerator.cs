using System.Collections.Generic;
using Caves.CaveMesh;
using Caves.Chunks;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGenerators.MarchingCubes
{
	public class MarchingCubesMeshGenerator : MeshGenerator
	{
		private int[,,] _matrix;

		public MarchingCubesMeshGenerator(MeshGeneratorSettings settings, CaveWall wall) : base(settings, wall)
		{
		}

		public override void Init(int[,,] matrix)
		{
			_matrix = matrix;
		}

		public override MeshData Generate()
		{
			List<Vector3> vertices = new List<Vector3>();
			//List<Vector3> normals = new List<Vector3>();
			List<int> triangles = new List<int>();

			int length = _matrix.GetLength(0);
			int width = _matrix.GetLength(1);
			int height = _matrix.GetLength(2);

			for (int i = 0; i < length - 1; i++)
			{
				for (int j = 0; j < width - 1; j++)
				{
					for (int k = 0; k < height - 1; k++)
					{
						int nodeConfiguration = MeshGeneratorData.GetNodeConfiguration(_matrix, i, j, k);

						foreach (Vector3 vertex in MeshGeneratorData.GetVertices(nodeConfiguration))
						{
							Vector3 vertexPosition = new Vector3((vertex.x + i - 1) * Settings.GridSize.x, (vertex.y + j - 1) * Settings.GridSize.y, (vertex.z + k - 1) * Settings.GridSize.z);
							vertices.Add(vertexPosition);

							int triangleIndex = vertices.Count - 1;
							triangles.Add(triangleIndex);

							//normals.Add(vertex);
						}
					}
				}
			}

			//mesh.uv = Unwrapping.GeneratePerTriangleUV(mesh);

			return new MeshData(vertices, triangles);
		}
	}
}