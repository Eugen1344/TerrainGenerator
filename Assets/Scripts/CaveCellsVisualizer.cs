﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CaveCellsVisualizer : MonoBehaviour
{
	public CaveCellsGenerator.CaveSettings Settings;
	public float CubeSize;
	public float CubeSpacing;
	public Color ActiveColor;
	public Color InactiveColor;

	private int[,] _initialState;
	private int[,] _simulatedState;

	private int[,] _currentDrawnCells;

	private List<GameObject> _instantiatedCubes = new List<GameObject>();

	private void Start()
	{
		ShowSimulatedState();
	}

	public void GenerateCave()
	{
		_initialState = CaveCellsGenerator.GetInitialState(Settings);
		_simulatedState = CaveCellsGenerator.GetSimulatedCaves(_initialState);
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

	private void DrawCells(int[,] cells)
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
				int cell = _currentDrawnCells[i, j];
				bool isCellActive = CaveCellsGenerator.IsCellActive(cell);

				Vector3 cellPosition = new Vector3(startingPosition.x + (i - height / 2) * CubeSpacing, startingPosition.y + (j - width / 2) * CubeSpacing, isCellActive ? CubeSpacing : 0);

				Color cubeColor = isCellActive ? ActiveColor : InactiveColor;

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