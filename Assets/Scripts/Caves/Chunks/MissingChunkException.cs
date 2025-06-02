using System;
using UnityEngine;

namespace Caves.Chunks
{
    public class MissingChunkException : Exception
    {
        public MissingChunkException(Vector3Int chunkCoordinate) : base(MissingChunkMessage(chunkCoordinate))
        {
        }

        private static string MissingChunkMessage(Vector3Int chunkCoordinate)
        {
            return $"Tried to find missing chunk at position: {chunkCoordinate}";
        }
    }
}