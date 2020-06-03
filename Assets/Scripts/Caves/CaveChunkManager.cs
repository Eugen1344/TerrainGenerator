using System;
using System.Collections.Generic;
using UnityEngine;

namespace Caves
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

			_chunkSize = Vector3.Scale(ChunkPrefab.WallMeshSettings.CellSize, ChunkPrefab.Settings.TerrainCubicSize);
		}

		public CaveChunk GenerateAndAddChunk(Vector2Int chunkCoordinate)
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

			newChunk.GenerateChunk(chunkCoordinate);

			return newChunk;
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
	}
}