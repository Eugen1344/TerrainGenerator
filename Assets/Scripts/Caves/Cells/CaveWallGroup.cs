using System.Collections.Generic;
using UnityEngine;

namespace Caves.Cells
{
	public class CaveWallGroup : CaveGroup
	{
		public CaveWallGroup(List<Vector3Int> cellChunkCoordinates) : base(cellChunkCoordinates)
		{
		}
	}
}