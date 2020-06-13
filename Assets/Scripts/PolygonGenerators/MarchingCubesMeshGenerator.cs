using System.Collections.Generic;
using Caves.CaveMesh;
using UnityEngine;
using UnityEngine.Rendering;

namespace PolygonGenerators
{
	public class MarchingCubesMeshGenerator : MeshGenerator
	{
		public MarchingCubesMeshGenerator(PolygonGeneratorSettings settings) : base(settings)
		{
		}

		public override Mesh Generate(int[,,] nodeMatrix)
		{
			List<Vector3> vertices = new List<Vector3>();
			//List<Vector3> normals = new List<Vector3>();
			List<int> triangles = new List<int>();

			Vector3 cellSize = Settings.CellSize;

			int length = nodeMatrix.GetLength(0);
			int width = nodeMatrix.GetLength(1);
			int height = nodeMatrix.GetLength(2);

			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < height; k++)
					{
						int nodeConfiguration = MarchingCubesData.GetNodeConfiguration(nodeMatrix, i, j, k);

						foreach (Vector3 vertex in MarchingCubesData.GetVertices(nodeConfiguration))
						{
							Vector3 vertexPosition = new Vector3((vertex.x + i) * cellSize.x, (vertex.y + j) * cellSize.y, (vertex.z + k) * cellSize.z);
							vertices.Add(vertexPosition);

							int triangleIndex = vertices.Count - 1;
							triangles.Add(triangleIndex);

							//normals.Add(vertex);
						}
					}
				}
			}

			Mesh mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32; //TODO maybe optimization will fix this
			Vector3[] verticesArray = vertices.ToArray();
			mesh.vertices = verticesArray;
			mesh.triangles = triangles.ToArray();
			//mesh.normals = normals.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.RecalculateTangents();
			mesh.Optimize();
			//mesh.uv = Unwrapping.GeneratePerTriangleUV(mesh);

			return mesh;
		}
	}
}