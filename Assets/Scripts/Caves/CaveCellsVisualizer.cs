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

	private CellType[,,] _currentDrawnCells;

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
		_currentDrawnCells = _currentCaveChunk.Cells;

		PlaceCubes();
	}

	private void PlaceCubes()
	{
		if (_currentDrawnCells == null)
			return;

		DeletePreviousCubes();

		Vector3 startingPosition = transform.position;
		Vector3 cubeSize = new Vector3(CubeSize, CubeSize, CubeSize);

		int length = _currentDrawnCells.GetLength(0);
		int width = _currentDrawnCells.GetLength(1);
		int height = _currentDrawnCells.GetLength(2);

		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < height; k++)
				{
					CellType cell = _currentDrawnCells[i, j, k];
					bool isHollowCell = CaveCellsGenerator.IsHollowCell(cell);

					if (isHollowCell)
						continue;

					Vector3 cellPosition = new Vector3(startingPosition.x + (i / 2f) * CubeSpacing, startingPosition.z + (k / 2f) * CubeSpacing, startingPosition.y + (j / 2f) * CubeSpacing);

					Color cubeColor = WallColor;

					Gizmos.color = cubeColor;

					//Gizmos.DrawCube(cellPosition, cubeSize);
					CreateAndPlaceCube(cellPosition, cubeSize, cubeColor);
				}
			}
		}
	}

	private void CreateAndPlaceCube(Vector3 position, Vector3 size, Color color)
	{
		GameObject cube = Instantiate(CubePrefab);

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