using Caves;
using Caves.Cells;
using UnityEditor;
using UnityEngine;

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
			foreach (CaveWallsGroup wall in Chunk.CellData.Walls)
			{
				foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
				{
					Vector3 globalPosition = Chunk.GetWorldPosition(coordinate);

					Gizmos.color = Color.black;
					Gizmos.DrawSphere(globalPosition, 3);

					if (DrawCoordinates)
					{
						Handles.Label(globalPosition, coordinate.ToString());
					}
				}
			}
		}

		if (DrawTunnels)
		{
			foreach (CaveTunnel tunnel in Chunk.CellData.Tunnels)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(Chunk.GetWorldPosition(tunnel.FirstCaveConnectionPoint), Chunk.GetWorldPosition(tunnel.SecondCaveConnectionPoint));
			}
		}
	}
}