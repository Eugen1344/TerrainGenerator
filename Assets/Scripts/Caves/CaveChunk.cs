using System;
using Caves.CaveMesh;
using Caves.Cells;
using MeshGenerators;
using UnityEngine;

namespace Caves
{
	public class CaveChunk : Chunk
	{
		public Vector3Int CellSize;
		public CaveChunkCellData CellData;
		public CaveChunkManager ChunkManager;
		public CaveWall WallPrefab;
		public CaveCellSettings Settings;
		public Vector2Int ChunkCoordinate;
		public int ChunkSeed;

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

			CreateAndPlaceWalls();
		}

		private void CreateAndPlaceWalls()
		{
			for (int i = 0; i < CellData.Walls.Count; i++)
			{
				PlaceWall(CellData.Walls[i], i);
			}
		}

		private void PlaceWall(CaveWallGroup wallCells, int index)
		{
			GameObject wallObject = Instantiate(WallPrefab.gameObject, transform);
			wallObject.name = index.ToString();
			CaveWall wall = wallObject.GetComponent<CaveWall>();

			wall.Generate(wallCells, this);
		}

		public Vector3 GetWorldPosition(Vector3Int cellCoordinate)
		{
			Vector3 localPosition = new Vector3(cellCoordinate.x * CellSize.x, cellCoordinate.y * CellSize.y, cellCoordinate.z * CellSize.z);
			return transform.TransformPoint(localPosition); //TODO spacing
		}
	}
}