using System.Collections.Generic;

public class CaveChunk : Chunk
{
	public CellType[,,] Cells; //TODO remake
	public List<CaveHollowGroup> Hollows;
	public List<CaveWallsGroup> Walls;
	public List<CaveTunnel> Tunnels;

	public CaveChunk(List<CaveHollowGroup> hollows, List<CaveWallsGroup> walls, List<CaveTunnel> tunnels)
	{
		Hollows = hollows;
		Walls = walls;
		Tunnels = tunnels;
	}
}