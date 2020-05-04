using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class CaveCellsGenerator
{
	public static CaveCellChunk GenerateCaveChunk(CaveSettings settings)
	{
		if (settings.RandomSeed)
			settings.Seed = Environment.TickCount;

		int levelsCount = settings.TerrainCubicSize.z;
		//float randomHollowCellsPercentDecreasePerLevel = levelsCount == 1 ? 0 : settings.RandomHollowCellsPercent / (levelsCount - 1);

		CellType[,,] resultCells = new CellType[settings.TerrainCubicSize.x, settings.TerrainCubicSize.y, settings.TerrainCubicSize.z];

		for (int i = 0; i < levelsCount; i++)
		{
			CaveSettings levelSettings = settings;

			levelSettings.RandomHollowCellsPercent = settings.RandomHollowCellsPercent - i * settings.RandomHollowCellsPercentDecreasePerLevel;

			CellType[,] cells = GetInitialState(levelSettings);
			cells = GetSimulatedCells(cells, levelSettings);

			AppendCellsToGrid(ref resultCells, cells, i);
		}

		List<CaveHollowGroup> hollows = GetCellGroups<CaveHollowGroup>(resultCells, CellType.Hollow);
		FilterHollowGroups(resultCells, hollows, settings.MinHollowGroupCubicSize);

		hollows = GetCellGroups<CaveHollowGroup>(resultCells, CellType.Hollow);

		List<CaveTunnel> tunnels = CaveConnector.ConnectCaves(ref resultCells, hollows, settings);

		List<CaveWallsGroup> walls = GetCellGroups<CaveWallsGroup>(resultCells, CellType.Wall);

		return new CaveCellChunk(hollows, walls, tunnels);
	}

	private static List<T> GetCellGroups<T>(CellType[,,] cells, CellType searchedCellType) where T : CaveGroup
	{
		int length = cells.GetLength(0);
		int width = cells.GetLength(1);
		int height = cells.GetLength(2);

		List<T> result = new List<T>();

		bool[,,] markedCells = new bool[length, width, height];

		Queue<Vector3Int> cellsToSearch = new Queue<Vector3Int>();

		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < height; k++)
				{
					if (cells[i, j, k] == searchedCellType && !markedCells[i, j, k])
					{
						List<Vector3Int> foundCells = new List<Vector3Int>();

						cellsToSearch.Enqueue(new Vector3Int(i, j, k));
						markedCells[i, j, k] = true;

						while (cellsToSearch.Count != 0)
						{
							Vector3Int searchCoreCell = cellsToSearch.Dequeue();

							foundCells.Add(searchCoreCell);

							int startI = searchCoreCell.x - 1 < 0 ? 0 : searchCoreCell.x - 1;
							int startJ = searchCoreCell.y - 1 < 0 ? 0 : searchCoreCell.y - 1;
							int endI = searchCoreCell.x + 1 >= length ? length - 1 : searchCoreCell.x + 1;
							int endJ = searchCoreCell.y + 1 >= width ? width - 1 : searchCoreCell.y + 1;
							int startK = searchCoreCell.z - 1 < 0 ? 0 : searchCoreCell.z - 1;
							int endK = searchCoreCell.z + 1 >= height ? height - 1 : searchCoreCell.z + 1;

							for (int i1 = startI; i1 <= endI; i1++)
							{
								for (int j1 = startJ; j1 <= endJ; j1++)
								{
									for (int k1 = startK; k1 <= endK; k1++)
									{
										if ((i1 == searchCoreCell.x && j1 == searchCoreCell.y && k1 == searchCoreCell.z) ||
											(i1 != searchCoreCell.x && j1 != searchCoreCell.y))
											continue;

										if (cells[i1, j1, k1] == searchedCellType && !markedCells[i1, j1, k1])
										{
											Vector3Int newSearchCoreCell = new Vector3Int(i1, j1, k1);

											cellsToSearch.Enqueue(newSearchCoreCell);
											markedCells[i1, j1, k1] = true;
										}
									}
								}
							}
						}

						//int markedCount = markedCells.Cast<bool>().Count(b => b);

						CaveGroup newGroup = CaveGroup.GetCaveGroup(searchedCellType, foundCells);
						result.Add((T)newGroup);
					}
				}
			}
		}

		return result;
	}

	private static void FilterHollowGroups(CellType[,,] resultCells, List<CaveHollowGroup> hollows, int minHollowGroupSize)
	{
		foreach (CaveHollowGroup group in hollows)
		{
			if (group.CellCount < minHollowGroupSize)
			{
				foreach (Vector3Int cell in group.CellChunkCoordinates)
				{
					resultCells[cell.x, cell.y, cell.z] = CellType.Wall;
				}
			}
		}
	}

	private static void AppendCellsToGrid(ref CellType[,,] grid, CellType[,] cells, int level)
	{
		int length = grid.GetLength(0);
		int width = grid.GetLength(1);

		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < width; j++)
			{
				grid[i, j, level] = cells[i, j];
			}
		}
	}

	public static CellType[,] GetInitialState(CaveSettings settings)
	{
		Random rand = new Random(settings.Seed);

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

	public static CellType[,] GetSimulatedCells(CellType[,] initialState, CaveSettings settings)
	{
		int height = initialState.GetLength(0);
		int width = initialState.GetLength(1);

		CellType[,] result = initialState;

		for (int iteration = 0; iteration < settings.IterationCount; iteration++)
		{
			result = (CellType[,])initialState.Clone();

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int nearbyWallCellCount = GetNearbyWallCellCount(initialState, i, j);

					if (nearbyWallCellCount > settings.SwitchToWallThreshold)
						result[i, j] = CellType.Wall;
					else if (nearbyWallCellCount < settings.SwitchToHollowThreshold)
						result[i, j] = CellType.Hollow;
				}
			}

			initialState = result;
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