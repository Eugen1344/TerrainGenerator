using System.Collections.Generic;
using UnityEngine;

namespace Caves.Cells
{
	public class CaveWallsGroup : CaveGroup
	{
		public CaveWallsGroup(List<Vector3Int> cellChunkCoordinates) : base(cellChunkCoordinates)
		{
		}
	}
}