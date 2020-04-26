using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

		DrawCells();
	}

	private void DrawCells()
	{
		PlaceCubes();
	}

	private void PlaceCubes()
	{
		if (_currentCaveChunk == null)
			return;

		DeletePreviousCubes();

		Vector3 startingPosition = transform.position;
		Vector3 cubeSize = new Vector3(CubeSize, CubeSize, CubeSize);

		foreach (CaveWallsGroup wallsGroup in _currentCaveChunk.Walls)
		{
			foreach (Vector3Int cellCoordinates in wallsGroup.CellChunkCoordinates)
			{
				Vector3 cellPosition = new Vector3(startingPosition.x + (cellCoordinates.x / 2f) * CubeSpacing, startingPosition.z + (cellCoordinates.z / 2f) * CubeSpacing, startingPosition.y + (cellCoordinates.y / 2f) * CubeSpacing);

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
}