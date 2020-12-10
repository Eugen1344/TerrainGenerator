using UnityEngine;

namespace MeshGenerators.MarchingCubes
{
	public class MeshGeneratorPrimaryNode : MeshGeneratorNode
	{
		public Vector3 Position;

		public MeshGeneratorPrimaryNode(Vector3 position)
		{
			Position = position;
		}
	}
}