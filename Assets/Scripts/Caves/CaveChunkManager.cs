using System.Collections.Generic;
using Caves;
using UnityEngine;

public class CaveChunkManager : MonoBehaviour
{
	public Dictionary<Vector2Int, CaveChunk> GeneratedChunks = new Dictionary<Vector2Int, CaveChunk>();
	public GameObject ChunkObject;

	public CaveChunk GenerateAndAddChunk(Vector2Int chunkPosition)
	{
		GameObject newChunkObject = Instantiate(ChunkObject);
		CaveChunk newChunk = newChunkObject.GetComponent<CaveChunk>();

		newChunk.GenerateChunk(chunkPosition);

		return newChunk;
	}
}