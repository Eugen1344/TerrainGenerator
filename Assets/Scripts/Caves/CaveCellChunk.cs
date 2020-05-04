using System.Collections.Generic;

public class CaveCellChunk : Chunk
{
	public CellType[,,] Cells; //TODO remake
	public List<CaveHollowGroup> Hollows;
	public List<CaveWallsGroup> Walls;
	public List<CaveTunnel> Tunnels;

	public CaveCellChunk(List<CaveHollowGroup> hollows, List<CaveWallsGroup> walls, List<CaveTunnel> tunnels)
	{
		Hollows = hollows;
		Walls = walls;
		Tunnels = tunnels;
	}
}