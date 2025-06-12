using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Develop.Editor
{
    [CustomEditor(typeof(MeshFilter))]
    public class NormalsVisualizer : UnityEditor.Editor
    {
        private Mesh mesh;
        private MeshFilter meshFilter;
        private Vector3[] vertices;
        private Vector3[] normals;
        private float normalsLength = 1f;
        private bool showNormals;

        private void OnEnable()
        {
            meshFilter = target as MeshFilter;
            if (meshFilter != null)
                mesh = meshFilter.sharedMesh;
        }

        private void OnSceneGUI()
        {
            if (!showNormals || mesh == null)
                return;

            Handles.zTest = CompareFunction.LessEqual;
            Handles.matrix = meshFilter.transform.localToWorldMatrix;
            vertices = mesh.vertices;
            normals = mesh.normals;

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 vertexPosition = vertices[i];
                Vector3 lineEndPosition = vertexPosition + normals[i] * normalsLength;

                Handles.color = Color.yellow;
                Handles.DrawLine(vertexPosition, lineEndPosition);

                Handles.color = Color.cyan;
                Handles.DrawSolidDisc(lineEndPosition, normals[i], 0.1f);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            {
                showNormals = EditorGUILayout.Toggle("Show Normals", showNormals);
                normalsLength = EditorGUILayout.FloatField("Normals Length", normalsLength);

                if (normalsLength < 0)
                    normalsLength = 0;
            }
        }
    }
}