using UnityEditor;
using UnityEngine;

namespace Caves.Cells
{
	[CustomEditor(typeof(CaveCellsVisualizer))]
	public class CaveCellsVisualizerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			CaveCellsVisualizer cellsVisualizer = (CaveCellsVisualizer)target;

			if (Application.isPlaying && GUILayout.Button("Show simulated state"))
			{
				cellsVisualizer.ShowSimulatedState();
			}
		}
	}
}