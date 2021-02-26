using Caves.Cells;
using Caves.Chunks;
using MeshGenerators;
using MeshGenerators.SurfaceNets;
using UnityEngine;

namespace Caves.CaveMesh
{
	public class CaveWall : MonoBehaviour
	{
		public MeshFilter MeshFilter;
		public MeshCollider MeshCollider;

		public CaveChunk Chunk { get; private set; }
		public Vector3Int MinCoordinate { get; private set; }

		private MeshGenerator _meshGenerator;
		private WallGroup _wall;
		private MeshData _data;

		private void Start()
		{
			transform.localPosition += MinCoordinate * _meshGenerator.Settings.GridSize;

			Mesh mesh = _meshGenerator.CreateMesh(_data);
			MeshFilter.sharedMesh = mesh;
			MeshCollider.sharedMesh = mesh;
		}

		public void Generate(WallGroup wall, CaveChunk chunk)
		{
			_wall = wall;
			Chunk = chunk;

			SetMeshGenerator();

			_data = GenerateMeshData(out Vector3Int minCoordinate);
			MinCoordinate = minCoordinate;
		}

		private void SetMeshGenerator() //TODO temp, move
		{
			_meshGenerator = new SurfaceNetsMeshGenerator(Chunk.ChunkManager.MeshSettings, this);
		}

		private MeshData GenerateMeshData(out Vector3Int minCoordinate)
		{
			int[,,] nodeMatrix = GetAlignedNodeMatrix(_wall, out minCoordinate);

			return _meshGenerator.Generate(nodeMatrix);
		}

		private static Vector2[] CalculateUVs(Vector3[] vertices, Vector3 size)
		{
			Vector2[] uvs = new Vector2[vertices.Length];

			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vertex = vertices[i];

				uvs[i] = new Vector2(vertex.x / size.x, vertex.z / size.z);
			}

			return uvs;
		}

		private int[,,] GetAlignedNodeMatrix(WallGroup wall, out Vector3Int minCoordinate)
		{
			minCoordinate = wall.CellChunkCoordinates[0];
			Vector3Int maxCoordinate = wall.CellChunkCoordinates[0];

			foreach (Vector3Int cell in wall.CellChunkCoordinates)
			{
				if (cell.x < minCoordinate.x)
					minCoordinate.x = cell.x;
				else if (cell.x > maxCoordinate.x)
					maxCoordinate.x = cell.x;

				if (cell.y < minCoordinate.y)
					minCoordinate.y = cell.y;
				else if (cell.y > maxCoordinate.y)
					maxCoordinate.y = cell.y;

				if (cell.z < minCoordinate.z)
					minCoordinate.z = cell.z;
				else if (cell.z > maxCoordinate.z)
					maxCoordinate.z = cell.z;
			}

			Vector3Int actualMatrixSize = (maxCoordinate + Vector3Int.one) - minCoordinate + new Vector3Int(2, 2, 2);

			int[,,] nodes = new int[actualMatrixSize.x, actualMatrixSize.y, actualMatrixSize.z];

			for (int i = 0; i < actualMatrixSize.x; i++)
			{
				for (int j = 0; j < actualMatrixSize.y; j++)
				{
					for (int k = 0; k < actualMatrixSize.z; k++)
					{
						if ((i == 0 || j == 0 || k == 0 ||
							i == actualMatrixSize.x - 1 || j == actualMatrixSize.y - 1 || k == actualMatrixSize.z - 1))
						{
							Vector3Int matrixCoordinate = new Vector3Int(i, j, k);
							Vector3Int chunkCoordinate = matrixCoordinate + minCoordinate - Vector3Int.one;

							if (Chunk.IsInsideChunk(chunkCoordinate))
								continue;

							nodes[i, j, k] = Chunk.GetCellFromAllChunks(chunkCoordinate) == CellType.Wall ? 1 : 0;
						}
					}
				}
			}

			foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
			{
				Vector3Int matrixCoordinate = coordinate - minCoordinate + Vector3Int.one;

				nodes[matrixCoordinate.x, matrixCoordinate.y, matrixCoordinate.z] = 1;
			}

			return nodes;
		}
	}
}