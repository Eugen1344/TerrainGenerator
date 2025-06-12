using System;
using UnityEngine;

namespace Caves.Cells.SimplexNoise
{
    [Serializable]
    public struct CavesGeneratorSettings
    {
        [field: SerializeField] public int MinCaveSize { get; private set; }
        [field: SerializeField] [Range(0, 1)] public float HollowCellThreshold { get; private set; }
        [field: SerializeField] public float HollowCellThresholdDecreasePerHeight { get; private set; }
        [field: SerializeField] public int RandomHollowCellsPercentDecreaseStartingHeight { get; private set; }

        [field: SerializeField] public bool GenerateTunnels;
        [field: SerializeField] public int TunnelRadius;

        public int GenerateSeed(int baseSeed, Vector3Int chunkCoordinate)
        {
            byte chunkCoordinateX = (byte)chunkCoordinate.x; //TODO change to Random(long)
            byte chunkCoordinateY = (byte)chunkCoordinate.y;
            byte chunkCoordinateZ = (byte)chunkCoordinate.z;

            int chunkOffset = chunkCoordinateX << sizeof(byte) * 2 * 8 | chunkCoordinateY << sizeof(byte) * 8 | chunkCoordinateZ; //TODO test this

            return baseSeed + chunkOffset;
        }

        public class InvalidCaveCellSettingsException : Exception
        {
            public InvalidCaveCellSettingsException(string message) : base(message)
            {
            }
        }
    }
}