﻿using System.Collections.Generic;
using Caves.Chunks;
using UnityEngine;

namespace Caves.Cells.CellularAutomata
{
	public class GhostChunkCellData : ChunkCellData
	{
		public ChunkCellData[,] NearbyChunks;

		public GhostChunkCellData(CellSettings settings, CaveChunkManager chunkManager, Vector3Int chunkCoordinate) : base(settings, chunkManager, chunkCoordinate)
		{
		}

		public void FindNearbyChunks(List<GhostChunkCellData> ghostChunks)
		{
			ChunkCellData[,] chunks = new ChunkCellData[3, 3];

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
						chunks[i, j] = ghostChunks.Find(ghostChunk => ghostChunk.ChunkCoordinate == coordinate);
					}
				}
			}

			NearbyChunks = chunks;
		}
	}
}