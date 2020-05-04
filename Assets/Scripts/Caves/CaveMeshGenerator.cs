using System.Collections.Generic;
using Caves.Cells;
using UnityEngine;

namespace Caves
{
	public class CaveMeshGenerator : MonoBehaviour
	{
		public Mesh GetMesh(List<CaveWallsGroup> walls)
		{
			foreach (CaveWallsGroup group in walls)
			{
				Vector3[] vertices = new Vector3[group.CellCount];

				for (int i = 0; i < group.CellChunkCoordinates.Count; i++)
				{
					//Vector2Int coordinate = group.CellChunkCoordinates[i];
					//dvertices[i] = new Vector3(coordinate.x, coordinate.y, 0);
				}
			}

			Mesh mesh = new Mesh();
			return mesh;
		}
	}
}