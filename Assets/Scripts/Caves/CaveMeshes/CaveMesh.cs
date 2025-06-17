using Caves.Cells;
using Caves.Chunks;
using MeshGenerators;
using MeshGenerators.SurfaceNets;
using UnityEngine;

namespace Caves.CaveMeshes
{
    public class CaveMesh : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        public CaveChunk Chunk { get; private set; }
        public Vector3Int MinCoordinate { get; private set; }

        private MeshGenerator _meshGenerator;
        private WallGroup _wall;
        private BaseGeneratorSettings _settings;
        private MeshData _data;
        private Mesh _mesh;

        private void Start()
        {
            transform.localPosition += MinCoordinate * _settings.ChunkSize;

            _mesh = _meshGenerator.CreateMesh(_data);
            _meshFilter.sharedMesh = _mesh;
            _meshCollider.sharedMesh = _mesh;
        }

        public void Generate(WallGroup wall, CaveChunk chunk, BaseGeneratorSettings baseGeneratorSettings)
        {
            _wall = wall;
            _settings = baseGeneratorSettings;
            Chunk = chunk;

            _meshGenerator = new SurfaceNetsMeshGenerator(_settings, this);

            _data = GenerateMeshData(out Vector3Int minCoordinate);
            MinCoordinate = minCoordinate;
        }

        private MeshData GenerateMeshData(out Vector3Int minCoordinate)
        {
            int[,,] nodeMatrix = GetAlignedNodeMatrix(_wall, out minCoordinate);

            return _meshGenerator.Generate(nodeMatrix);
        }

        private static Vector2[] CalculateUVs(Vector3[] vertices, Vector3 size)
        {
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];

                uvs[i] = new Vector2(vertex.x / size.x, vertex.z / size.z);
            }

            return uvs;
        }

        private int[,,] GetAlignedNodeMatrix(WallGroup wall, out Vector3Int minCoordinate)
        {
            minCoordinate = wall.CellChunkCoordinates[0];
            Vector3Int maxCoordinate = minCoordinate;

            foreach (Vector3Int cell in wall.CellChunkCoordinates)
            {
                if (cell.x < minCoordinate.x)
                    minCoordinate.x = cell.x;
                else if (cell.x > maxCoordinate.x)
                    maxCoordinate.x = cell.x;

                if (cell.y < minCoordinate.y)
                    minCoordinate.y = cell.y;
                else if (cell.y > maxCoordinate.y)
                    maxCoordinate.y = cell.y;

                if (cell.z < minCoordinate.z)
                    minCoordinate.z = cell.z;
                else if (cell.z > maxCoordinate.z)
                    maxCoordinate.z = cell.z;
            }

            Vector3Int actualMatrixSize = maxCoordinate - minCoordinate + new Vector3Int(2, 2, 2);
            int[,,] nodes = new int[actualMatrixSize.x, actualMatrixSize.y, actualMatrixSize.z];

            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    for (int k = 0; k <= 1; k++)
                    {
                        Vector3Int nearbyChunkCoordinateOffset = new Vector3Int(i, j, k);
                        if (nearbyChunkCoordinateOffset == Vector3Int.zero)
                            continue;

                        FillEdgeFromNearbyChunk(ref nodes, nearbyChunkCoordinateOffset);
                    }
                }
            }

            foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
            {
                Vector3Int matrixCoordinate = coordinate - minCoordinate;

                nodes[matrixCoordinate.x, matrixCoordinate.y, matrixCoordinate.z] = 1;
            }

            return nodes;
        }

        private void FillEdgeFromNearbyChunk(ref int[,,] nodes, Vector3Int nearbyChunkCoordinateOffset)
        {
            Vector3Int nearbyChunkCoordinate = Chunk.ChunkCoordinate + nearbyChunkCoordinateOffset;
            Vector3Int invertedNearbyChunkCoordinateOffset = Vector3Int.one - nearbyChunkCoordinateOffset;
            CaveChunk nearbyChunk = _chunkManager.GetChunk(nearbyChunkCoordinate);

            Vector3Int lastCoordinate = new Vector3Int(nodes.GetLength(0) - 1, nodes.GetLength(1) - 1, nodes.GetLength(2) - 1);
            Vector3Int min = nearbyChunkCoordinateOffset * lastCoordinate;
            Vector3Int max = lastCoordinate - invertedNearbyChunkCoordinateOffset;

            for (int i = min.x; i <= max.x; i++)
            {
                for (int j = min.y; j <= max.y; j++)
                {
                    for (int k = min.z; k <= max.z; k++)
                    {
                        Vector3Int chunkCellCoordinate = new Vector3Int(i, j, k);
                        Vector3Int nearbyChunkCellCoordinate = chunkCellCoordinate * invertedNearbyChunkCoordinateOffset;

                        CellType cell = nearbyChunk.GetCell(nearbyChunkCellCoordinate);
                        nodes[i, j, k] = cell == CellType.Wall ? 1 : 0;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Destroy(_mesh);
        }
    }
}