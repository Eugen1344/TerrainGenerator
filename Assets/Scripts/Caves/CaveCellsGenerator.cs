using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public partial class CaveCellsGenerator
{
	[Serializable]
	public struct CaveSettings
	{
		public Vector2Int TerrainCubicSize;
		[Range(0, 1)] public float RandomHollowCellsPercent;
		public int Seed;
	}

	public static CaveChunk GenerateCaveChunk(CaveSettings settings)
	{
		CellType[,] cells = GetInitialState(settings);
		cells = GetSimulatedCells(cells);

		return GetCaveChunk(cells);
	}

	private static CaveChunk GetCaveChunk(CellType[,] cells)
	{
		List<CaveHollowGroup> hollows = GetCellGroups<CaveHollowGroup>(cells, CellType.Hollow).ToList();
		List<CaveWallsGroup> walls = GetCellGroups<CaveWallsGroup>(cells, CellType.Wall).ToList();

		return new CaveChunk(hollows, walls);
	}

	private static List<T> GetCellGroups<T>(CellType[,] cells, CellType searchedCellType) where T : CaveGroup
	{
		int height = cells.GetLength(0);
		int width = cells.GetLength(1);

		List<T> result = new List<T>();

		bool[,] markedCells = new bool[height, width];

		Queue<Vector2Int> cellsToSearch = new Queue<Vector2Int>();

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (cells[i, j] == searchedCellType && !markedCells[i, j])
				{
					List<Vector2Int> foundCells = new List<Vector2Int>();

					cellsToSearch.Enqueue(new Vector2Int(i, j));
					markedCells[i, j] = true;

					while (cellsToSearch.Count != 0)
					{
						Vector2Int searchCoreCell = cellsToSearch.Dequeue();

						foundCells.Add(searchCoreCell);

						int startI = searchCoreCell.x - 1 < 0 ? 0 : searchCoreCell.x - 1;
						int startJ = searchCoreCell.y - 1 < 0 ? 0 : searchCoreCell.y - 1;
						int endI = searchCoreCell.x + 1 >= height ? height - 1 : searchCoreCell.x + 1;
						int endJ = searchCoreCell.y + 1 >= width ? width - 1 : searchCoreCell.y + 1;

						for (int i1 = startI; i1 <= endI; i1++)
						{
							for (int j1 = startJ; j1 <= endJ; j1++)
							{
								if (i1 == searchCoreCell.x && j1 == searchCoreCell.y || (i1 != searchCoreCell.x && j1 != searchCoreCell.y))
									continue;

								if (cells[i1, j1] == searchedCellType && !markedCells[i1, j1])
								{
									Vector2Int newSearchCoreCell = new Vector2Int(i1, j1);

									cellsToSearch.Enqueue(newSearchCoreCell);
									markedCells[i1, j1] = true;
								}
							}
						}
					}

					CaveGroup newGroup = CaveGroup.GetCaveGroup(searchedCellType, foundCells);
					result.Add((T)newGroup);
				}
			}
		}

		return result;
	}

	public static CellType[,] GetInitialState(CaveSettings settings)
	{
		Random rand;

		if (settings.Seed == 0)
			rand = new Random();
		else
			rand = new Random(settings.Seed);

		CellType[,] result = new CellType[settings.TerrainCubicSize.x, settings.TerrainCubicSize.y];

		for (int i = 0; i < result.GetLength(0); i++)
		{
			for (int j = 0; j < result.GetLength(1); j++)
			{
				if (rand.NextDouble() <= settings.RandomHollowCellsPercent)
					result[i, j] = CellType.Hollow;
				else
					result[i, j] = CellType.Wall;
			}
		}

		return result;
	}

	public static CellType[,] GetSimulatedCells(CellType[,] initialState)
	{
		int height = initialState.GetLength(0);
		int width = initialState.GetLength(1);

		CellType[,] result = (CellType[,])initialState.Clone();

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				int nearbyWallCellCount = GetNearbyWallCellCount(result, i, j);

				if (nearbyWallCellCount > 4)
					result[i, j] = CellType.Wall;
				else if (nearbyWallCellCount < 4)
					result[i, j] = CellType.Hollow;
			}
		}

		return result;
	}

	private static int GetNearbyWallCellCount(CellType[,] cells, int i, int j, int radius = 1)
	{
		int height = cells.GetLength(0);
		int width = cells.GetLength(1);

		int startI = i - radius < 0 ? 0 : i - radius;
		int startJ = j - radius < 0 ? 0 : j - radius;
		int endI = i + radius >= height ? height - 1 : i + radius;
		int endJ = j + radius >= width ? width - 1 : j + radius;

		int count = 0;

		for (int y = startI; y <= endI; y++)
		{
			for (int x = startJ; x <= endJ; x++)
			{
				if (y == i && x == j)
					continue;

				if (IsWallCell(cells[y, x]))
					count++;
			}
		}

		return count;
	}

	public static bool IsHollowCell(CellType cell)
	{
		return cell == CellType.Hollow;
	}

	public static bool IsWallCell(CellType cell)
	{
		return cell == CellType.Wall;
	}
}