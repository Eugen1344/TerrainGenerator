using System;
using UnityEngine;
using Random = System.Random;

public class CaveCellsGenerator : MonoBehaviour
{
	[Serializable]
	public struct CaveSettings
	{
		public Vector2Int TerrainCubicSize;
		[Range(0, 1)] public float RandomActiveCellsPercent;
		public int Seed;
	}

	public static int[,] GetInitialState(CaveSettings settings)
	{
		Random rand;

		if (settings.Seed == 0)
			rand = new Random();
		else
			rand = new Random(settings.Seed);

		int[,] result = new int[settings.TerrainCubicSize.x, settings.TerrainCubicSize.y];

		for (int i = 0; i < result.GetLength(0); i++)
		{
			for (int j = 0; j < result.GetLength(1); j++)
			{
				if (rand.NextDouble() <= settings.RandomActiveCellsPercent)
					result[i, j] = 1;
				else
					result[i, j] = 0;
			}
		}

		return result;
	}

	public static int[,] GetSimulatedCaves(int[,] initialState)
	{
		int height = initialState.GetLength(0);
		int width = initialState.GetLength(1);

		int[,] result = (int[,])initialState.Clone();

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				int nearbyInactiveCellCount = GetNearbyInactiveCellCount(result, i, j);

				if (nearbyInactiveCellCount > 4)
					result[i, j] = 0;
				else if (nearbyInactiveCellCount < 4)
					result[i, j] = 1;
			}
		}

		return result;
	}

	private static int GetNearbyInactiveCellCount(int[,] cells, int i, int j, int radius = 1)
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

				if (!IsCellActive(cells[y, x]))
					count++;
			}
		}

		return count;
	}

	public static bool IsCellActive(int cell)
	{
		return cell != 0;
	}
}