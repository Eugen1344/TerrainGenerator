using System.Collections.Generic;
using System.Linq;
using Caves.Cells;
using PolygonGenerators;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Caves.CaveMesh
{
	public class CaveWallsMesh
	{
		public Mesh Mesh;

		private MeshGenerator _meshGenerator;

		public CaveWallsMesh(MeshGenerator meshGenerator)
		{
			_meshGenerator = meshGenerator;
		}

		public void GenerateWallMesh(List<CaveWallsGroup> walls)
		{
			Mesh = GetMesh(walls);
		}

		private Mesh GetMesh(List<CaveWallsGroup> walls)
		{
			foreach (CaveWallsGroup wall in walls)
			{
				int[,,] nodeMatrix = GetAlignedNodeMatrix(wall, out Vector3Int matrixSize, out Vector3Int minCoordinate);

				_meshGenerator.Generate(nodeMatrix);
			}

			return mesh;
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

		private static int[,,] GetAlignedNodeMatrix(CaveWallsGroup wall, out Vector3Int minCoordinate)
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

			minCoordinate -= Vector3Int.one; //TODO hack

			return nodes;
		}
	}
}