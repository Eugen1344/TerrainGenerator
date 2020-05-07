using UnityEngine;

namespace Caves.CaveMesh
{
	public class CaveMeshSecondaryNode : CaveMeshNode
	{
		public CaveMeshPrimaryNode FirstNode;
		public CaveMeshPrimaryNode SecondNode;

		public CaveMeshSecondaryNode(CaveMeshPrimaryNode firstNode, CaveMeshPrimaryNode secondNode)
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
