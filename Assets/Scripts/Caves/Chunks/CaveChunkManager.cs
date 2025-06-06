using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caves.Cells;
using Caves.Cells.SimplexNoise;
using MeshGenerators;
using UnityEngine;

namespace Caves.Chunks
{
    public class CaveChunkManager : MonoBehaviour
    {
        public CellSettings GeneratorSettings;
        public MeshGeneratorSettings MeshSettings;

        public GameObject ChunkHolder;
        public CaveChunk ChunkPrefab;
        public bool RandomSeed;

        public Dictionary<Vector3Int, CaveChunk> GeneratedChunks = new Dictionary<Vector3Int, CaveChunk>();

        private Vector3 _chunkSize;
        private readonly Dictionary<Vector3Int, Task<CaveChunk>> _chunkGenerationQueue = new Dictionary<Vector3Int, Task<CaveChunk>>();

        private void Awake()
        {
            if (RandomSeed)
                GeneratorSettings.Seed = Environment.TickCount;

            _chunkSize = Vector3.Scale(MeshSettings.GridSize, GeneratorSettings.ChunkGridSize);
        }

        public async Task<CaveChunk> CreateChunkAsync(Vector3Int chunkCoordinate)
        {
            Task<CaveChunk> mainChunkTask = GenerateAndAddChunkAsync(chunkCoordinate);
            _ = mainChunkTask.ContinueWith(ChunkGenerationExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);

            List<Task<CaveChunk>> nearbyChunkTasks = new List<Task<CaveChunk>>(9);

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

                        Task<CaveChunk> chunkTask = GenerateAndAddChunkAsync(nearbyChunkCoordinate);
                        _ = chunkTask.ContinueWith(ChunkGenerationExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);

                        nearbyChunkTasks.Add(chunkTask);
                    }
                }
            }

            await Task.WhenAll(nearbyChunkTasks);

            CaveChunk chunk = mainChunkTask.Result;

            Task finalizationTask = FinalizeChunkAsync(chunk);
            _ = finalizationTask.ContinueWith(ChunkFinalizationExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            await finalizationTask;

            return chunk;
        }

        private void ChunkGenerationExceptionHandler(Task<CaveChunk> task)
        {
            if (task?.Exception?.InnerException != null)
                throw task.Exception.InnerException;
        }

        private void ChunkFinalizationExceptionHandler(Task task)
        {
            if (task?.Exception?.InnerException != null)
                Debug.LogException(task.Exception.InnerException);
        }

        private async Task<CaveChunk> GenerateAndAddChunkAsync(Vector3Int chunkCoordinate)
        {
            Task<CaveChunk> newChunkTask;
            bool isTaskInQueue;

            lock (GeneratedChunks)
            {
                if (GeneratedChunks.TryGetValue(chunkCoordinate, out CaveChunk generatedChunk))
                    return generatedChunk;

                isTaskInQueue = _chunkGenerationQueue.TryGetValue(chunkCoordinate, out newChunkTask);

                if (!isTaskInQueue)
                {
                    newChunkTask = GenerateChunkAsync(chunkCoordinate);
                    _chunkGenerationQueue.Add(chunkCoordinate, newChunkTask);
                }
            }

            if (!isTaskInQueue)
            {
                CaveChunk chunk = await newChunkTask;

                lock (GeneratedChunks)
                {
                    GeneratedChunks.Add(chunkCoordinate, chunk);
                    _chunkGenerationQueue.Remove(chunkCoordinate);
                }
            }

            return await newChunkTask;
        }

        private async Task<CaveChunk> GenerateChunkAsync(Vector3Int chunkCoordinate)
        {
            ChunkCellData cellData = new ChunkCellData(GeneratorSettings, this, chunkCoordinate);
            CaveChunk chunk = CreateChunk(cellData.ChunkCoordinate.ToString());

            await chunk.Generate(cellData, this);

            return chunk;
        }

        private CaveChunk CreateChunk(string chunkName)
        {
            GameObject newChunkObject = Instantiate(ChunkPrefab.gameObject, ChunkHolder.transform);
            newChunkObject.name = chunkName;
            newChunkObject.SetActive(false);

            CaveChunk newChunk = newChunkObject.GetComponent<CaveChunk>();

            return newChunk;
        }

        private async Task FinalizeChunkAsync(CaveChunk chunk)
        {
            //Debug.Log($"Finalizing chunk: {chunk.ChunkCoordinate}");

            await chunk.FinalizeGenerationAsync();
        }

        public Vector3 GetChunkWorldPosition(Vector3Int chunkCoordinate)
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
            Vector3Int offset = new Vector3Int();

            if (globalCellCoordinate.x < 0)
                offset.x = 1;

            if (globalCellCoordinate.y < 0)
                offset.y = 1;

            if (globalCellCoordinate.z < 0)
                offset.z = 1;

            globalCellCoordinate += offset;

            Vector3Int chunkCoordinate = new Vector3Int(
                (int)(globalCellCoordinate.x / GeneratorSettings.ChunkGridSize.x),
                (int)(globalCellCoordinate.y / GeneratorSettings.ChunkGridSize.y),
                (int)(globalCellCoordinate.z / GeneratorSettings.ChunkGridSize.z));

            return chunkCoordinate - offset;
        }
    }
}