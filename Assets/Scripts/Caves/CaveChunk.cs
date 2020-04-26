using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveChunk : Chunk
{
	public CellType[,,] Cells; //TODO remake
	public List<CaveHollowGroup> Hollows;
	public List<CaveWallsGroup> Walls;

	public CaveChunk(List<CaveHollowGroup> hollows, List<CaveWallsGroup> walls)
	{
		Hollows = hollows;
		Walls = walls;
	}
}