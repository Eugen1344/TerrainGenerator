using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CaveGroup
{
	public List<Vector2Int> CellChunkCoordinates = new List<Vector2Int>();
	public int CellCount => CellChunkCoordinates.Count;

	protected CaveGroup(List<Vector2Int> cellChunkCoordinates)
	{
		CellChunkCoordinates = cellChunkCoordinates;
	}

	public static CaveGroup GetCaveGroup(CellType cellType, List<Vector2Int> cellChunkCoordinates)
	{
		switch (cellType)
		{
			case CellType.Wall:
				return new CaveWallsGroup(cellChunkCoordinates);
			case CellType.Hollow:
				return new CaveHollowGroup(cellChunkCoordinates);
			default:
				throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
		}
	}
}