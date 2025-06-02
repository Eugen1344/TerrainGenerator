using UnityEditor;
using UnityEngine;

namespace Caves.Cells
{
    [CustomEditor(typeof(CellsVisualizer))]
    public class CaveCellsVisualizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CellsVisualizer cellsVisualizer = (CellsVisualizer)target;

            if (Application.isPlaying && GUILayout.Button("Show simulated state"))
            {
                cellsVisualizer.ShowSimulatedState();
            }
        }
    }
}