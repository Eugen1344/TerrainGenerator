using System;
using System.Runtime;
using Caves.CaveMesh;
using Caves.Cells;
using UnityEngine;

namespace Caves
{
	public class CaveChunk : Chunk
	{
		public CaveChunkManager ChunkManager;
		public CaveChunkCellData CellData;
		public CaveMeshSettings WallMeshSettings;
		public CaveWallsMesh WallsMesh;
		public CaveCellSettings Settings;
		public Vector2Int ChunkCoordinate;
		public int ChunkSeed;

		public MeshFilter TestMesh;
		public MeshCollider TestCollider;

		public void GenerateChunk(Vector2Int chunkCoordinate)
		{
			ChunkCoordinate = chunkCoordinate;

			ChunkSeed = Settings.GenerateSeed(Settings.Seed, chunkCoordinate);

			CellData = new CaveChunkCellData(Settings, ChunkManager, chunkCoordinate);
			CellData.Generate();
			WallsMesh = CaveWallsMesh.GenerateWallMesh(CellData.Walls, WallMeshSettings);

			TestMesh.sharedMesh = WallsMesh.Mesh;
			TestCollider.sharedMesh = WallsMesh.Mesh;
		}

		public Vector3 GetWorldPosition(Vector3Int cellPosition)
		{
			Vector3 localPosition = new Vector3(cellPosition.x * WallMeshSettings.CellSize.x, cellPosition.y * WallMeshSettings.CellSize.y, cellPosition.z * WallMeshSettings.CellSize.z);
			return transform.TransformPoint(localPosition); //TODO spacing
		}
	}
}