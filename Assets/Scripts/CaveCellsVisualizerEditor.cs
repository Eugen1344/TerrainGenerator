using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveCellsVisualizer))]
public class CaveCellsVisualizerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		CaveCellsVisualizer cellsVisualizer = (CaveCellsVisualizer)target;

		if (GUILayout.Button("Show initial state"))
		{
			cellsVisualizer.ShowInitialState();
		}
		if (GUILayout.Button("Show simulated state"))
		{
			cellsVisualizer.ShowSimulatedState();
		}
	}
}