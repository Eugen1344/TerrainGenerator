using System.Collections.Generic;
using Caves.Chunks;
using SimplexNoise;
using UnityEngine;
using Random = System.Random;

namespace Caves.Cells.SimplexNoise
{
	public abstract class ChunkCellData
	{
		public readonly CellSettings Settings;
		public readonly Vector2Int ChunkCoordinate;
		public readonly int ChunkSeed;

		public List<CellType[,,]> Iterations = new List<CellType[,,]>();

		protected CaveChunkManager _chunkManager;

		protected ChunkCellData(CellSettings settings, CaveChunkManager chunkManager, Vector2Int chunkCoordinate)
		{
			Debug.Log($"Created chunk {GetType()}");

			_chunkManager = chunkManager;

			Settings = settings;
			ChunkCoordinate = chunkCoordinate;
			ChunkSeed = Settings.GenerateSeed(Settings.Seed, chunkCoordinate);

			Noise.Seed = ChunkSeed;
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
