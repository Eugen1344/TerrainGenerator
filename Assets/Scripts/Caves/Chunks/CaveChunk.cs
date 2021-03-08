using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Caves.CaveMesh;
using Caves.Cells;
using Caves.Cells.SimplexNoise;
using MeshGenerators.SurfaceNets;
using UnityEngine;

namespace Caves.Chunks
{
	public class CaveChunk : Chunk
	{
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

		public async Task Generate(ChunkCellData data, CaveChunkManager chunkManager)
		{
			Settings = chunkManager.GeneratorSettings;
			ChunkManager = chunkManager;
			CellData = data;
			ChunkCoordinate = data.ChunkCoordinate;
			ChunkSeed = Settings.GenerateSeed(Settings.Seed, ChunkCoordinate);

			await data.Generate();
		}

		public async Task FinalizeGenerationAsync()
		{
			IsFinalized = true;

			if (IsFinalized)
				return;

			if (_finalizationTask == null)
				_finalizationTask = FinalizeTaskAsync();

			await _finalizationTask;
		}

		private async Task FinalizeTaskAsync()
		{
			CellData.FinalizeGeneration();
			_walls = CreateWalls();

			await GenerateWallMeshesAsync(_walls);

			gameObject.SetActive(true);
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
			Vector3 localPosition = new Vector3(cellCoordinate.x * ChunkManager.MeshSettings.GridSize.x, cellCoordinate.y * ChunkManager.MeshSettings.GridSize.y, cellCoordinate.z * ChunkManager.MeshSettings.GridSize.z);
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
			return localCellCoordinate.x >= 0 && localCellCoordinate.x < Settings.ChunkCubicSize.x &&
				   localCellCoordinate.y >= 0 && localCellCoordinate.y < Settings.ChunkCubicSize.y &&
				   localCellCoordinate.z >= 0 && localCellCoordinate.z < Settings.ChunkCubicSize.z;
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
				localCoordinate.x = Settings.ChunkCubicSize.x - Mathf.Abs(globalCoordinate.x + 1) % Settings.ChunkCubicSize.x - 1;
			}
			else
			{
				localCoordinate.x = globalCoordinate.x % Settings.ChunkCubicSize.x;
			}

			if (globalCoordinate.y < 0)
			{
				localCoordinate.y = Settings.ChunkCubicSize.y - Mathf.Abs(globalCoordinate.y + 1) % Settings.ChunkCubicSize.y - 1;
			}
			else
			{
				localCoordinate.y = globalCoordinate.y % Settings.ChunkCubicSize.y;
			}

			if (globalCoordinate.z < 0)
			{
				localCoordinate.z = Settings.ChunkCubicSize.z - Mathf.Abs(globalCoordinate.z + 1) % Settings.ChunkCubicSize.z - 1;
			}
			else
			{
				localCoordinate.z = globalCoordinate.z % Settings.ChunkCubicSize.z;
			}

			return localCoordinate;
		}

		public Vector3Int GetGlobalCoordinate(Vector3Int localCoordinate)
		{
			return new Vector3Int(localCoordinate.x + Settings.ChunkCubicSize.x * ChunkCoordinate.x, localCoordinate.y + Settings.ChunkCubicSize.y * ChunkCoordinate.y, localCoordinate.z + Settings.ChunkCubicSize.z * ChunkCoordinate.z);
		}
	}
}