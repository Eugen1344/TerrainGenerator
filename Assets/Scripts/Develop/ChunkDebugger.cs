using Caves.Cells;
using Caves.Chunks;
using UnityEditor;
using UnityEngine;

namespace Develop
{
    public class ChunkDebugger : MonoBehaviour
    {
        public bool DrawCells;
        public bool DrawCoordinates;
        public bool DrawTunnels;
        public CaveChunk Chunk;

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            if (DrawCells)
            {
                foreach (WallGroup wall in Chunk.CellData.Walls)
                {
                    foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
                    {
                        Vector3 globalPosition = Chunk.GetWorldPosition(coordinate);

                        Gizmos.color = Color.cyan;
                        Gizmos.DrawSphere(globalPosition, 3);

                        if (DrawCoordinates)
                        {
#if UNITY_EDITOR
                            Handles.Label(globalPosition, coordinate.ToString());
#endif
                        }
                    }
                }
            }

            if (DrawTunnels)
            {
                /*foreach (Tunnel tunnel in Chunk.CellData.Tunnels)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(Chunk.GetWorldPosition(tunnel.FirstCaveConnectionPoint), Chunk.GetWorldPosition(tunnel.SecondCaveConnectionPoint));
                }*/
            }
        }
    }
}