using System;
using System.Collections.Generic;
using Caves.Cells;
using UnityEngine;

namespace Caves.Chunks
{
	public class CaveChunkManager : MonoBehaviour
	{
		public Dictionary<Vector2Int, CaveChunk> GeneratedChunks = new Dictionary<Vector2Int, CaveChunk>();
		public GameObject ChunkHolder;
		public CaveChunk ChunkPrefab;
		public bool RandomSeed;
		public int Seed;

		private Vector3 _chunkSize;

		private void Awake()
		{
			if (RandomSeed)
				Seed = Environment.TickCount;

			_chunkSize = Vector3.Scale(ChunkPrefab.CellSize, ChunkPrefab.Settings.TerrainCubicSize);
		}

		public CaveChunk CreateChunk(Vector2Int chunkCoordinate)
		{
			CaveChunk chunk = GenerateAndAddChunk(chunkCoordinate);

			for (int i = chunkCoordinate.x - 1; i <= chunkCoordinate.x + 1; i++)
			{
				for (int j = chunkCoordinate.y - 1; j <= chunkCoordinate.y + 1; j++)
				{
					Vector2Int nearbyChunkCoordinate = new Vector2Int(i, j);

					GenerateAndAddChunk(nearbyChunkCoordinate);
				}
			}

			FinalizeChunk(chunk);

			return chunk;
		}

		private CaveChunk GenerateAndAddChunk(Vector2Int chunkCoordinate)
		{
			if (GeneratedChunks.TryGetValue(chunkCoordinate, out CaveChunk generatedChunk))
				return generatedChunk;

			GameObject newChunkObject = Instantiate(ChunkPrefab.gameObject, ChunkHolder.transform);
			newChunkObject.name = chunkCoordinate.ToString();
			newChunkObject.transform.localPosition = GetChunkWorldPosition(chunkCoordinate);

			CaveChunk newChunk = newChunkObject.GetComponent<CaveChunk>();
			newChunk.Settings.Seed = Seed; //TODO rewrite seed
			newChunk.ChunkManager = this;

			GeneratedChunks.Add(chunkCoordinate, newChunk);

			newChunk.Generate(chunkCoordinate);

			return newChunk;
		}

		private void FinalizeChunk(CaveChunk chunk)
		{
			chunk.FinalizeGeneration();
		}

		private Vector3 GetChunkWorldPosition(Vector2Int chunkCoordinate)
		{
			return new Vector3(chunkCoordinate.x * _chunkSize.x, chunkCoordinate.y * _chunkSize.y, 0);
		}

		public Vector2Int GetChunkCoordinate(Vector3 worldPosition)
		{
			Vector3 localPosition = worldPosition - ChunkHolder.transform.position;

			Vector2Int chunkCoordinate = new Vector2Int((int)(localPosition.z / _chunkSize.y), (int)(localPosition.x / _chunkSize.x));

			if (localPosition.x < 0)
				chunkCoordinate.y -= 1;

			if (localPosition.z < 0)
				chunkCoordinate.x -= 1;

			return chunkCoordinate;
		}

		public CellType GetCellFromAllChunks(Vector3Int globalCellCoordinate)
		{
			Vector2Int chunkCoordinate = GetChunkCoordinate(globalCellCoordinate);

			if (GeneratedChunks.TryGetValue(chunkCoordinate, out CaveChunk chunk))
			{
				return chunk.GetCell(globalCellCoordinate);
			}

			throw new MissingChunkException(chunkCoordinate);
		}

		public Vector2Int GetChunkCoordinate(Vector3Int globalCellCoordinate)
		{
			Vector2Int chunkCoordinate = new Vector2Int((int)(globalCellCoordinate.x / ChunkPrefab.Settings.TerrainCubicSize.x), (int)(globalCellCoordinate.y / ChunkPrefab.Settings.TerrainCubicSize.y));

			if (globalCellCoordinate.x < 0)
				chunkCoordinate.x -= 1;

			if (globalCellCoordinate.y < 0)
				chunkCoordinate.y -= 1;

			return chunkCoordinate;
		}
	}
}