using System.Collections.Generic;
using Caves.Cells;
using UnityEngine;

namespace Develop
{
	public static class TestWalls
	{
		public static CaveWallsGroup Wall2x2 = new CaveWallsGroup(new List<Vector3Int>
		{
			new Vector3Int(0, 0, 0),
			new Vector3Int(1, 0, 0),
			new Vector3Int(0, 1, 0),
			new Vector3Int(1, 1, 0),
		});
	}
}
