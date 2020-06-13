using Caves.CaveMesh;
using UnityEngine;

namespace PolygonGenerators
{
	public abstract class MeshGenerator
	{
		public readonly PolygonGeneratorSettings Settings;

		public abstract Mesh Generate(int[,,] nodeMatrix);

		protected MeshGenerator(PolygonGeneratorSettings settings)
		{
			Settings = settings;
		}
	}
}