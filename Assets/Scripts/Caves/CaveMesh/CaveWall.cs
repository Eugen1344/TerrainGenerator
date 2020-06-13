using System;
using Caves.Cells;
using MeshGenerators;
using UnityEngine;

namespace Caves.CaveMesh
{
	public class CaveWall : MonoBehaviour
	{
		public MeshGeneratorSettings MeshSettings;
		public MeshFilter MeshFilter;
		public MeshCollider MeshCollider;

		private MeshGenerator _meshGenerator;
		private CaveWallGroup _wall;
		private CaveChunk _chunk;

		public void Generate(CaveWallGroup wall, CaveChunk chunk)
		{
			_wall = wall;
			_chunk = chunk;

			SetMarchingCubesMeshGenerator();

			Mesh mesh = GenerateMesh(out Vector3Int minCoordinate);

			transform.localPosition += minCoordinate * _chunk.CellSize;

			MeshFilter.sharedMesh = mesh;
			MeshCollider.sharedMesh = mesh;
		}

		private void SetMarchingCubesMeshGenerator() //TODO temp, move
		{
			_meshGenerator = new MarchingCubesMeshGenerator(MeshSettings);
		}

		private Mesh GenerateMesh(out Vector3Int minCoordinate)
		{
			int[,,] nodeMatrix = GetAlignedNodeMatrix(_wall, out minCoordinate);

			return _meshGenerator.Generate(nodeMatrix, _chunk.CellSize);
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

		private static int[,,] GetAlignedNodeMatrix(CaveWallGroup wall, out Vector3Int minCoordinate)
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
				if (cell.z > maxCoordinate.z)
					maxCoordinate.z = cell.z;
			}

			Vector3Int actualMatrixSize = (maxCoordinate + Vector3Int.one) - minCoordinate + new Vector3Int(2, 2, 2);

			int[,,] nodes = new int[actualMatrixSize.x, actualMatrixSize.y, actualMatrixSize.z];

			foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
			{
				Vector3Int matrixCoordinate = coordinate - minCoordinate + Vector3Int.one;

				nodes[matrixCoordinate.x, matrixCoordinate.y, matrixCoordinate.z] = 1;
			}

			//minCoordinate -= Vector3Int.one; //TODO hack

			return nodes;
		}
	}
}