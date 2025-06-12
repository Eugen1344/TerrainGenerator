using System;
using UnityEngine;

namespace MeshGenerators
{
    [Serializable]
    public struct BaseGeneratorSettings
    {
        [field: SerializeField] public Vector3Int ChunkSize { get; private set; }
        [field: SerializeField] public Vector3Int GridSize { get; private set; }
        [field: SerializeField] public float NoiseScale { get; private set; }
        [field: SerializeField] public int SmoothIterationCount { get; private set; }

        public readonly Vector3 GetChunkGridSizeMultiplier()
        {
            return new Vector3((float)ChunkSize.x / (GridSize.x - 1), (float)ChunkSize.y / (GridSize.y - 1), (float)ChunkSize.z / (GridSize.z - 1));
        }
    }
}