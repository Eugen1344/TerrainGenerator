using System;
using System.Collections.Generic;
using Caves.Cells;
using Caves.Cells.SimplexNoise;
using Cysharp.Threading.Tasks;
using MeshGenerators;
using UnityEngine;

namespace Caves.Chunks
{
    public class CaveChunkManager : MonoBehaviour
    {
        [SerializeField] private bool _randomSeed;
        [SerializeField] private int _baseSeed;
        [SerializeField] private GameObject _chunkRoot;
        [SerializeField] private CaveChunk _caveChunkPrefab;
        [SerializeField] private SeamChunk _seamChunkPrefab;

        [field: SerializeField] public BaseGeneratorSettings BaseGeneratorSettings { get; private set; }
        [field: SerializeField] public CavesGeneratorSettings CavesSettings { get; private set; }

        private readonly Dictionary<Vector3Int, CaveChunk> _generatedCaveChunks = new Dictionary<Vector3Int, CaveChunk>();
        private readonly Dictionary<Vector3Int, SeamChunk> _generatedSeamChunks = new Dictionary<Vector3Int, SeamChunk>();
        private readonly Dictionary<Vector3Int, AsyncLazy<CaveChunk>> _chunkGenerationQueue = new Dictionary<Vector3Int, AsyncLazy<CaveChunk>>();

        private void Awake()
        {
            SetupBaseSeed();
        }

        private void SetupBaseSeed()
        {
            if (_randomSeed)
                _baseSeed = Environment.TickCount;
        }

        public async UniTask<CaveChunk> CreateChunkAsync(Vector3Int chunkCoordinate)
        {
            if (_generatedCaveChunks.TryGetValue(chunkCoordinate, out CaveChunk generatedChunk))
                return generatedChunk;

            bool isTaskInQueue = _chunkGenerationQueue.TryGetValue(chunkCoordinate, out AsyncLazy<CaveChunk> newChunkTask);
            if (isTaskInQueue)
                return await newChunkTask;

            newChunkTask = GenerateChunkAsync(chunkCoordinate).ToAsyncLazy();
            _chunkGenerationQueue[chunkCoordinate] = newChunkTask;

            CaveChunk chunk = await newChunkTask;

            _generatedCaveChunks[chunkCoordinate] = chunk;
            _chunkGenerationQueue.Remove(chunkCoordinate);

            return await newChunkTask;
        }

        private async UniTask<CaveChunk> GenerateChunkAsync(Vector3Int chunkCoordinate)
        {
            SeamChunk[,,] seamChunks = GatherSeamChunks(chunkCoordinate);
            CaveChunk chunk = CreateChunk(chunkCoordinate, seamChunks);

            await chunk.GenerateData();
            await chunk.GenerateMeshes();

            return chunk;
        }

        private CaveChunk CreateChunk(Vector3Int chunkCoordinate, SeamChunk[,,] seamChunks)
        {
            CaveChunk newChunk = Instantiate(_caveChunkPrefab, _chunkRoot.transform);
            newChunk.Setup(chunkCoordinate, seamChunks, BaseGeneratorSettings, CavesSettings, _baseSeed);

            return newChunk;
        }

        private SeamChunk[,,] GatherSeamChunks(Vector3Int chunkCoordinate)
        {
            SeamChunk[,,] nearbySeamChunks = new SeamChunk[3, 3, 3];

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3Int seamCoordinate = chunkCoordinate + new Vector3Int(i, j, k);
                        nearbySeamChunks[i, j, k] = GetOrAddSeamChunk(seamCoordinate);
                    }
                }
            }

            return nearbySeamChunks;
        }

        private SeamChunk GetOrAddSeamChunk(Vector3Int seamCoordinate)
        {
            if (_generatedSeamChunks.TryGetValue(seamCoordinate, out SeamChunk seamChunk))
                return seamChunk;

            seamChunk = CreateSeamChunk(seamCoordinate);
            _generatedSeamChunks[seamCoordinate] = seamChunk;

            return seamChunk;
        }

        private SeamChunk CreateSeamChunk(Vector3Int seamCoordinate)
        {
            SeamChunk newChunk = Instantiate(_seamChunkPrefab, _chunkRoot.transform);
            newChunk.Setup(seamCoordinate);

            return newChunk;
        }

        public CaveChunk GetChunk(Vector3Int chunkCoordinate)
        {
            return _generatedCaveChunks.GetValueOrDefault(chunkCoordinate);
        }

        public Vector3Int GetChunkCoordinate(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - _chunkRoot.transform.position;

            Vector3Int chunkCoordinate = new Vector3Int((int)(localPosition.x / BaseGeneratorSettings.ChunkSize.x), (int)(localPosition.y / BaseGeneratorSettings.ChunkSize.y), (int)(localPosition.z / BaseGeneratorSettings.ChunkSize.z));

            if (localPosition.x < 0)
                chunkCoordinate.x -= 1;

            if (localPosition.y < 0)
                chunkCoordinate.y -= 1;

            if (localPosition.z < 0)
                chunkCoordinate.z -= 1;

            return chunkCoordinate;
        }

        public CellType GetCellFromAllChunks(Vector3Int globalCellCoordinate)
        {
            Vector3Int chunkCoordinate = GetChunkCoordinate(globalCellCoordinate);

            lock (_generatedCaveChunks)
            {
                if (_generatedCaveChunks.TryGetValue(chunkCoordinate, out CaveChunk chunk))
                    return chunk.GetCell(globalCellCoordinate);
            }

            throw new MissingChunkException(chunkCoordinate);
        }

        public Vector3Int GetChunkCoordinate(Vector3Int globalCellCoordinate)
        {
            Vector3Int offset = new Vector3Int();

            if (globalCellCoordinate.x < 0)
                offset.x = 1;

            if (globalCellCoordinate.y < 0)
                offset.y = 1;

            if (globalCellCoordinate.z < 0)
                offset.z = 1;

            globalCellCoordinate += offset;

            Vector3Int chunkCoordinate = new Vector3Int(
                globalCellCoordinate.x / BaseGeneratorSettings.ChunkSize.x,
                globalCellCoordinate.y / BaseGeneratorSettings.ChunkSize.y,
                globalCellCoordinate.z / BaseGeneratorSettings.ChunkSize.z);

            return chunkCoordinate - offset;
        }
    }
}