using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerators.SurfaceNets
{
    public class MeshGeneratorNode
    {
        private const int NodeCount = 6;
        private const int TriangleCount = 12;

        public Vector3 Position;
        public Vector3Int MatrixPosition;
        public int TriangleIndex;

        private readonly Vector3 _minPosition;
        private readonly Vector3 _maxPosition;

        public readonly List<MeshGeneratorNode> LinkedNodes = new List<MeshGeneratorNode>(NodeCount);

        public MeshGeneratorNode(Vector3 position, Vector3Int matrixPosition, int triangleIndex)
        {
            Position = position;
            MatrixPosition = matrixPosition;
            TriangleIndex = triangleIndex;

            _minPosition = MatrixPosition - new Vector3(0.5f, 0.5f, 0.5f);
            _maxPosition = MatrixPosition + new Vector3(0.5f, 0.5f, 0.5f);
        }

        public void CreateMutualLink(MeshGeneratorNode node)
        {
            LinkedNodes.Add(node);
            node.LinkedNodes.Add(this);
        }

        public List<int> GetAllTriangles(int[,,] pointsMatrix)
        {
            List<int> triangles = new List<int>(TriangleCount * 3);

            for (int i = 0; i < LinkedNodes.Count; i++)
            {
                MeshGeneratorNode firstNode = LinkedNodes[i];

                for (int j = i + 1; j < LinkedNodes.Count; j++)
                {
                    MeshGeneratorNode secondNode = LinkedNodes[j];

                    //if ((firstNode.HaveCreatedTriangle && !secondNode.HaveCreatedTriangle) || (!firstNode.HaveCreatedTriangle && secondNode.HaveCreatedTriangle))
                    //continue;

                    if (firstNode.MatrixPosition.x == secondNode.MatrixPosition.x && firstNode.MatrixPosition.y != secondNode.MatrixPosition.y && firstNode.MatrixPosition.z != secondNode.MatrixPosition.z
                        || firstNode.MatrixPosition.y == secondNode.MatrixPosition.y && firstNode.MatrixPosition.x != secondNode.MatrixPosition.x && firstNode.MatrixPosition.z != secondNode.MatrixPosition.z
                        || firstNode.MatrixPosition.z == secondNode.MatrixPosition.z && firstNode.MatrixPosition.x != secondNode.MatrixPosition.x && firstNode.MatrixPosition.y != secondNode.MatrixPosition.y)
                    {
                        Vector3Int oppositeMatrixPoint = firstNode.MatrixPosition + secondNode.MatrixPosition - MatrixPosition;

                        Vector3Int minCoordinate = new Vector3Int(Mathf.Min(MatrixPosition.x, firstNode.MatrixPosition.x, secondNode.MatrixPosition.x, oppositeMatrixPoint.x) + 1,
                            Mathf.Min(MatrixPosition.y, firstNode.MatrixPosition.y, secondNode.MatrixPosition.y, oppositeMatrixPoint.y) + 1,
                            Mathf.Min(MatrixPosition.z, firstNode.MatrixPosition.z, secondNode.MatrixPosition.z, oppositeMatrixPoint.z) + 1);
                        Vector3Int maxCoordinate = new Vector3Int(Mathf.Max(MatrixPosition.x, firstNode.MatrixPosition.x, secondNode.MatrixPosition.x, oppositeMatrixPoint.x),
                            Mathf.Max(MatrixPosition.y, firstNode.MatrixPosition.y, secondNode.MatrixPosition.y, oppositeMatrixPoint.y),
                            Mathf.Max(MatrixPosition.z, firstNode.MatrixPosition.z, secondNode.MatrixPosition.z, oppositeMatrixPoint.z));

                        int firstCommonPoint = pointsMatrix[minCoordinate.x, minCoordinate.y, minCoordinate.z];
                        int secondCommonPoint = pointsMatrix[maxCoordinate.x, maxCoordinate.y, maxCoordinate.z];

                        if (firstCommonPoint == secondCommonPoint)
                            continue;

                        triangles.Add(TriangleIndex);

                        Vector3Int normal = Cross(firstNode.MatrixPosition - MatrixPosition, secondNode.MatrixPosition - MatrixPosition);

                        if (firstCommonPoint > secondCommonPoint)
                        {
                            if (normal + minCoordinate == maxCoordinate)
                            {
                                triangles.Add(firstNode.TriangleIndex);
                                triangles.Add(secondNode.TriangleIndex);
                            }
                            else
                            {
                                triangles.Add(secondNode.TriangleIndex);
                                triangles.Add(firstNode.TriangleIndex);
                            }
                        }
                        else
                        {
                            if (normal + maxCoordinate == minCoordinate)
                            {
                                triangles.Add(firstNode.TriangleIndex);
                                triangles.Add(secondNode.TriangleIndex);
                            }
                            else
                            {
                                triangles.Add(secondNode.TriangleIndex);
                                triangles.Add(firstNode.TriangleIndex);
                            }
                        }
                    }
                }
            }

            return triangles;
        }

        public void PlaceEquidistant()
        {
            Vector3 equidistantPosition = Vector3.zero;

            foreach (MeshGeneratorNode linkedNode in LinkedNodes)
                equidistantPosition += linkedNode.Position;

            Position = ConstraintPosition(equidistantPosition / LinkedNodes.Count);
        }

        public Vector3 ConstraintPosition(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _minPosition.x, _maxPosition.x);
            position.y = Mathf.Clamp(position.y, _minPosition.y, _maxPosition.y);
            position.z = Mathf.Clamp(position.z, _minPosition.z, _maxPosition.z);

            return position;
        }

        public static Vector3Int Cross(Vector3Int first, Vector3Int second)
        {
            return new Vector3Int(first.y * second.z - first.z * second.y, first.z * second.x - first.x * second.z, first.x * second.y - first.y * second.x);
        }
    }
}