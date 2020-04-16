using System.Collections.Generic;

public class CaveChunk : Chunk
{
	public List<CaveHollowGroup> Hollows;
	public List<CaveWallsGroup> Walls;

	public CaveChunk(List<CaveHollowGroup> hollows, List<CaveWallsGroup> walls)
	{
		Hollows = hollows;
		Walls = walls;
	}
}