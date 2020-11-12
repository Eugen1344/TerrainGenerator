using System.Collections.Generic;
using UnityEngine;

namespace Caves.Cells
{
	public class WallGroup : CaveGroup
	{
		public WallGroup(List<Vector3Int> cellChunkCoordinates) : base(cellChunkCoordinates)
		{
		}
	}
}