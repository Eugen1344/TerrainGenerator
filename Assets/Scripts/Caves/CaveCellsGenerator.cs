using System;
using Random = System.Random;

public class CaveCellsGenerator
{
	public static CaveChunk GenerateCaveChunk(CaveSettings settings)
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

			AppendCellsToGrid(resultCells, cells, i);
		}

		return new CaveChunk(resultCells);
	}

	private static void AppendCellsToGrid(CellType[,,] grid, CellType[,] cells, int level)
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