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
		public int TunnelHeight;
		public int TunnelWidth;

		public class InvalidCaveCellSettingsException : Exception
		{
			public InvalidCaveCellSettingsException(string message) : base(message)
			{
			}
		}
	}
}