using UnityEngine;

namespace MeshGenerators
{
	public class MarchingCubesPrimaryNode : MarchingCubesNode
	{
		public Vector3 Position;

		public MarchingCubesPrimaryNode(Vector3 position)
		{
			Position = position;
		}
	}
}