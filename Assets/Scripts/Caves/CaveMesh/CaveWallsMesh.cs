using System.Collections.Generic;
using System.Linq;
using Caves.Cells;
using UnityEngine;

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
			List<int> triangles = new List<int>();

			foreach (CaveWallsGroup wall in walls)
			{
				CellType[,,] cellMatrix = GetAlignedCellMatrix(wall, out Vector3Int matrixSize);

				for (int i = 0; i < matrixSize.x; i++)
				{
					for (int j = 0; j < matrixSize.y; j++)
					{
						for (int k = 0; k < matrixSize.z; k++)
						{
							int nodeConfiguration = CaveMeshData.GetNodeConfiguration(cellMatrix, i, j, k);

							foreach (Vector3 vertex in CaveMeshData.GetVertices(nodeConfiguration))
							{
								vertices.Add(vertex);

								triangles.Add(vertices.Count - 1);
							}
						}
					}
				}
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			return mesh;
		}

		private static CellType[,,] GetAlignedCellMatrix(CaveWallsGroup wall, out Vector3Int physicalMatrixSize)
		{
			Vector3Int minCoordinate = wall.CellChunkCoordinates[0];
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

			physicalMatrixSize = maxCoordinate - minCoordinate + Vector3Int.one;

			Vector3Int actualMatrixSize = physicalMatrixSize + Vector3Int.one;

			CellType[,,] cells = new CellType[actualMatrixSize.x, actualMatrixSize.y, actualMatrixSize.z];

			foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
			{
				Vector3Int matrixCoordinate = coordinate - minCoordinate;

				cells[matrixCoordinate.x, matrixCoordinate.y, matrixCoordinate.z] = CellType.Wall;
			}

			return cells;
		}

		/*private static CellType[,,] GetCellMatrixAlligned4x4x4(CaveWallsGroup wall, CaveSettings settings)
		{
			CellType[,,] cells = new CellType[settings.TerrainCubicSize.x, settings.TerrainCubicSize.y, settings.TerrainCubicSize.z];

			foreach (Vector3Int cell in wall.CellChunkCoordinates)
			{
				cells[cell.x, cell.y, cell.z] = CellType.Wall;
			}

			return cells;
		}*/
	}
}