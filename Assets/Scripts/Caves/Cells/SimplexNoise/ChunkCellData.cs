using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MeshGenerators;
using SimplexNoise;
using UnityEngine;

namespace Caves.Cells.SimplexNoise
{
    public class ChunkCellData
    {
        public CellType[,,] Cells;

        public List<HollowGroup> Hollows;
        public List<WallGroup> Walls;
        public List<Tunnel> Tunnels;

        private readonly BaseGeneratorSettings _baseSettings;
        private readonly CavesGeneratorSettings _cavesSettings;
        private readonly Vector3Int _chunkCoordinate;

        private readonly Noise _noiseGenerator;

        public ChunkCellData(BaseGeneratorSettings baseSettings, CavesGeneratorSettings cavesSettings, Vector3Int chunkCoordinate, int baseSeed)
        {
            _baseSettings = baseSettings;
            _cavesSettings = cavesSettings;
            _chunkCoordinate = chunkCoordinate;

            _noiseGenerator = new Noise(baseSeed);
        }

        public UniTask GenerateAsync(CancellationToken cancellationToken)
        {
            return UniTask.RunOnThreadPool(GenerateDataAsync, cancellationToken: cancellationToken);
        }

        private void GenerateDataAsync()
        {
            Vector3Int gridSize = _baseSettings.GridSize;
            Vector3Int offset = gridSize * _chunkCoordinate;
            float[,,] noise = SampleNoise(gridSize, offset);

            Cells = GetCellsFromNoise(noise);

            Hollows = GetCellGroups<HollowGroup>(CellType.Hollow);
            Hollows = RemoveSmallHollowGroups(Hollows, ref Cells, _cavesSettings.MinCaveSize);

            Tunnels = _cavesSettings.GenerateTunnels ? Tunnel.CreateTunnelsAndConnectCaves(ref Cells, Hollows, _baseSettings, _cavesSettings) : new List<Tunnel>();

            Walls = GetCellGroups<WallGroup>(CellType.Wall);
        }

        private float[,,] SampleNoise(Vector3Int size, Vector3Int offset)
        {
            float[,,] noise = new float[size.x, size.y, size.z];

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        Vector3Int noisePixelPosition = new Vector3Int(i + offset.x, j + offset.y, k + offset.z);

                        noise[i, j, k] = _noiseGenerator.CalcPixel3D(noisePixelPosition.x, noisePixelPosition.y, noisePixelPosition.z, _baseSettings.NoiseScale);
                    }
                }
            }

            return noise;
        }

        private List<HollowGroup> RemoveSmallHollowGroups(List<HollowGroup> hollows, ref CellType[,,] cells, int minCaveSize)
        {
            List<HollowGroup> filteredHollows = new List<HollowGroup>();

            foreach (HollowGroup hollow in hollows)
            {
                if (hollow.CellCount < minCaveSize)
                {
                    foreach (Vector3Int coordinate in hollow.CellChunkCoordinates)
                        cells[coordinate.x, coordinate.y, coordinate.z] = CellType.Wall;
                }
                else
                {
                    filteredHollows.Add(hollow);
                }
            }

            return filteredHollows;
        }

        private CellType[,,] GetCellsFromNoise(float[,,] noise)
        {
            int length = noise.GetLength(0);
            int width = noise.GetLength(1);
            int height = noise.GetLength(2);

            CellType[,,] cells = new CellType[length, width, height];

            Vector3Int offset = _baseSettings.GridSize * _chunkCoordinate;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        float normalizedNoise = (noise[i, j, k] + 1f) / 2f;
                        Vector3Int noisePixelPosition = new Vector3Int(i + offset.x, j + offset.y, k + offset.z);

                        if (normalizedNoise <= GetRandomHollowCellsPercent(noisePixelPosition))
                            cells[i, j, k] = CellType.Hollow;
                        else
                            cells[i, j, k] = CellType.Wall;
                    }
                }
            }

            return cells;
        }

        private float GetRandomHollowCellsPercent(Vector3Int globalPixelCoordinate)
        {
            int heightDiff = Mathf.Abs(globalPixelCoordinate.y - _cavesSettings.RandomHollowCellsPercentDecreaseStartingHeight);

            return Mathf.Clamp(_cavesSettings.HollowCellThreshold - heightDiff * _cavesSettings.HollowCellThresholdDecreasePerHeight, 0, _cavesSettings.HollowCellThreshold);
        }

        private List<T> GetCellGroups<T>(CellType searchedCellType) where T : CaveGroup
        {
            int length = Cells.GetLength(0);
            int width = Cells.GetLength(1);
            int height = Cells.GetLength(2);

            List<T> result = new List<T>();

            bool[,,] markedCells = new bool[length, width, height];

            Queue<Vector3Int> cellsToSearch = new Queue<Vector3Int>();

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        if (Cells[i, j, k] == searchedCellType && !markedCells[i, j, k])
                        {
                            List<Vector3Int> foundCells = new List<Vector3Int>();

                            cellsToSearch.Enqueue(new Vector3Int(i, j, k));
                            markedCells[i, j, k] = true;

                            while (cellsToSearch.Count != 0)
                            {
                                Vector3Int searchCoreCell = cellsToSearch.Dequeue();

                                foundCells.Add(searchCoreCell);

                                int startI = searchCoreCell.x - 1 < 0 ? 0 : searchCoreCell.x - 1;
                                int startJ = searchCoreCell.y - 1 < 0 ? 0 : searchCoreCell.y - 1;
                                int endI = searchCoreCell.x + 1 >= length ? length - 1 : searchCoreCell.x + 1;
                                int endJ = searchCoreCell.y + 1 >= width ? width - 1 : searchCoreCell.y + 1;
                                int startK = searchCoreCell.z - 1 < 0 ? 0 : searchCoreCell.z - 1;
                                int endK = searchCoreCell.z + 1 >= height ? height - 1 : searchCoreCell.z + 1;

                                for (int i1 = startI; i1 <= endI; i1++)
                                {
                                    for (int j1 = startJ; j1 <= endJ; j1++)
                                    {
                                        for (int k1 = startK; k1 <= endK; k1++)
                                        {
                                            if (i1 == searchCoreCell.x && j1 == searchCoreCell.y && k1 == searchCoreCell.z ||
                                                i1 != searchCoreCell.x && j1 != searchCoreCell.y)
                                                continue;

                                            if (Cells[i1, j1, k1] == searchedCellType && !markedCells[i1, j1, k1])
                                            {
                                                Vector3Int newSearchCoreCell = new Vector3Int(i1, j1, k1);

                                                cellsToSearch.Enqueue(newSearchCoreCell);
                                                markedCells[i1, j1, k1] = true;
                                            }
                                        }
                                    }
                                }
                            }

                            CaveGroup newGroup = CaveGroup.GetCaveGroup(searchedCellType, foundCells);
                            result.Add((T)newGroup);
                        }
                    }
                }
            }

            return result;
        }
    }
}