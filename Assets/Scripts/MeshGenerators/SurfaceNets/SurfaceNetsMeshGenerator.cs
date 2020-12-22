using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGenerators.SurfaceNets
{
	public class SurfaceNetsMeshGenerator : MeshGenerator
	{
		public SurfaceNetsMeshGenerator(MeshGeneratorSettings settings) : base(settings)
		{
		}

		public override MeshData Generate(int[,,] nodeMatrix)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			List<MeshGeneratorNode> surfaceNodes = GetSurfaceNodes(nodeMatrix);

			for (int i = 0; i < Settings.SmoothIterationCount; i++)
			{
				foreach (MeshGeneratorNode node in surfaceNodes)
				{
					node.PlaceEquidistant();
				}
			}

			foreach (MeshGeneratorNode node in surfaceNodes)
			{
				List<Vector3> nodeTriangles = node.GetAllTriangles();

				foreach (Vector3 vertex in nodeTriangles)
				{
					Vector3 scaledVertex = new Vector3(vertex.x * Settings.GridSize.x, vertex.y * Settings.GridSize.y, vertex.z * Settings.GridSize.z);
					vertices.Add(scaledVertex);
				}
			}

			for (int i = 0; i < vertices.Count; i++)
			{
				triangles.Add(i);
			}


			//mesh.uv = Unwrapping.GeneratePerTriangleUV(mesh);

			return new MeshData(vertices, triangles);
		}

		private List<MeshGeneratorNode> GetSurfaceNodes(int[,,] nodeMatrix)
		{
			int length = nodeMatrix.GetLength(0);
			int width = nodeMatrix.GetLength(1);
			int height = nodeMatrix.GetLength(2);

			List<MeshGeneratorNode> nodes = new List<MeshGeneratorNode>();
			MeshGeneratorNode[,,] matrix = new MeshGeneratorNode[length, width, height];

			for (int i = 0; i < length - 1; i++)
			{
				for (int j = 0; j < width - 1; j++)
				{
					for (int k = 0; k < height - 1; k++)
					{
						MeshGeneratorNode node = GetSurfaceNode(nodeMatrix, i, j, k);

						matrix[i, j, k] = node;

						if (node == null)
							continue;

						if (i != 0)
						{
							MeshGeneratorNode prevNode = matrix[i - 1, j, k];
							prevNode?.CreateMutualLink(node);
						}
						if (j != 0)
						{
							MeshGeneratorNode prevNode = matrix[i, j - 1, k];
							prevNode?.CreateMutualLink(node);
						}
						if (k != 0)
						{
							MeshGeneratorNode prevNode = matrix[i, j, k - 1];
							prevNode?.CreateMutualLink(node);
						}

						nodes.Add(node);
					}
				}
			}

			return nodes;
		}

		private MeshGeneratorNode GetSurfaceNode(int[,,] nodeMatrix, int i0, int j0, int k0)
		{
			int node0 = nodeMatrix[i0, j0, k0];
			int node1 = nodeMatrix[i0 + 1, j0, k0];
			int node2 = nodeMatrix[i0 + 1, j0 + 1, k0];
			int node3 = nodeMatrix[i0, j0 + 1, k0];

			int node4 = nodeMatrix[i0, j0, k0 + 1];
			int node5 = nodeMatrix[i0 + 1, j0, k0 + 1];
			int node6 = nodeMatrix[i0 + 1, j0 + 1, k0 + 1];
			int node7 = nodeMatrix[i0, j0 + 1, k0 + 1];

			int nodeSum = node0 + node1 + node2 + node3 + node4 + node5 + node6 + node7;

			if (nodeSum == 0 || nodeSum == 8)
			{
				return null;
			}

			return new MeshGeneratorNode(new Vector3(i0, j0, k0), new Vector3Int(i0, j0, k0), nodeMatrix);
		}
	}
}