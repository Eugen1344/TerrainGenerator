using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class CaveCellsVisualizer : MonoBehaviour
{
	public CaveSettings Settings;
	public float CubeSize;
	public float CubeSpacing;
	public Color HollowColor;
	public Color WallColor;
	public GameObject CubePrefab;

	private CaveChunk _currentCaveChunk;

	private List<GameObject> _instantiatedCubes = new List<GameObject>();

	private void Start()
	{
		ShowSimulatedState();
	}

	public void GenerateCave()
	{
		_currentCaveChunk = CaveCellsGenerator.GenerateCaveChunk(Settings);
	}

	public void ShowSimulatedState()
	{
		GenerateCave();

		if (_currentCaveChunk == null)
			return;

		DrawCells();
		DrawTunnelLines();
	}

	private void DrawCells()
	{
		PlaceCubes();
	}

	private void PlaceCubes()
	{
		DeletePreviousCubes();

		Vector3 cubeSize = new Vector3(CubeSize, CubeSize, CubeSize);

		foreach (CaveWallsGroup wallsGroup in _currentCaveChunk.Walls)
		{
			foreach (Vector3Int cellCoordinates in wallsGroup.CellChunkCoordinates)
			{
				Vector3 cellPosition = CellChunkCoordinatesToWorld(cellCoordinates);

				Color cubeColor = WallColor;

				Gizmos.color = cubeColor;

				//Gizmos.DrawCube(cellPosition, cubeSize);
				CreateAndPlaceCube(cellPosition, cubeSize, cubeColor, cellCoordinates);
			}
		}
	}

	private void CreateAndPlaceCube(Vector3 position, Vector3 size, Color color, Vector3Int cellCoordinates)
	{
		GameObject cube = Instantiate(CubePrefab);

		cube.name = cellCoordinates.ToString();
		cube.transform.parent = transform;
		cube.transform.localPosition = position;
		cube.transform.localScale = size;
		cube.GetComponent<MeshRenderer>().material.color = color;

		_instantiatedCubes.Add(cube);
	}

	private void DeletePreviousCubes()
	{
		foreach (GameObject cube in _instantiatedCubes)
		{
			Destroy(cube);
		}

		_instantiatedCubes.Clear();
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying || _currentCaveChunk == null)
			return;

		for (int i = 0; i < _currentCaveChunk.Hollows.Count; i++)
		{
			CaveHollowGroup hollow = _currentCaveChunk.Hollows[i];
			Vector3 textGlobalPoint = CellChunkCoordinatesToWorld(hollow.CellChunkCoordinates[0]);

			GUIStyle textStyle = new GUIStyle();
			textStyle.fontSize = 20;

			Handles.Label(textGlobalPoint, i.ToString(), textStyle);

			foreach (Vector3Int cell in hollow.CellChunkCoordinates)
			{
				Vector3 globalFirstPoint = CellChunkCoordinatesToWorld(cell);

				Gizmos.color = GetRandomColor(i);
				Gizmos.DrawSphere(globalFirstPoint, 2);
			}
		}
	}

	private Color GetRandomColor(int i)
	{
		Random rand = new Random(i);

		return new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
	}

	private void DrawTunnelLines()
	{
		foreach (CaveTunnel tunnel in _currentCaveChunk.Tunnels)
		{
			Vector3 globalFirstPoint = CellChunkCoordinatesToWorld(tunnel.FirstCaveConnectionPoint);
			Vector3 globalSecondPoint = CellChunkCoordinatesToWorld(tunnel.SecondCaveConnectionPoint);

			Debug.DrawLine(globalFirstPoint, globalSecondPoint, Color.cyan, 1000000000);
		}
	}

	private Vector3 CellChunkCoordinatesToWorld(Vector3Int cellCoordinates)
	{
		Vector3 startingPosition = transform.position;

		return new Vector3(startingPosition.x + (cellCoordinates.x / 2f) * CubeSpacing, startingPosition.z + (cellCoordinates.z / 2f) * CubeSpacing, startingPosition.y + (cellCoordinates.y / 2f) * CubeSpacing);
	}
}