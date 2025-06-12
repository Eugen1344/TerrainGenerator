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
        [SerializeField] private CaveChunk _chunkPrefab;

        [field: SerializeField] public BaseGeneratorSettings BaseGeneratorSettings { get; private set; }
        [field: SerializeField] public CavesGeneratorSettings CavesSettings { get; private set; }

        private readonly Dictionary<Vector3Int, CaveChunk> _generatedChunks = new Dictionary<Vector3Int, CaveChunk>();
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
            AsyncLazy<CaveChunk> mainChunkTask = GenerateAndAddChunkAsync(chunkCoordinate).ToAsyncLazy();

            List<AsyncLazy<CaveChunk>> nearbyChunkTasks = new List<AsyncLazy<CaveChunk>>(9);

            nearbyChunkTasks.Add(mainChunkTask);

            for (int i = chunkCoordinate.x - 1; i <= chunkCoordinate.x + 1; i++)
            {
                for (int j = chunkCoordinate.y - 1; j <= chunkCoordinate.y + 1; j++)
                {
                    for (int k = chunkCoordinate.z - 1; k <= chunkCoordinate.z + 1; k++)
                    {
                        Vector3Int nearbyChunkCoordinate = new Vector3Int(i, j, k);

                        if (nearbyChunkCoordinate == chunkCoordinate)
                            continue;

                        AsyncLazy<CaveChunk> chunkTask = GenerateAndAddChunkAsync(nearbyChunkCoordinate).ToAsyncLazy();
                        nearbyChunkTasks.Add(chunkTask);
                    }
                }
            }

            await UniTask.WhenAll(nearbyChunkTasks.ConvertAll(lazyTask => lazyTask.Task));

            CaveChunk chunk = await mainChunkTask;

            await chunk.FinalizeGenerationAsync();

            return chunk;
        }

        private async UniTask<CaveChunk> GenerateAndAddChunkAsync(Vector3Int chunkCoordinate)
        {
            if (_generatedChunks.TryGetValue(chunkCoordinate, out CaveChunk generatedChunk))
                return generatedChunk;

            bool isTaskInQueue = _chunkGenerationQueue.TryGetValue(chunkCoordinate, out AsyncLazy<CaveChunk> newChunkTask);
            if (isTaskInQueue)
                return await newChunkTask;

            newChunkTask = GenerateChunkAsync(chunkCoordinate).ToAsyncLazy();
            _chunkGenerationQueue.Add(chunkCoordinate, newChunkTask);

            CaveChunk chunk = await newChunkTask;

            _generatedChunks.Add(chunkCoordinate, chunk);
            _chunkGenerationQueue.Remove(chunkCoordinate);

            return await newChunkTask;
        }

        private async UniTask<CaveChunk> GenerateChunkAsync(Vector3Int chunkCoordinate)
        {
            CaveChunk chunk = CreateChunk(chunkCoordinate);
            await chunk.Generate();

            return chunk;
        }

        private CaveChunk CreateChunk(Vector3Int chunkCoordinate)
        {
            CaveChunk newChunk = Instantiate(_chunkPrefab, _chunkRoot.transform);
            newChunk.Setup(chunkCoordinate, this, _baseSeed);

            return newChunk;
        }

        public CaveChunk GetChunk(Vector3Int chunkCoordinate)
        {
            return _generatedChunks.GetValueOrDefault(chunkCoordinate);
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

            lock (_generatedChunks)
            {
                if (_generatedChunks.TryGetValue(chunkCoordinate, out CaveChunk chunk))
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