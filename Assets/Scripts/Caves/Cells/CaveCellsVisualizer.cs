﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Caves.Cells
{
	public class CaveCellsVisualizer : MonoBehaviour
	{
		public CaveCellSettings Settings;
		public float CubeSize;
		public float CubeSpacing;
		public Color HollowColor;
		public Color WallColor;
		public GameObject CubePrefab;

		private CaveChunkCellData _currentCaveCells;

		private List<GameObject> _instantiatedCubes = new List<GameObject>();

		private void Start()
		{
			ShowSimulatedState();
		}

		public void GenerateCave()
		{
			_currentCaveCells = CaveChunkCellData.GenerateCaveChunk(Settings);
		}

		public void ShowSimulatedState()
		{
			GenerateCave();

			if (_currentCaveCells == null)
				return;

			DrawCells();
		}

		private void DrawCells()
		{
			PlaceCubes();
		}

		private void PlaceCubes()
		{
			DeletePreviousCubes();

			Vector3 cubeSize = new Vector3(CubeSize, CubeSize, CubeSize);

			foreach (CaveWallsGroup wallsGroup in _currentCaveCells.Walls)
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
			if (!Application.isPlaying || _currentCaveCells == null)
				return;

			DrawHollows();
			DrawTunnelLines();
		}

		private Color GetRandomColor(int i)
		{
			Random rand = new Random(Settings.Seed + i);

			return new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
		}

		private void DrawHollows()
		{
			for (int i = 0; i < _currentCaveCells.Hollows.Count; i++)
			{
				CaveHollowGroup hollow = _currentCaveCells.Hollows[i];
				Vector3 textGlobalPoint = CellChunkCoordinatesToWorld(hollow.CellChunkCoordinates[0]);

				GUIStyle textStyle = new GUIStyle();
				textStyle.fontSize = 20;

#if UNITY_EDITOR
				Handles.Label(textGlobalPoint, i.ToString(), textStyle);
#endif

				foreach (Vector3Int cell in hollow.CellChunkCoordinates)
				{
					Vector3 globalFirstPoint = CellChunkCoordinatesToWorld(cell);

					Gizmos.color = GetRandomColor(i);
					Gizmos.DrawSphere(globalFirstPoint, 2);
				}
			}
		}

		private void DrawTunnelLines()
		{
			foreach (CaveTunnel tunnel in _currentCaveCells.Tunnels)
			{
				Vector3 globalFirstPoint = CellChunkCoordinatesToWorld(tunnel.FirstCaveConnectionPoint);
				Vector3 globalSecondPoint = CellChunkCoordinatesToWorld(tunnel.SecondCaveConnectionPoint);

				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(globalFirstPoint, globalSecondPoint);
			}
		}

		private Vector3 CellChunkCoordinatesToWorld(Vector3Int cellCoordinates)
		{
			Vector3 startingPosition = transform.position;

			return new Vector3(startingPosition.x + (cellCoordinates.x / 2f) * CubeSpacing, startingPosition.z + (cellCoordinates.z / 2f) * CubeSpacing, startingPosition.y + (cellCoordinates.y / 2f) * CubeSpacing);
		}
	}
}