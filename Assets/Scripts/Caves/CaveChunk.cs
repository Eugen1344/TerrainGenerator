using Caves.CaveMesh;
using Caves.Cells;

namespace Caves
{
	public class CaveChunk : Chunk
	{
		public CaveChunkCellData CellData;
		public CaveWallsMesh WallsMesh;
		public CaveSettings Settings;

		private void Awake()
		{
			CellData = CaveChunkCellData.GenerateCaveChunk(Settings);
			WallsMesh = CaveWallsMesh.GenerateWallMesh(CellData.Walls);
		}
	}
}
