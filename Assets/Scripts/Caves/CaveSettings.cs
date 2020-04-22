using System;
using UnityEngine;

[Serializable]
public struct CaveSettings
{
	public Vector3Int TerrainCubicSize;
	public int SwitchToHollowThreshold;
	public int SwitchToWallThreshold;
	[Range(0, 1)] public float RandomHollowCellsPercent;
	[Range(0, 1)] public float RandomHollowCellsPercentDecreasePerLevel;
	public int IterationCount;
	public bool RandomSeed;
	public int Seed;

	public class InvalidCaveSettingsException : Exception
	{
		public InvalidCaveSettingsException(string message) : base(message)
		{
		}
	}
}