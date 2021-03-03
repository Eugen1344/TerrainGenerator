using System.Collections.Generic;
using System.Threading.Tasks;
using Caves.Chunks;
using SimplexNoise;
using UnityEngine;

namespace Caves.Cells.SimplexNoise
{
	public class ChunkCellData
	{
		public readonly CellSettings Settings;
		public readonly Vector3Int ChunkCoordinate;
		//public readonly int ChunkSeed;

		protected CaveChunkManager _chunkManager;

		public CellType[,,] Cells;

		public List<HollowGroup> Hollows;
		public List<WallGroup> Walls;
		public List<Tunnel> Tunnels;

		public bool IsFinalized = false;

		private Noise _noiseGenerator;

		public ChunkCellData(CellSettings settings, CaveChunkManager chunkManager, Vector3Int chunkCoordinate)
		{
			_chunkManager = chunkManager;

			Settings = settings;
			ChunkCoordinate = chunkCoordinate;
			//ChunkSeed = Settings.GenerateSeed(Settings.Seed, chunkCoordinate);

			_noiseGenerator = new Noise(Settings.Seed);
		}

		public async Task Generate()
		{
			float[,,] noise = await Task.Run(GetNoise);
			Cells = await Task.Run(() => GetCellsFromNoise(noise));

			Task<List<HollowGroup>> getHollowGroupTask = GetCellGroupsAsync<HollowGroup>(CellType.Hollow);
			Hollows = await getHollowGroupTask;

			Hollows = await Task.Run(() => RemoveSmallHollowGroups(Hollows, ref Cells, Settings.MinCaveSize));

			Tunnels = Settings.GenerateTunnels ? Tunnel.CreateTunnelsAndConnectCaves(ref Cells, Hollows, Settings) : new List<Tunnel>();

			Task<List<WallGroup>> getWallGroupTask = GetCellGroupsAsync<WallGroup>(CellType.Wall);

			Walls = await getWallGroupTask;
		}

		private float[,,] GetNoise()
		{
			float[,,] noise = new float[Settings.ChunkCubicSize.x, Settings.ChunkCubicSize.y, Settings.ChunkCubicSize.z];

			Vector3Int offset = Settings.ChunkCubicSize * ChunkCoordinate;

			for (int i = 0; i < Settings.ChunkCubicSize.x; i++)
			{
				for (int j = 0; j < Settings.ChunkCubicSize.y; j++)
				{
					for (int k = 0; k < Settings.ChunkCubicSize.z; k++)
					{
						Vector3Int noisePixelPosition = new Vector3Int(i + offset.x, j + offset.y, k + offset.z);

						noise[i, j, k] = _noiseGenerator.CalcPixel3D(noisePixelPosition.x, noisePixelPosition.y, noisePixelPosition.z, Settings.NoiseScale);
					}
				}
			}

			return noise;
		}

		public float GetRandomHollowCellsPercent(Vector3Int globalPixelCoordinate)
		{
			int heightDiff = Mathf.Abs(globalPixelCoordinate.z - Settings.CentralDecreasePoint.z);

			return Mathf.Clamp(Settings.RandomHollowCellsPercent - heightDiff * Settings.RandomHollowCellsPercentDecreasePerPixel, 0, Settings.RandomHollowCellsPercent);
		}

		public void FinalizeGeneration()
		{
			IsFinalized = true;
		}

		private List<HollowGroup> RemoveSmallHollowGroups(List<HollowGroup> hollows, ref CellType[,,] cells, int minCaveSize)
		{
			List<HollowGroup> filteredHollows = new List<HollowGroup>();

			foreach (HollowGroup hollow in hollows)
			{
				if (hollow.CellCount < minCaveSize)
				{
					foreach (Vector3Int coordinate in hollow.CellChunkCoordinates)
					{
						cells[coordinate.x, coordinate.y, coordinate.z] = CellType.Wall;
					}
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

			Vector3Int offset = Settings.ChunkCubicSize * ChunkCoordinate;

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

		private async Task<List<T>> GetCellGroupsAsync<T>(CellType searchedCellType) where T : CaveGroup
		{
			return await Task.Run(() => GetCellGroups<T>(searchedCellType));
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
											if ((i1 == searchCoreCell.x && j1 == searchCoreCell.y && k1 == searchCoreCell.z) ||
												(i1 != searchCoreCell.x && j1 != searchCoreCell.y))
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

		public static bool IsHollowCell(CellType cell)
		{
			return cell == CellType.Hollow;
		}

		public static bool IsWallCell(CellType cell)
		{
			return cell == CellType.Wall;
		}
	}
}
