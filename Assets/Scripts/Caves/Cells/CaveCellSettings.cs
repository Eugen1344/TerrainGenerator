using System;
using UnityEngine;

namespace Caves.Cells
{
	[Serializable]
	public struct CaveCellSettings
	{
		public Vector3Int TerrainCubicSize;
		public int MinCaveHeight;
		public bool RandomSeed;
		public int Seed;
		public int SwitchToHollowThreshold;
		public int SwitchToWallThreshold;
		[Range(0, 1)] public float RandomHollowCellsPercent;
		[Range(0, 1)] public float RandomHollowCellsPercentDecreasePerLevel;
		public int IterationCount;
		public int MinHollowGroupCubicSize;
		public bool GenerateTunnels;
		public int TunnelHeight;
		public int TunnelWidth;

		public void GenerateSeed(Vector2Int chunkPosition)
		{
			if (RandomSeed)
				Seed = Environment.TickCount;

			int chunkOffset = (chunkPosition.x << sizeof(short) * 8) | chunkPosition.y;

			Seed += chunkOffset;
		}

		public class InvalidCaveCellSettingsException : Exception
		{
			public InvalidCaveCellSettingsException(string message) : base(message)
			{
			}
		}
	}
}