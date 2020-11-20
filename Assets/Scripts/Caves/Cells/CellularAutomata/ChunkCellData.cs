using System.Collections.Generic;
using Caves.Chunks;
using UnityEngine;
using Random = System.Random;

namespace Caves.Cells.CellularAutomata
{
	public abstract class ChunkCellData
	{
		public readonly CellSettings Settings;
		public readonly Vector3Int ChunkCoordinate;
		public readonly int ChunkSeed;

		public List<CellType[,,]> Iterations = new List<CellType[,,]>();
		public CellType[,,] FinalIteration;

		protected CaveChunkManager _chunkManager;

		protected ChunkCellData(CellSettings settings, CaveChunkManager chunkManager, Vector3Int chunkCoordinate)
		{
			Debug.Log($"Created chunk {GetType()}");

			_chunkManager = chunkManager;

			Settings = settings;
			ChunkCoordinate = chunkCoordinate;
			ChunkSeed = Settings.GenerateSeed(Settings.Seed, chunkCoordinate);
		}

		public void GenerateIterations(ChunkCellData[,] nearbyChunks, int lastIteration)
		{
			if (Iterations.Count == 0)
			{
				Debug.Log($"Generated chunk {GetType()} iteration 0");

				CellType[,,] initialState = GetInitialState();

				Iterations.Add(initialState);
			}

			for (int i = Iterations.Count - 1; i < lastIteration; i++)
			{
				Debug.Log($"Generated chunk {GetType()} iteration {i + 1}");

				CellType[,,] previousIteration = Iterations[i];
				CellType[,,] nextIteration = GetNextIteration(previousIteration, nearbyChunks, i);

				Iterations.Add(nextIteration);
			}
		}

		public CellType[,,] GetInitialState()
		{
			CellType[,,] result = new CellType[Settings.TerrainCubicSize.x, Settings.TerrainCubicSize.y, Settings.TerrainCubicSize.z];

			for (int k = 0; k < Settings.TerrainCubicSize.z; k++)
			{
				float levelRandomHollowCellsPercent;

				if (k >= Settings.MinCaveHeight)
					levelRandomHollowCellsPercent = Settings.RandomHollowCellsPercent - (k - Settings.MinCaveHeight + 1) * Settings.RandomHollowCellsPercentDecreasePerLevel;
				else
					levelRandomHollowCellsPercent = Settings.RandomHollowCellsPercent;

				Random rand = new Random(ChunkSeed);

				for (int i = 0; i < Settings.TerrainCubicSize.x; i++)
				{
					for (int j = 0; j < Settings.TerrainCubicSize.y; j++)
					{
						if (rand.NextDouble() <= levelRandomHollowCellsPercent)
							result[i, j, k] = CellType.Hollow;
						else
							result[i, j, k] = CellType.Wall;
					}
				}
			}

			return result;
		}

		public CellType[,,] GetNextIteration(CellType[,,] previousState, ChunkCellData[,] nearbyChunks, int previousIteration)
		{
			int length = previousState.GetLength(0);
			int width = previousState.GetLength(1);
			int height = previousState.GetLength(2);

			CellType[,,] simulatedState = new CellType[length, width, height];

			for (int k = 0; k < height; k++)
			{
				for (int i = 0; i < length; i++)
				{
					for (int j = 0; j < width; j++)
					{
						int nearbyWallCellCount = GetNearbyWallCellCount(previousState, nearbyChunks, previousIteration, i, j, k);

						if (nearbyWallCellCount > Settings.SwitchToWallThreshold)
							simulatedState[i, j, k] = CellType.Wall;
						else if (nearbyWallCellCount < Settings.SwitchToHollowThreshold)
							simulatedState[i, j, k] = CellType.Hollow;
						else
							simulatedState[i, j, k] = previousState[i, j, k];
					}
				}
			}

			return simulatedState;
		}

		protected int GetNearbyWallCellCount(CellType[,,] cells, ChunkCellData[,] nearbyChunks, int previousIteration, int i, int j, int k, int radius = 1)
		{
			int startI = i - radius;
			int startJ = j - radius;
			int endI = i + radius;
			int endJ = j + radius;

			int count = 0;

			for (int y = startI; y <= endI; y++)
			{
				for (int x = startJ; x <= endJ; x++)
				{
					if (y == i && x == j)
						continue;

					CellType cell = GetCellFromNearbyChunks(cells, nearbyChunks, previousIteration, y, x, k);

					if (IsWallCell(cell))
						count++;
				}
			}

			return count;
		}

		private CellType GetCellFromNearbyChunks(CellType[,,] cells, ChunkCellData[,] nearbyChunks, int previousIteration, int i, int j, int k)
		{
			int height = cells.GetLength(0);
			int width = cells.GetLength(1);

			int chosenMatrixI = (i + height) / height;
			int chosenMatrixJ = (j + width) / width;

			ChunkCellData chosenChunk = nearbyChunks[chosenMatrixI, chosenMatrixJ];

			if (chosenChunk == null)
				return CellType.Hollow;

			int relativeI = i - (chosenMatrixI - 1) * height;
			int relativeJ = j - (chosenMatrixJ - 1) * width;

			CellType[,,] chosenMatrix = chosenChunk.Iterations[previousIteration];

			return chosenMatrix[relativeI, relativeJ, k];
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
}
