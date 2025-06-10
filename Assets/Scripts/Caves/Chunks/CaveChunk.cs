#nullable enable
using System.Collections.Generic;
using Caves.CaveMeshes;
using Caves.Cells;
using Caves.Cells.SimplexNoise;
using Cysharp.Threading.Tasks;
using MeshGenerators;
using UnityEngine;

namespace Caves.Chunks
{
    public class CaveChunk : Chunk
    {
        public ChunkCellData CellData { get; private set; }

        [SerializeField] private CaveMesh _caveMeshPrefab;

        private BaseGeneratorSettings _baseSettings;
        private CavesGeneratorSettings _cavesSettings;

        private bool _isFinalized;
        private List<CaveMesh> _caveMeshes = new List<CaveMesh>();
        private AsyncLazy? _finalizationTask;

        private void Start()
        {
            foreach (CaveMesh caveMesh in _caveMeshes)
                caveMesh.gameObject.SetActive(true);
        }

        public async UniTask Generate(ChunkCellData data, BaseGeneratorSettings baseSettings, CavesGeneratorSettings cavesSettings)
        {
            _baseSettings = baseSettings;
            _cavesSettings = cavesSettings;
            CellData = data;

            gameObject.transform.position = GetWorldPosition();

            await data.GenerateAsync(destroyCancellationToken);
        }

        public async UniTask FinalizeGenerationAsync()
        {
            if (_isFinalized)
                return;

            _isFinalized = true;

            _finalizationTask ??= GenerateMeshesAsync().ToAsyncLazy();
            await _finalizationTask;
        }

        private async UniTask GenerateMeshesAsync()
        {
            _caveMeshes = InstantiateMeshes();

            await GenerateMeshesAsync(_caveMeshes);

            gameObject.SetActive(true);
        }

        private List<CaveMesh> InstantiateMeshes()
        {
            List<CaveMesh> caveMeshes = new List<CaveMesh>();

            for (int i = 0; i < CellData.Walls.Count; i++)
                caveMeshes.Add(InstantiateMesh(i));

            return caveMeshes;
        }

        private CaveMesh InstantiateMesh(int index)
        {
            GameObject wallObject = Instantiate(_caveMeshPrefab.gameObject, transform);
            wallObject.SetActive(false);
            wallObject.name = index.ToString();

            return wallObject.GetComponent<CaveMesh>();
        }

        private async UniTask GenerateMeshesAsync(List<CaveMesh> caveMeshes)
        {
            List<UniTask> meshGenerationTasks = new List<UniTask>();

            for (int i = 0; i < CellData.Walls.Count; i++)
                meshGenerationTasks.Add(GenerateMeshAsync(caveMeshes[i], CellData.Walls[i]));

            await UniTask.WhenAll(meshGenerationTasks);
        }

        private async UniTask GenerateMeshAsync(CaveMesh caveMesh, WallGroup wallCells)
        {
            await UniTask.RunOnThreadPool(() => caveMesh.Generate(wallCells, this, _baseSettings));
        }

        public Vector3 GetWorldPosition(Vector3Int cellCoordinate)
        {
            Vector3 localPosition = new Vector3(cellCoordinate.x * _baseSettings.ChunkSize.x, cellCoordinate.y * _baseSettings.ChunkSize.y, cellCoordinate.z * _baseSettings.ChunkSize.z);

            return transform.TransformPoint(localPosition); //TODO spacing
        }

        public bool IsInsideChunk(Vector3Int localCellCoordinate)
        {
            return localCellCoordinate.x >= 0 && localCellCoordinate.x < _baseSettings.ChunkSize.x &&
                   localCellCoordinate.y >= 0 && localCellCoordinate.y < _baseSettings.ChunkSize.y &&
                   localCellCoordinate.z >= 0 && localCellCoordinate.z < _baseSettings.ChunkSize.z;
        }

        public CellType GetCell(Vector3Int globalCoordinate)
        {
            Vector3Int localCoordinate = GetLocalCoordinate(globalCoordinate);

            return CellData.Cells[localCoordinate.x, localCoordinate.y, localCoordinate.z];
        }

        public Vector3 GetWorldPosition()
        {
            return new Vector3(CellData.ChunkCoordinate.x * _baseSettings.ChunkSize.x, CellData.ChunkCoordinate.y * _baseSettings.ChunkSize.y, CellData.ChunkCoordinate.z * _baseSettings.ChunkSize.z);
        }

        public Vector3Int GetLocalCoordinate(Vector3Int globalCoordinate)
        {
            Vector3Int chunkSize = _baseSettings.ChunkSize;
            Vector3Int localCoordinate = new Vector3Int(0, 0, 0);

            if (globalCoordinate.x < 0)
                localCoordinate.x = chunkSize.x - Mathf.Abs(globalCoordinate.x + 1) % chunkSize.x - 1;
            else
                localCoordinate.x = globalCoordinate.x % chunkSize.x;

            if (globalCoordinate.y < 0)
                localCoordinate.y = chunkSize.y - Mathf.Abs(globalCoordinate.y + 1) % chunkSize.y - 1;
            else
                localCoordinate.y = globalCoordinate.y % chunkSize.y;

            if (globalCoordinate.z < 0)
                localCoordinate.z = chunkSize.z - Mathf.Abs(globalCoordinate.z + 1) % chunkSize.z - 1;
            else
                localCoordinate.z = globalCoordinate.z % chunkSize.z;

            return localCoordinate;
        }

        public Vector3Int GetGlobalCoordinate(Vector3Int localCoordinate)
        {
            return new Vector3Int(localCoordinate.x + _baseSettings.ChunkSize.x * CellData.ChunkCoordinate.x, localCoordinate.y + _baseSettings.ChunkSize.y * CellData.ChunkCoordinate.y, localCoordinate.z + _baseSettings.ChunkSize.z * CellData.ChunkCoordinate.z);
        }
    }
}