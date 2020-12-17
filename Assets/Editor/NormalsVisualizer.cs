using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class NormalsVisualizer : Editor
{
	private Mesh mesh;

	void OnEnable()
	{
		MeshFilter mf = target as MeshFilter;
		if (mf != null)
		{
			mesh = mf.sharedMesh;
		}
	}

	void OnSceneGUI()
	{
		if (mesh == null)
		{
			return;
		}

		for (int i = 0; i < mesh.normals.Length; i++)
		{
			Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
			Handles.color = Color.yellow;

			Vector3 normal = mesh.normals[i];
			Vector3 normalStart = mesh.vertices[i];
			Vector3 normalEnd = mesh.vertices[i] + normal;
			Handles.DrawLine(normalStart, normalEnd);

			Handles.color = Color.blue;
			Handles.DrawWireDisc(normalEnd, normal, 0.1f);
		}
	}
}