﻿using Caves.CaveMesh;
using Caves.Cells;
using UnityEngine;

namespace Caves.Chunks
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
		public bool IsFinalized = false;

		public void Generate(Vector2Int chunkCoordinate)
		{
			ChunkCoordinate = chunkCoordinate;

			ChunkSeed = Settings.GenerateSeed(Settings.Seed, chunkCoordinate);

			CellData = new CaveChunkCellData(Settings, ChunkManager, chunkCoordinate);
			CellData.Generate();
		}

		public void FinalizeGeneration()
		{
			if (IsFinalized)
				return;

			CellData.FinalizeGeneration();

			TestCreateAndPlaceWalls();

			IsFinalized = true;
		}

		private void TestCreateAndPlaceWalls()
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

		public CellType GetCellFromAllChunks(Vector3Int localCellCoordinate)
		{
			if (IsInsideChunk(localCellCoordinate))
			{
				return CellData.FinalIteration[localCellCoordinate.x, localCellCoordinate.y, localCellCoordinate.z];
			}

			Vector3Int globalCellCoordinate = GetGlobalCoordinate(localCellCoordinate);
			return ChunkManager.GetCellFromAllChunks(globalCellCoordinate);
		}

		public bool IsInsideChunk(Vector3Int localCellCoordinate)
		{
			return localCellCoordinate.x >= 0 && localCellCoordinate.x < Settings.TerrainCubicSize.x &&
				   localCellCoordinate.y >= 0 && localCellCoordinate.y < Settings.TerrainCubicSize.y &&
				   localCellCoordinate.z >= 0 && localCellCoordinate.z < Settings.TerrainCubicSize.z;
		}

		public CellType GetCell(Vector3Int globalCoordinate)
		{
			Vector3Int localCoordinate = GetLocalCoordinate(globalCoordinate);

			return CellData.FinalIteration[localCoordinate.x, localCoordinate.y, localCoordinate.z];
		}

		public Vector3Int GetLocalCoordinate(Vector3Int globalCoordinate)
		{
			Vector3Int localCoordinate = new Vector3Int(0, 0, globalCoordinate.z);

			if (globalCoordinate.x < 0)
			{
				localCoordinate.x = Settings.TerrainCubicSize.x - Mathf.Abs(globalCoordinate.x + 1) % Settings.TerrainCubicSize.x - 1;
			}
			else
			{
				localCoordinate.x = globalCoordinate.x % Settings.TerrainCubicSize.x;
			}

			if (globalCoordinate.y < 0)
			{
				localCoordinate.y = Settings.TerrainCubicSize.y - Mathf.Abs(globalCoordinate.y + 1) % Settings.TerrainCubicSize.y - 1;
			}
			else
			{
				localCoordinate.y = globalCoordinate.y % Settings.TerrainCubicSize.y;
			}

			return localCoordinate;
		}

		public Vector3Int GetGlobalCoordinate(Vector3Int localCoordinate)
		{
			return new Vector3Int(localCoordinate.x + Settings.TerrainCubicSize.x * ChunkCoordinate.x, localCoordinate.y + Settings.TerrainCubicSize.y * ChunkCoordinate.y, localCoordinate.z);
		}
	}
}