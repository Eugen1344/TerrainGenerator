﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caves.CaveMesh;
using Caves.Cells;
using Caves.Cells.SimplexNoise;
using UnityEngine;

namespace Caves.Chunks
{
	public class CaveChunk : Chunk
	{
		public Vector3Int CellSize;
		public ChunkCellData CellData;
		public CaveChunkManager ChunkManager;
		public CaveWall WallPrefab;
		public CellSettings Settings;
		public Vector3Int ChunkCoordinate;
		public int ChunkSeed;
		public bool IsFinalized = false;

		private List<CaveWall> _walls = new List<CaveWall>();
		private Task _finalizationTask;

		private void Start()
		{
			gameObject.transform.localPosition = ChunkManager.GetChunkWorldPosition(CellData.ChunkCoordinate);

			foreach (CaveWall wall in _walls)
			{
				wall.gameObject.SetActive(true);
			}
		}

		public void Init(ChunkCellData data, CaveChunkManager chunkManager)
		{
			Settings = chunkManager.GeneratorSettings;
			ChunkManager = chunkManager;
			CellData = data;
			ChunkCoordinate = data.ChunkCoordinate;
			ChunkSeed = Settings.GenerateSeed(Settings.Seed, ChunkCoordinate);
		}

		public async Task FinalizeGenerationAsync()
		{
			lock (this)
			{
				if (IsFinalized)
					return;

				if (_finalizationTask == null)
					_finalizationTask = FinalizeTaskAsync();
			}

			await _finalizationTask;
		}

		private async Task FinalizeTaskAsync()
		{
			await CellData.FinalizeGenerationAsync();
			_walls = CreateWalls();

			await GenerateWallMeshesAsync(_walls);

			gameObject.SetActive(true);

			lock (this)
			{
				IsFinalized = true;
			}
		}

		private List<CaveWall> CreateWalls()
		{
			List<CaveWall> walls = new List<CaveWall>();

			for (int i = 0; i < CellData.Walls.Count; i++)
			{
				walls.Add(CreateWall(i));
			}

			return walls;
		}

		private async Task GenerateWallMeshesAsync(List<CaveWall> walls)
		{
			List<Task> wallGenerationTasks = new List<Task>();

			for (int i = 0; i < CellData.Walls.Count; i++)
			{
				wallGenerationTasks.Add(GenerateWallMeshAsync(walls[i], CellData.Walls[i]));
			}

			await Task.WhenAll(wallGenerationTasks);
		}

		private CaveWall CreateWall(int index)
		{
			GameObject wallObject = Instantiate(WallPrefab.gameObject, transform);
			wallObject.SetActive(false);
			wallObject.name = index.ToString();

			return wallObject.GetComponent<CaveWall>();
		}

		private async Task GenerateWallMeshAsync(CaveWall wall, WallGroup wallCells)
		{
			await Task.Run(() => wall.Generate(wallCells, this));
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
				return CellData.Cells[localCellCoordinate.x, localCellCoordinate.y, localCellCoordinate.z];
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

			return CellData.Cells[localCoordinate.x, localCoordinate.y, localCoordinate.z];
		}

		public Vector3Int GetLocalCoordinate(Vector3Int globalCoordinate)
		{
			Vector3Int localCoordinate = new Vector3Int(0, 0, 0);

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

			if (globalCoordinate.z < 0)
			{
				localCoordinate.z = Settings.TerrainCubicSize.z - Mathf.Abs(globalCoordinate.z + 1) % Settings.TerrainCubicSize.z - 1;
			}
			else
			{
				localCoordinate.z = globalCoordinate.z % Settings.TerrainCubicSize.z;
			}

			return localCoordinate;
		}

		public Vector3Int GetGlobalCoordinate(Vector3Int localCoordinate)
		{
			return new Vector3Int(localCoordinate.x + Settings.TerrainCubicSize.x * ChunkCoordinate.x, localCoordinate.y + Settings.TerrainCubicSize.y * ChunkCoordinate.y, localCoordinate.z + Settings.TerrainCubicSize.z * ChunkCoordinate.z);
		}
	}
}