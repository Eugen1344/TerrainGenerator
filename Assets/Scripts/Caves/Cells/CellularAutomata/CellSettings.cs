using System;
using UnityEngine;

namespace Caves.Cells.CellularAutomata
{
	[Serializable]
	public struct CellSettings
	{
		public Vector3Int TerrainCubicSize;
		public int MinCaveHeight;
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

		public int GenerateSeed(int baseSeed, Vector3Int chunkCoordinate)
		{
			ushort chunkCoordinateX = (ushort)chunkCoordinate.x;
			ushort chunkCoordinateY = (ushort)chunkCoordinate.y;
			int chunkOffset = chunkCoordinateX << (sizeof(ushort) * 8) | chunkCoordinateY;

			return baseSeed + chunkOffset;
		}

		public class InvalidCaveCellSettingsException : Exception
		{
			public InvalidCaveCellSettingsException(string message) : base(message)
			{
			}
		}
	}
}