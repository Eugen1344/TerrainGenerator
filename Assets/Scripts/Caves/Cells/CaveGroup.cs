using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Caves.Cells
{
	public abstract class CaveGroup
	{
		public List<Vector3Int> CellChunkCoordinates;
		public int CellCount => CellChunkCoordinates.Count;
		//public List<Vector3Int> GroundCells; //TODO maybe break cells into layers (List<Vector2Int>[])

		protected CaveGroup(List<Vector3Int> cellChunkCoordinates)
		{
			CellChunkCoordinates = cellChunkCoordinates;
			//GroundCells = GetGroundCells();
		}

		/*private List<Vector3Int> GetGroundCells()
		{
			return CellChunkCoordinates.Where(cell => cell.z == 0).ToList();
		}*/

		public Vector3Int GetLowestPoint()
		{
			return CellChunkCoordinates.OrderBy(coord => coord.z).First();
		}

		public static CaveGroup GetCaveGroup(CellType cellType, List<Vector3Int> cellChunkCoordinates)
		{
			switch (cellType)
			{
				case CellType.Wall:
					return new WallGroup(cellChunkCoordinates);
				case CellType.Hollow:
					return new HollowGroup(cellChunkCoordinates);
				default:
					throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
			}
		}
	}
}