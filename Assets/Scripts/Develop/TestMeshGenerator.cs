using MeshGenerators;
using MeshGenerators.MarchingCubes;
using MeshGenerators.SurfaceNets;
using UnityEngine;

namespace Develop
{
    public class TestMeshGenerator : MonoBehaviour
    {
        public MeshGeneratorSettings Settings;
        public int SphereRadius;

        [ContextMenu("Marching cubes sphere")]
        public void MarchingCubesSphere()
        {
            MeshGenerator generator = new MarchingCubesMeshGenerator(Settings, null);

            GenerateSphere(generator);
        }

        [ContextMenu("Surface nets sphere")]
        public void SurfaceNetsSphere()
        {
            MeshGenerator generator = new SurfaceNetsMeshGenerator(Settings, null);

            GenerateSphere(generator);
        }

        public void GenerateSphere(MeshGenerator generator)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            Mesh mesh = generator.CreateMesh(generator.Generate(GetSphereMatrix()));
            meshFilter.sharedMesh = mesh;
        }

        private int[,,] GetCubeMatrix(int cubeSize)
        {
            int[,,] nodeMatrix = new int[cubeSize + 2, cubeSize + 2, cubeSize + 2];

            for (int i = 1; i < cubeSize + 1; i++)
            {
                for (int j = 1; j < cubeSize + 1; j++)
                {
                    for (int k = 1; k < cubeSize + 1; k++)
                    {
                        nodeMatrix[i, j, k] = 1;
                    }
                }
            }

            return nodeMatrix;
        }

        private int[,,] GetSphereMatrix()
        {
            int sphereDiameter = SphereRadius * 2;
            int[,,] matrix = new int[sphereDiameter + 2, sphereDiameter + 2, sphereDiameter + 2];

            Vector3Int sphereCenter = new Vector3Int(SphereRadius, SphereRadius, SphereRadius) + Vector3Int.one;

            for (int i = 1; i < sphereDiameter; i++)
            {
                for (int j = 1; j < sphereDiameter; j++)
                {
                    for (int k = 1; k < sphereDiameter; k++)
                    {
                        Vector3Int currentCoordinate = new Vector3Int(i, j, k);

                        float distanceToCenter = Vector3Int.Distance(currentCoordinate, sphereCenter);
                        if (distanceToCenter <= SphereRadius)
                        {
                            matrix[i, j, k] = 1;
                        }
                    }
                }
            }

            return matrix;
        }
    }
}