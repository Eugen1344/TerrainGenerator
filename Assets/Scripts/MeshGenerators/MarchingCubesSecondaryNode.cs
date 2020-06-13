using UnityEngine;

namespace MeshGenerators
{
	public class MarchingCubesSecondaryNode : MarchingCubesNode
	{
		public MarchingCubesPrimaryNode FirstNode;
		public MarchingCubesPrimaryNode SecondNode;

		public MarchingCubesSecondaryNode(MarchingCubesPrimaryNode firstNode, MarchingCubesPrimaryNode secondNode)
		{
			FirstNode = firstNode;
			SecondNode = secondNode;
		}

		public Vector3 InterpolatePosition(float k)
		{
			return (SecondNode.Position + FirstNode.Position) / 2f;
		}
	}
}