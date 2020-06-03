using System.Collections.Generic;
using System.Linq;
using Caves.Cells;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Caves.CaveMesh
{
	public class CaveWallsMesh
	{
		public Mesh Mesh;

		public CaveWallsMesh(Mesh mesh)
		{
			Mesh = mesh;
		}

		public static CaveWallsMesh GenerateWallMesh(List<CaveWallsGroup> walls, CaveMeshSettings settings)
		{
			Mesh mesh = GetMesh(walls, settings);

			return new CaveWallsMesh(mesh);
		}

		private static Mesh GetMesh(List<CaveWallsGroup> walls, CaveMeshSettings settings)
		{
			List<Vector3> vertices = new List<Vector3>();
			//List<Vector3> normals = new List<Vector3>();
			List<int> triangles = new List<int>();

			foreach (CaveWallsGroup wall in walls)
			{
				CellType[,,] cellMatrix = GetAlignedCellMatrix(wall, out Vector3Int matrixSize, out Vector3Int minCoordinate);

				for (int i = 0; i < matrixSize.x; i++)
				{
					for (int j = 0; j < matrixSize.y; j++)
					{
						for (int k = 0; k < matrixSize.z; k++)
						{
							int nodeConfiguration = CaveMeshData.GetNodeConfiguration(cellMatrix, i, j, k);

							foreach (Vector3 vertex in CaveMeshData.GetVertices(nodeConfiguration))
							{
								Vector3 vertexPosition = new Vector3((vertex.x + minCoordinate.x + i) * settings.CellSize.x, (vertex.y + minCoordinate.y + j) * settings.CellSize.y, (vertex.z + minCoordinate.z + k) * settings.CellSize.z);
								vertices.Add(vertexPosition);

								int triangleIndex = vertices.Count - 1;
								triangles.Add(triangleIndex);

								//normals.Add(vertex);
							}
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

		private static CellType[,,] GetAlignedCellMatrix(CaveWallsGroup wall, out Vector3Int physicalMatrixSize, out Vector3Int minCoordinate)
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

			physicalMatrixSize = maxCoordinate - minCoordinate + new Vector3Int(2, 2, 2);

			Vector3Int actualMatrixSize = physicalMatrixSize + Vector3Int.one;

			CellType[,,] cells = new CellType[actualMatrixSize.x, actualMatrixSize.y, actualMatrixSize.z];

			foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
			{
				Vector3Int matrixCoordinate = coordinate - minCoordinate + Vector3Int.one;

				cells[matrixCoordinate.x, matrixCoordinate.y, matrixCoordinate.z] = CellType.Wall;
			}

			minCoordinate -= Vector3Int.one; //TODO hack

			return cells;
		}
	}
}