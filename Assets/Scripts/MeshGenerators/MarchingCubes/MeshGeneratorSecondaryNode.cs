using UnityEngine;

namespace MeshGenerators.MarchingCubes
{
    public class MeshGeneratorSecondaryNode : MeshGeneratorNode
    {
        public MeshGeneratorPrimaryNode FirstNode;
        public MeshGeneratorPrimaryNode SecondNode;

        public MeshGeneratorSecondaryNode(MeshGeneratorPrimaryNode firstNode, MeshGeneratorPrimaryNode secondNode)
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