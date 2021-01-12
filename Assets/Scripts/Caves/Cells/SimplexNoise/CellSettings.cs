using System;
using UnityEngine;

namespace Caves.Cells.SimplexNoise
{
	[Serializable]
	public struct CellSettings
	{
		public Vector3Int ChunkCubicSize;
		public int Seed;
		[Range(0, 1)] public float RandomHollowCellsPercent;
		public int MinCaveSize;
		public float NoiseScale;
		public float RandomHollowCellsPercentDecreasePerPixel;
		public Vector3Int CentralDecreasePoint;

		public bool GenerateTunnels;
		public int TunnelHeight;
		public int TunnelWidth;

		public int GenerateSeed(int baseSeed, Vector3Int chunkCoordinate)
		{
			byte chunkCoordinateX = (byte)chunkCoordinate.x; //TODO change to Random(long)
			byte chunkCoordinateY = (byte)chunkCoordinate.y;
			byte chunkCoordinateZ = (byte)chunkCoordinate.z;

			int chunkOffset = (chunkCoordinateX << (sizeof(byte) * 2 * 8)) | (chunkCoordinateY << (sizeof(byte) * 8)) | chunkCoordinateZ; //TODO test this

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