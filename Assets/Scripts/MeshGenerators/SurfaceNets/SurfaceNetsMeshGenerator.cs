using System.Collections.Generic;
using Caves.CaveMeshes;
using UnityEngine;

namespace MeshGenerators.SurfaceNets
{
    public class SurfaceNetsMeshGenerator : MeshGenerator
    {
        public SurfaceNetsMeshGenerator(BaseGeneratorSettings settings, CaveMesh caveMesh) : base(settings, caveMesh)
        {
        }

        public override MeshData Generate(int[,,] matrix)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            List<MeshGeneratorNode> surfaceNodes = GetSurfaceNodes(matrix);

            for (int i = 0; i < _settings.SmoothIterationCount; i++)
            {
                foreach (MeshGeneratorNode node in surfaceNodes)
                    node.PlaceEquidistant();
            }

            foreach (MeshGeneratorNode node in surfaceNodes)
            {
                //Vector3Int chunkPosition = node.MatrixPosition + _caveMesh.MinCoordinate;
                Vector3 scaledVertex = Vector3.Scale(node.Position, _settings.GetChunkGridSizeMultiplier());
                vertices.Add(scaledVertex);

                List<int> nodeTriangles = node.GetAllTriangles();
                triangles.AddRange(nodeTriangles);
            }

            //mesh.uv = Unwrapping.GeneratePerTriangleUV(mesh);

            return new MeshData(vertices, triangles);
        }

        private bool IsOnChunkEdge(Vector3Int coordinate)
        {
            Vector3Int chunkSize = _settings.ChunkSize;

            return coordinate.x == 0 || coordinate.y == 0 || coordinate.z == 0 ||
                   coordinate.x == chunkSize.x - 1 || coordinate.y == chunkSize.y - 1 || coordinate.z == chunkSize.z - 1;
        }

        private List<MeshGeneratorNode> GetSurfaceNodes(int[,,] matrix)
        {
            int length = matrix.GetLength(0);
            int width = matrix.GetLength(1);
            int height = matrix.GetLength(2);

            List<MeshGeneratorNode> nodes = new List<MeshGeneratorNode>();
            MeshGeneratorNode[,,] nodeMatrix = new MeshGeneratorNode[length, width, height];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        //if (i == 0 || j == 0 || k == 0 || i == length - 1 || j == width - 1 || k == height - 1)
                        //  continue;

                        int triangleIndex = nodes.Count;
                        MeshGeneratorNode node = GetSurfaceNode(matrix, i, j, k, triangleIndex);
                        nodeMatrix[i, j, k] = node;

                        if (node == null)
                            continue;

                        if (i != 0)
                        {
                            MeshGeneratorNode prevNode = nodeMatrix[i - 1, j, k];
                            prevNode?.CreateMutualLink(node);
                        }

                        if (j != 0)
                        {
                            MeshGeneratorNode prevNode = nodeMatrix[i, j - 1, k];
                            prevNode?.CreateMutualLink(node);
                        }

                        if (k != 0)
                        {
                            MeshGeneratorNode prevNode = nodeMatrix[i, j, k - 1];
                            prevNode?.CreateMutualLink(node);
                        }

                        nodes.Add(node);
                    }
                }
            }

            return nodes;
        }

        /*private MeshGeneratorNode GetNearbyChunkNode(int i, int j, int k, Vector3Int offset, int[,,] matrix)
        {
            MeshGeneratorNode node = null;

            Vector3Int nearbyChunkPosition = new Vector3Int(Wall.Chunk.ChunkCoordinate.x, Wall.Chunk.ChunkCoordinate.y, Wall.Chunk.ChunkCoordinate.z) + offset;

            if (Wall.Chunk.ChunkManager.GeneratedChunks.TryGetValue(nearbyChunkPosition, out CaveChunk nearbyChunk))
            {
                if (nearbyChunk.IsFinalized)
                {
                    Vector3Int nearbyNodeCoordinate = new Vector3Int(i + Wall.MinCoordinate.x, j + Wall.MinCoordinate.y, k + Wall.MinCoordinate.z);

                    if (offset.x == -1)
                        nearbyNodeCoordinate.x = Wall.Chunk.Settings.ChunkCubicSize.x - 1;
                    else if (offset.x == 1)
                        nearbyNodeCoordinate.x = 0;
                    else if (offset.y == -1)
                        nearbyNodeCoordinate.y = Wall.Chunk.Settings.ChunkCubicSize.y - 1;
                    else if (offset.y == 1)
                        nearbyNodeCoordinate.y = 0;
                    else if (offset.z == -1)
                        nearbyNodeCoordinate.z = Wall.Chunk.Settings.ChunkCubicSize.z - 1;
                    else if (offset.z == 1)
                        nearbyNodeCoordinate.z = 0;

                    if (nearbyChunk.EdgeNodes.TryGetValue(nearbyNodeCoordinate, out MeshGeneratorNode nearbyNode))
                    {
                        node = new MeshGeneratorNode(nearbyNode.Position, new Vector3Int(i, j, k), matrix) { IsStatic = true };
                    }
                }
            }

            return node;
        }*/

        private MeshGeneratorNode GetSurfaceNode(int[,,] matrix, int i0, int j0, int k0, int triangleIndex)
        {
            int node0 = matrix[i0, j0, k0];
            int node1 = matrix[i0 + 1, j0, k0];
            int node2 = matrix[i0 + 1, j0 + 1, k0];
            int node3 = matrix[i0, j0 + 1, k0];

            int node4 = matrix[i0, j0, k0 + 1];
            int node5 = matrix[i0 + 1, j0, k0 + 1];
            int node6 = matrix[i0 + 1, j0 + 1, k0 + 1];
            int node7 = matrix[i0, j0 + 1, k0 + 1];

            int nodeSum = node0 + node1 + node2 + node3 + node4 + node5 + node6 + node7;

            if (nodeSum == 0 || nodeSum == 8)
                return null;

            return new MeshGeneratorNode(new Vector3(i0, j0, k0), new Vector3Int(i0, j0, k0), matrix, triangleIndex);
        }
    }
}