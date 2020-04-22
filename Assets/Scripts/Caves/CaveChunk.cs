using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveChunk : Chunk
{
	public CellType[,,] Cells;
	public List<CaveHollowGroup> Hollows;
	public List<CaveWallsGroup> Walls;

	public CaveChunk(CellType[,,] cells)
	{
		Cells = cells;

		Hollows = GetCellGroups<CaveHollowGroup>(cells, CellType.Hollow).ToList();
		Walls = GetCellGroups<CaveWallsGroup>(cells, CellType.Wall).ToList();
	}

	public CaveChunk(List<CaveHollowGroup> hollows, List<CaveWallsGroup> walls)
	{
		Hollows = hollows;
		Walls = walls;
	}

	private static List<T> GetCellGroups<T>(CellType[,,] cells, CellType searchedCellType) where T : CaveGroup
	{
		int length = cells.GetLength(0);
		int width = cells.GetLength(1);
		int height = cells.GetLength(2);

		List<T> result = new List<T>();

		bool[,,] markedCells = new bool[length, width, height];

		Queue<Vector3Int> cellsToSearch = new Queue<Vector3Int>();

		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < height; k++)
				{
					if (cells[i, j, k] == searchedCellType && !markedCells[i, j, k])
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
											(i1 != searchCoreCell.x && j1 != searchCoreCell.y && k1 != searchCoreCell.z))
											continue;

										if (cells[i1, j1, k1] == searchedCellType && !markedCells[i1, j1, k1])
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