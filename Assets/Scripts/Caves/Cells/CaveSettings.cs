using System;
using UnityEngine;

namespace Caves.Cells
{
	[Serializable]
	public struct CaveSettings
	{
		public Vector3Int TerrainCubicSize;
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

		public class InvalidCaveSettingsException : Exception
		{
			public InvalidCaveSettingsException(string message) : base(message)
			{
			}
		}
	}
}