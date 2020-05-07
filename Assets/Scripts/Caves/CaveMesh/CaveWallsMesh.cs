using System.Collections.Generic;
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

		public static CaveWallsMesh GenerateWallMesh(List<CaveWallsGroup> walls)
		{
			Mesh mesh = GetMesh(walls);

			return new CaveWallsMesh(mesh);
		}

		private static Mesh GetMesh(List<CaveWallsGroup> walls)
		{
			foreach (CaveWallsGroup wall in walls)
			{
				CellType[,,] cellMatrix = GetCellMatrixAlligned4X4X4(wall);

				int length = cellMatrix.GetLength(0);
				int width = cellMatrix.GetLength(1);
				int height = cellMatrix.GetLength(2);

				for (int i = 0; i < length; i++)
				{
					for (int j = 0; j < width; j++)
					{
						for (int k = 0; k < height; k++)
						{
							int nodeConfiguration = GetNodeConfiguration(cellMatrix, i, j, k);

							Cave
						}
					}
				}
			}

			Mesh mesh = new Mesh();
			//mesh.vertices
			return mesh;
		}

		private static int GetNodeConfiguration(CellType[,,] alignedMatrix, int i0, int j0, int k0)
		{
			int node0 = alignedMatrix[i0, j0, k0] == CellType.Wall ? 1 : 0;
			int node1 = alignedMatrix[i0 + 1, j0, k0] == CellType.Wall ? 1 : 0;
			int node2 = alignedMatrix[i0 + 1, j0 + 1, k0] == CellType.Wall ? 1 : 0;
			int node3 = alignedMatrix[i0, j0 + 1, k0] == CellType.Wall ? 1 : 0;

			int node4 = alignedMatrix[i0, j0, k0 + 1] == CellType.Wall ? 1 : 0;
			int node5 = alignedMatrix[i0 + 1, j0, k0 + 1] == CellType.Wall ? 1 : 0;
			int node6 = alignedMatrix[i0 + 1, j0 + 1, k0 + 1] == CellType.Wall ? 1 : 0;
			int node7 = alignedMatrix[i0, j0 + 1, k0 + 1] == CellType.Wall ? 1 : 0;

			return node0 | (node1 << 1) | (node2 << 2) | (node3 << 3) |
				   (node4 << 4) | (node5 << 5) | (node6 << 6) | (node7 << 7);
		}

		private static CellType[,,] GetCellMatrixAligned2(CaveWallsGroup wall)
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

			Vector3Int matrixSize = maxCoordinate - minCoordinate + Vector3Int.one;

			if (matrixSize.x % 2 != 0) //TODO do better solution
			{
				matrixSize.x += 1;
			}
			if (matrixSize.y % 2 != 0)
			{
				matrixSize.y += 1;
			}
			if (matrixSize.z % 2 != 0)
			{
				matrixSize.z += 1;
			}

			CellType[,,] cells = new CellType[matrixSize.x, matrixSize.y, matrixSize.z];

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