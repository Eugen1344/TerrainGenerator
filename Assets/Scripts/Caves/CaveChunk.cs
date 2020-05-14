using System;
using Caves.CaveMesh;
using Caves.Cells;
using UnityEngine;

namespace Caves
{
	public class CaveChunk : Chunk
	{
		public CaveChunkCellData CellData;
		public CaveMeshSettings WallMeshSettings;
		public CaveWallsMesh WallsMesh;
		public CaveCellSettings Settings;

		public MeshFilter TestMesh;
		public MeshCollider TestCollider;

		[ContextMenu("Generate chunk")]
		public void GenerateChunk()
		{
			CheckSeed();
			
			CellData = CaveChunkCellData.GenerateCaveChunk(Settings);
			WallsMesh = CaveWallsMesh.GenerateWallMesh(CellData.Walls, WallMeshSettings);

			TestMesh.sharedMesh = WallsMesh.Mesh;
			TestCollider.sharedMesh = WallsMesh.Mesh;
		}

		private void Awake()
		{
			GenerateChunk();
		}

		private void CheckSeed()
		{
			if (Settings.RandomSeed)
				Settings.Seed = Environment.TickCount;
		}

		public Vector3 GetWorldPosition(Vector3Int cellPosition)
		{
			Vector3 localPosition = new Vector3(cellPosition.x * WallMeshSettings.CellSize.x, cellPosition.y * WallMeshSettings.CellSize.y, cellPosition.z * WallMeshSettings.CellSize.z);
			return transform.TransformPoint(localPosition); //TODO spacing
		}
	}
}