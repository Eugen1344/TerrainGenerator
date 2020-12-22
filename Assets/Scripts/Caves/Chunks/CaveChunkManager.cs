using System;
using System.Collections.Generic;
using Caves.Cells;
using UnityEngine;

namespace Caves.Chunks
{
	public class CaveChunkManager : MonoBehaviour
	{
		public Dictionary<Vector3Int, CaveChunk> GeneratedChunks = new Dictionary<Vector3Int, CaveChunk>();
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

		public CaveChunk CreateChunk(Vector3Int chunkCoordinate)
		{
			CaveChunk chunk = GenerateAndAddChunk(chunkCoordinate);

			for (int i = chunkCoordinate.x - 1; i <= chunkCoordinate.x + 1; i++)
			{
				for (int j = chunkCoordinate.y - 1; j <= chunkCoordinate.y + 1; j++)
				{
					for (int k = chunkCoordinate.z - 1; k <= chunkCoordinate.z + 1; k++)
					{
						Vector3Int nearbyChunkCoordinate = new Vector3Int(i, j, k);

						if (nearbyChunkCoordinate == chunkCoordinate)
							continue;

						GenerateAndAddChunk(nearbyChunkCoordinate);
					}
				}
			}

			FinalizeChunk(chunk);

			return chunk;
		}

		private CaveChunk GenerateAndAddChunk(Vector3Int chunkCoordinate)
		{
			if (GeneratedChunks.TryGetValue(chunkCoordinate, out CaveChunk generatedChunk))
				return generatedChunk;
			
			Debug.Log($"Generating chunk: {chunkCoordinate}");

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
			Debug.Log($"Finalizing chunk: {chunk.ChunkCoordinate}");
			
			chunk.FinalizeGeneration();
		}

		private Vector3 GetChunkWorldPosition(Vector3Int chunkCoordinate)
		{
			return new Vector3(chunkCoordinate.x * _chunkSize.x, chunkCoordinate.y * _chunkSize.y, chunkCoordinate.z * _chunkSize.z);
		}

		public Vector3Int GetChunkCoordinate(Vector3 worldPosition)
		{
			Vector3 localPosition = worldPosition - ChunkHolder.transform.position;

			Vector3Int chunkCoordinate = new Vector3Int((int)(localPosition.z / _chunkSize.y), (int)(localPosition.x / _chunkSize.x), (int)(localPosition.y / _chunkSize.z));

			if (localPosition.x < 0)
				chunkCoordinate.y -= 1;

			if (localPosition.z < 0)
				chunkCoordinate.x -= 1;

			if (localPosition.y < 0)
				chunkCoordinate.z -= 1;

			return chunkCoordinate;
		}

		public CellType GetCellFromAllChunks(Vector3Int globalCellCoordinate)
		{
			Vector3Int chunkCoordinate = GetChunkCoordinate(globalCellCoordinate);

			if (GeneratedChunks.TryGetValue(chunkCoordinate, out CaveChunk chunk))
			{
				return chunk.GetCell(globalCellCoordinate);
			}

			throw new MissingChunkException(chunkCoordinate);
		}

		public Vector3Int GetChunkCoordinate(Vector3Int globalCellCoordinate)
		{
			Vector3Int chunkCoordinate = new Vector3Int((int)(globalCellCoordinate.x / ChunkPrefab.Settings.TerrainCubicSize.x), (int)(globalCellCoordinate.y / ChunkPrefab.Settings.TerrainCubicSize.y), (int)(globalCellCoordinate.z / ChunkPrefab.Settings.TerrainCubicSize.z));

			if (globalCellCoordinate.x < 0)
				chunkCoordinate.x -= 1;

			if (globalCellCoordinate.y < 0)
				chunkCoordinate.y -= 1;

			if (globalCellCoordinate.z < 0)
				chunkCoordinate.z -= 1;

			return chunkCoordinate;
		}
	}
}