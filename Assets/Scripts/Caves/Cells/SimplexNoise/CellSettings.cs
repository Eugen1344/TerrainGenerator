using System;
using UnityEngine;

namespace Caves.Cells.SimplexNoise
{
	[Serializable]
	public struct CellSettings
	{
		public Vector3Int TerrainCubicSize;
		public int Seed;
		[Range(0, 1)] public float RandomHollowCellsPercent;
		public bool GenerateTunnels;
		public int TunnelHeight;
		public int TunnelWidth;

		public int GenerateSeed(int baseSeed, Vector2Int chunkCoordinate)
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