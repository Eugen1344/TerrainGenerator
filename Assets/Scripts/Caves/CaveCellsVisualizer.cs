using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CaveCellsVisualizer : MonoBehaviour
{
	public CaveCellsGenerator.CaveSettings Settings;
	public float CubeSize;
	public float CubeSpacing;
	public Color HollowColor;
	public Color WallColor;

	private CellType[,] _initialState;
	private CellType[,] _simulatedState;

	private CellType[,] _currentDrawnCells;

	private List<GameObject> _instantiatedCubes = new List<GameObject>();

	private void Start()
	{
		ShowSimulatedState();

		CaveCellsGenerator.GenerateCaveChunk(Settings);
	}

	public void GenerateCave()
	{
		_initialState = CaveCellsGenerator.GetInitialState(Settings);
		_simulatedState = CaveCellsGenerator.GetSimulatedCells(_initialState, Settings);
	}

	public void ShowInitialState()
	{
		GenerateCave();

		DrawCells(_initialState);
	}

	public void ShowSimulatedState()
	{
		GenerateCave();

		DrawCells(_simulatedState);
	}

	private void DrawCells(CellType[,] cells)
	{
		_currentDrawnCells = cells;

		PlaceCubes();
	}

	private void PlaceCubes()
	{
		if (_currentDrawnCells == null)
			return;

		DeletePreviousCubes();

		Vector3 startingPosition = transform.position;
		Vector3 cubeSize = new Vector3(CubeSize, CubeSize, CubeSize);

		int height = _currentDrawnCells.GetLength(0);
		int width = _currentDrawnCells.GetLength(1);

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				CellType cell = _currentDrawnCells[i, j];
				bool isHollowCell = CaveCellsGenerator.IsHollowCell(cell);

				Vector3 cellPosition = new Vector3(startingPosition.x + (i - height / 2) * CubeSpacing, startingPosition.y + (j - width / 2) * CubeSpacing, isHollowCell ? CubeSpacing : 0);

				Color cubeColor = isHollowCell ? HollowColor : WallColor;

				Gizmos.color = cubeColor;

				//Gizmos.DrawCube(cellPosition, cubeSize);
				CreateAndPlaceCube(cellPosition, cubeSize, cubeColor);
			}
		}
	}

	private void CreateAndPlaceCube(Vector3 position, Vector3 size, Color color)
	{
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

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