using Caves.CaveMesh;

namespace PolygonGenerators
{
	public abstract class PolygonGenerator
	{
		public readonly PolygonGeneratorSettings Settings;

		protected PolygonGenerator(PolygonGeneratorSettings settings)
		{
			Settings = settings;
		}
	}
}