using UnityEngine;

namespace MeshGenerators
{
	public abstract class MeshGenerator
	{
		public readonly MeshGeneratorSettings Settings;

		public abstract Mesh Generate(int[,,] nodeMatrix);

		protected MeshGenerator(MeshGeneratorSettings settings)
		{
			Settings = settings;
		}
	}
}