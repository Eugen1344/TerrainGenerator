using System.Collections.Generic;
using Caves.Chunks;
using UnityEngine;

namespace Caves.Cells.CellularAutomata
{
    public class GeneratedChunkCellData : ChunkCellData
    {
        public List<HollowGroup> Hollows;
        public List<WallGroup> Walls;
        public List<Tunnel> Tunnels;
        public bool IsFinalized = false;

        public GeneratedChunkCellData(CellSettings settings, CaveChunkManager chunkManager, Vector3Int chunkCoordinate) : base(settings, chunkManager, chunkCoordinate)
        {
        }

        public void Generate()
        {
            ChunkCellData[,] nearbyChunks = GetNearbyChunksOrGenerateGhostChunks(out List<GhostChunkCellData> ghostChunks);

            foreach (GhostChunkCellData ghostChunk in ghostChunks)
            {
                ghostChunk.FindNearbyChunks(ghostChunks);
            }

            for (int i = 0; i < Settings.IterationCount; i++)
            {
                GenerateIterations(nearbyChunks, i);

                foreach (GhostChunkCellData ghostChunk in ghostChunks)
                {
                    ghostChunk.GenerateIterations(ghostChunk.NearbyChunks, i);
                }
            }

            FinalIteration = (CellType[,,])Iterations[Iterations.Count - 1].Clone();

            Hollows = GetCellGroups<HollowGroup>(CellType.Hollow);
        }

        public void FinalizeGeneration()
        {
            //RemoveSmallHollowGroupsByGroundSize(Hollows, Settings.MinHollowGroupCubicSize);

            Hollows = GetCellGroups<HollowGroup>(CellType.Hollow);

            Tunnels = Settings.GenerateTunnels ? Tunnel.CreateTunnelsAndConnectCaves(ref FinalIteration, Hollows, Settings) : new List<Tunnel>();

            Walls = GetCellGroups<WallGroup>(CellType.Wall);

            IsFinalized = true;
        }

        private ChunkCellData[,] GetNearbyChunksOrGenerateGhostChunks(out List<GhostChunkCellData> ghostChunks)
        {
            ChunkCellData[,] chunks = new ChunkCellData[3, 3];
            ghostChunks = new List<GhostChunkCellData>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1)
                    {
                        chunks[i, j] = this;

                        continue;
                    }

                    Vector3Int coordinate = new Vector3Int(ChunkCoordinate.x - 1 + i, ChunkCoordinate.y - 1 + j, 0);

                    if (_chunkManager.GeneratedChunks.TryGetValue(coordinate, out CaveChunk chunk))
                    {
                        //chunks[i, j] = chunk.CellData;
                    }
                    else
                    {
                        GhostChunkCellData ghostChunk = new GhostChunkCellData(Settings, _chunkManager, coordinate);
                        chunks[i, j] = ghostChunk;

                        ghostChunks.Add(ghostChunk);
                    }
                }
            }

            return chunks;
        }

        private List<T> GetCellGroups<T>(CellType searchedCellType) where T : CaveGroup
        {
            int length = FinalIteration.GetLength(0);
            int width = FinalIteration.GetLength(1);
            int height = FinalIteration.GetLength(2);

            List<T> result = new List<T>();

            bool[,,] markedCells = new bool[length, width, height];

            Queue<Vector3Int> cellsToSearch = new Queue<Vector3Int>();

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        if (FinalIteration[i, j, k] == searchedCellType && !markedCells[i, j, k])
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

                                            if (FinalIteration[i1, j1, k1] == searchedCellType && !markedCells[i1, j1, k1])
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

        /*private void RemoveSmallHollowGroupsByGroundSize(List<HollowGroup> hollows, int minHollowGroupGroundSize)
        {
            foreach (HollowGroup group in hollows)
            {
                if (group.GroundCells.Count < minHollowGroupGroundSize)
                {
                    foreach (Vector3Int cell in group.CellChunkCoordinates)
                    {
                        FinalIteration[cell.x, cell.y, cell.z] = CellType.Wall;
                    }
                }
            }
        }*/

        private void FilterHollowGroups(List<HollowGroup> hollows, int minHollowGroupSize)
        {
            foreach (HollowGroup group in hollows)
            {
                if (group.CellCount < minHollowGroupSize)
                {
                    foreach (Vector3Int cell in group.CellChunkCoordinates)
                    {
                        FinalIteration[cell.x, cell.y, cell.z] = CellType.Wall;
                    }
                }
            }
        }
    }
}