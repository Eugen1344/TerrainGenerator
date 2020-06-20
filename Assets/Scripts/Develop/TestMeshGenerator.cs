using System;
using MeshGenerators;
using UnityEngine;

namespace Develop
{
	public class TestMeshGenerator : MonoBehaviour
	{
		public Vector3Int GridSize;
		public int SphereRadius;
		public MeshGenerator MeshGenerator;

		private void Awake()
		{
			MeshGenerator = new MarchingCubesMeshGenerator(new MeshGeneratorSettings());
		}

		[ContextMenu("Generate sphere")]
		public void GenerateSphere()
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();

			Mesh mesh = MeshGenerator.Generate(GetNodeMatrix(), GridSize);
			meshFilter.sharedMesh = mesh;
		}

		private int[,,] GetNodeMatrix()
		{
			int sphereDiameter = SphereRadius * 2;
			int[,,] matrix = new int[sphereDiameter, sphereDiameter, sphereDiameter];

			Vector3Int sphereCenter = new Vector3Int(SphereRadius, SphereRadius, SphereRadius);

			for (int i = 0; i < sphereDiameter; i++)
			{
				for (int j = 0; j < sphereDiameter; j++)
				{
					for (int k = 0; k < sphereDiameter; k++)
					{
						Vector3Int currentCoordinate = new Vector3Int(i, j, k);

						if (Vector3Int.Distance(currentCoordinate, sphereCenter) <= SphereRadius)
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