﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Caves.Cells
{
    public abstract class CaveGroup
    {
        public List<Vector3Int> CellChunkCoordinates;
        public int CellCount => CellChunkCoordinates.Count;

        protected CaveGroup(List<Vector3Int> cellChunkCoordinates)
        {
            CellChunkCoordinates = cellChunkCoordinates;
        }

        public Vector3Int GetLowestPoint()
        {
            return CellChunkCoordinates.OrderBy(coord => coord.y).First();
        }

        public static CaveGroup GetCaveGroup(CellType cellType, List<Vector3Int> cellChunkCoordinates)
        {
            switch (cellType)
            {
                case CellType.Wall:
                    return new WallGroup(cellChunkCoordinates);
                case CellType.Hollow:
                    return new HollowGroup(cellChunkCoordinates);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
            }
        }
    }
}