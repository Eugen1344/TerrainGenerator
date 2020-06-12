using Caves.CaveMesh;
using Caves.Cells;
using UnityEngine;

namespace Caves
{
	public class CaveChunk : Chunk
	{
		public CaveChunkManager ChunkManager;
		public CaveChunkCellData CellData;
		public PolygonGeneratorSettings WallPolygonSettings;
		public CaveWallsMesh WallsMesh;
		public CaveCellSettings Settings;
		public Vector2Int ChunkCoordinate;
		public int ChunkSeed;

		public MeshFilter TestMesh;
		public MeshCollider TestCollider;

		public void Generate(Vector2Int chunkCoordinate)
		{
			ChunkCoordinate = chunkCoordinate;

			ChunkSeed = Settings.GenerateSeed(Settings.Seed, chunkCoordinate);

			CellData = new CaveChunkCellData(Settings, ChunkManager, chunkCoordinate);
			CellData.Generate();
		}

		public void FinalizeGeneration()
		{
			CellData.FinalizeGeneration();

			WallsMesh = CaveWallsMesh.GenerateWallMesh(CellData.Walls, WallPolygonSettings);

			TestMesh.sharedMesh = WallsMesh.Mesh;
			TestCollider.sharedMesh = WallsMesh.Mesh;
		}

		public Vector3 GetWorldPosition(Vector3Int cellPosition)
		{
			Vector3 localPosition = new Vector3(cellPosition.x * WallPolygonSettings.CellSize.x, cellPosition.y * WallPolygonSettings.CellSize.y, cellPosition.z * WallPolygonSettings.CellSize.z);
			return transform.TransformPoint(localPosition); //TODO spacing
		}
	}
}