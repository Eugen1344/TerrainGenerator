using System;
using UnityEngine;

namespace MeshGenerators
{
	[Serializable]
	public struct MeshGeneratorSettings
	{
		public Vector3Int GridSize;
		public int SmoothIterationCount;
	}
}