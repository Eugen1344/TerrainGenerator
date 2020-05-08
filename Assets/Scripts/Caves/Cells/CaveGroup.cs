using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Caves.Cells
{
	public abstract class CaveGroup
	{
		public List<Vector3Int> CellChunkCoordinates = new List<Vector3Int>();
		public int CellCount => CellChunkCoordinates.Count;
		public int GroundCellCount;

		protected CaveGroup(List<Vector3Int> cellChunkCoordinates)
		{
			CellChunkCoordinates = cellChunkCoordinates;
			GroundCellCount = GetGroundCellCount();
		}

		private int GetGroundCellCount()
		{
			return CellChunkCoordinates.Count(cell => cell.z == 0);
		}

		public static CaveGroup GetCaveGroup(CellType cellType, List<Vector3Int> cellChunkCoordinates)
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
}