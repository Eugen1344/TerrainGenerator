using UnityEngine;

namespace MeshGenerators
{
	public abstract class MeshGenerator
	{
		public readonly MeshGeneratorSettings Settings;

		public abstract Mesh Generate(int[,,] nodeMatrix, Vector3Int gridSize);

		protected MeshGenerator(MeshGeneratorSettings settings)
		{
			Settings = settings;
		}
	}
}