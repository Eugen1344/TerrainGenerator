using System.Collections.Generic;
using Caves.CaveMesh;
using Caves.Chunks;
using UnityEngine;

namespace MeshGenerators.SurfaceNets
{
	public class SurfaceNetsMeshGenerator : MeshGenerator
	{
		public SurfaceNetsMeshGenerator(MeshGeneratorSettings settings, CaveWall wall) : base(settings, wall)
		{
		}

		public override MeshData Generate(int[,,] matrix)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			List<MeshGeneratorNode> surfaceNodes = GetSurfaceNodes(matrix);

			for (int i = 0; i < Settings.SmoothIterationCount; i++)
			{
				foreach (MeshGeneratorNode node in surfaceNodes)
				{
					node.PlaceEquidistant();
				}
			}

			foreach (MeshGeneratorNode node in surfaceNodes)
			{
				Vector3Int chunkPosition = node.MatrixPosition + Wall.MinCoordinate;

				if (IsOnChunkEdge(chunkPosition))
				{
					if (!Wall.Chunk.EdgeNodes.ContainsKey(chunkPosition))
						Wall.Chunk.EdgeNodes.Add(chunkPosition, node);
				}

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

		private bool IsOnChunkEdge(Vector3Int coordinate)
		{
			Vector3Int chunkSize = Wall.Chunk.Settings.ChunkCubicSize;

			return coordinate.x == 0 || coordinate.y == 0 || coordinate.z == 0 ||
				   coordinate.x == chunkSize.x - 1 || coordinate.y == chunkSize.y - 1 || coordinate.z == chunkSize.z - 1;
		}

		private List<MeshGeneratorNode> GetSurfaceNodes(int[,,] matrix)
		{
			int length = matrix.GetLength(0) + 1;
			int width = matrix.GetLength(1) + 1;
			int height = matrix.GetLength(2) + 1;

			List<MeshGeneratorNode> nodes = new List<MeshGeneratorNode>();
			MeshGeneratorNode[,,] nodeMatrix = new MeshGeneratorNode[length, width, height];

			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < height; k++)
					{
						MeshGeneratorNode node = null;

						if (i == 0)
							node = GetNearbyChunkNode(i, j, k, new Vector3Int(-1, 0, 0), matrix);
						else if (i == length - 1)
							node = GetNearbyChunkNode(i, j, k, new Vector3Int(1, 0, 0), matrix);
						else if (j == 0)
							node = GetNearbyChunkNode(i, j, k, new Vector3Int(0, -1, 0), matrix);
						else if (j == width - 1)
							node = GetNearbyChunkNode(i, j, k, new Vector3Int(0, 1, 0), matrix);
						else if (k == 0)
							node = GetNearbyChunkNode(i, j, k, new Vector3Int(0, 0, -1), matrix);
						else if (k == height - 1)
							node = GetNearbyChunkNode(i, j, k, new Vector3Int(0, 0, 1), matrix);
						else
							node = GetSurfaceNode(matrix, i, j, k);

						nodeMatrix[i, j, k] = node;

						if (node == null)
							continue;

						if (i != 0)
						{
							MeshGeneratorNode prevNode = nodeMatrix[i - 1, j, k];
							prevNode?.CreateMutualLink(node);
						}

						if (j != 0)
						{
							MeshGeneratorNode prevNode = nodeMatrix[i, j - 1, k];
							prevNode?.CreateMutualLink(node);
						}
						if (k != 0)
						{
							MeshGeneratorNode prevNode = nodeMatrix[i, j, k - 1];
							prevNode?.CreateMutualLink(node);
						}

						nodes.Add(node);
					}
				}
			}

			return nodes;
		}

		private MeshGeneratorNode GetNearbyChunkNode(int i, int j, int k, Vector3Int offset, int[,,] matrix)
		{
			MeshGeneratorNode node = null;

			Vector3Int nearbyChunkPosition = new Vector3Int(Wall.Chunk.ChunkCoordinate.x, Wall.Chunk.ChunkCoordinate.y, Wall.Chunk.ChunkCoordinate.z) + offset;

			if (Wall.Chunk.ChunkManager.GeneratedChunks.TryGetValue(nearbyChunkPosition, out CaveChunk nearbyChunk))
			{
				if (nearbyChunk.IsFinalized)
				{
					Vector3Int nearbyNodeCoordinate = new Vector3Int(i + Wall.MinCoordinate.x, j + Wall.MinCoordinate.y, k + Wall.MinCoordinate.z);

					if (offset.x == -1)
						nearbyNodeCoordinate.x = Wall.Chunk.Settings.ChunkCubicSize.x - 1;
					else if (offset.x == 1)
						nearbyNodeCoordinate.x = 0;
					else if (offset.y == -1)
						nearbyNodeCoordinate.y = Wall.Chunk.Settings.ChunkCubicSize.y - 1;
					else if (offset.y == 1)
						nearbyNodeCoordinate.y = 0;
					else if (offset.z == -1)
						nearbyNodeCoordinate.z = Wall.Chunk.Settings.ChunkCubicSize.z - 1;
					else if (offset.z == 1)
						nearbyNodeCoordinate.z = 0;

					if (nearbyChunk.EdgeNodes.TryGetValue(nearbyNodeCoordinate, out MeshGeneratorNode nearbyNode))
					{
						node = new MeshGeneratorNode(nearbyNode.Position, new Vector3Int(i, j, k), matrix) { IsStatic = true };
					}
				}
			}

			return node;
		}

		private MeshGeneratorNode GetSurfaceNode(int[,,] matrix, int i0, int j0, int k0)
		{
			Vector3Int nodeMatrixPosition = new Vector3Int(i0 - 1, j0 - 1, k0 - 1);

			int node0 = matrix[nodeMatrixPosition.x, nodeMatrixPosition.y, nodeMatrixPosition.z];
			int node1 = matrix[nodeMatrixPosition.x + 1, nodeMatrixPosition.y, nodeMatrixPosition.z];
			int node2 = matrix[nodeMatrixPosition.x + 1, nodeMatrixPosition.y + 1, nodeMatrixPosition.z];
			int node3 = matrix[nodeMatrixPosition.x, nodeMatrixPosition.y + 1, nodeMatrixPosition.z];

			int node4 = matrix[nodeMatrixPosition.x, nodeMatrixPosition.y, nodeMatrixPosition.z + 1];
			int node5 = matrix[nodeMatrixPosition.x + 1, nodeMatrixPosition.y, nodeMatrixPosition.z + 1];
			int node6 = matrix[nodeMatrixPosition.x + 1, nodeMatrixPosition.y + 1, nodeMatrixPosition.z + 1];
			int node7 = matrix[nodeMatrixPosition.x, nodeMatrixPosition.y + 1, nodeMatrixPosition.z + 1];

			int nodeSum = node0 + node1 + node2 + node3 + node4 + node5 + node6 + node7;

			if (nodeSum == 0 || nodeSum == 8)
			{
				return null;
			}

			return new MeshGeneratorNode(new Vector3(i0, j0, k0), new Vector3Int(i0, j0, k0), matrix);
		}
	}
}