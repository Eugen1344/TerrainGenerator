using System;
using UnityEngine;

namespace Caves.Chunks
{
	public class MissingChunkException : Exception
	{
		public MissingChunkException(Vector2Int chunkCoordinate) : base(MissingChunkMessage(chunkCoordinate))
		{

		}

		private static string MissingChunkMessage(Vector2Int chunkCoordinate)
		{
			return $"Tried to find missing chunk at position: {chunkCoordinate}";
		}
	}
}