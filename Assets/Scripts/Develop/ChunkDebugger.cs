using Caves;
using Caves.Cells;
using UnityEditor;
using UnityEngine;

public class ChunkDebugger : MonoBehaviour
{
	public bool DrawCells;
	public bool DrawCoordinates;
	public CaveChunk Chunk;

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying || !DrawCells)
			return;

		foreach (CaveWallsGroup wall in Chunk.CellData.Walls)
		{
			foreach (Vector3Int coordinate in wall.CellChunkCoordinates)
			{
				Vector3 globalPosition = Chunk.GetWorldPosition(coordinate);

				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere(globalPosition, 3);

				if (DrawCoordinates)
				{
					Handles.Label(globalPosition, coordinate.ToString());
				}
			}
		}
	}
}