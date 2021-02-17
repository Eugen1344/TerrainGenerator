using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Random = System.Random;

namespace Caves.Cells.SimplexNoise
{
	public class Tunnel
	{
		public List<Vector3Int> CellChunkCoordinates;
		public HollowGroup _first;
		public HollowGroup _second;
		public Vector3Int FirstCaveConnectionPoint;
		public Vector3Int SecondCaveConnectionPoint;

		public Tunnel(List<Vector3Int> cellChunkCoordinates, HollowGroup first, HollowGroup second, Vector3Int firstCaveConnectionPoint, Vector3Int secondCaveConnectionPoint)
		{
			CellChunkCoordinates = cellChunkCoordinates;
			_first = first;
			_second = second;
			FirstCaveConnectionPoint = firstCaveConnectionPoint;
			SecondCaveConnectionPoint = secondCaveConnectionPoint;
		}

		public static List<Tunnel> CreateTunnelsAndConnectCaves(ref CellType[,,] cells, List<HollowGroup> hollows, CellSettings settings)
		{
			List<HollowGroup> alreadyConnectedCaves = new List<HollowGroup>();
			List<Tunnel> tunnels = new List<Tunnel>();

			foreach (HollowGroup firstHollow in hollows)
			{
				(HollowGroup secondHollow, Vector3Int firstPoint, Vector3Int secondPoint) = ClosestNotConnectedHollow(firstHollow, hollows, alreadyConnectedCaves);

				if (EmergencyShutdown.Instance.CheckForShutdownTimer())
				{
					return tunnels;
				}

				if (secondHollow == null)
					continue;

				List<Vector3Int> tunnelCells = GetTunnelCellsAndConnectCaves(ref cells, settings, firstPoint, secondPoint);

				Tunnel tunnel = new Tunnel(tunnelCells, firstHollow, secondHollow, firstPoint, secondPoint);
				tunnels.Add(tunnel);

				alreadyConnectedCaves.Add(firstHollow);
			}

			return tunnels;
		}

		private static List<Vector3Int> GetTunnelCellsAndConnectCaves(ref CellType[,,] cells, CellSettings settings, Vector3Int firstPoint, Vector3Int secondPoint)
		{
			int firstX = Mathf.Min(firstPoint.x, secondPoint.x);
			int secondX = Mathf.Max(firstPoint.x, secondPoint.x);
			int firstY = Mathf.Min(firstPoint.y, secondPoint.y);
			int secondY = Mathf.Max(firstPoint.y, secondPoint.y);
			int firstZ = Mathf.Min(firstPoint.z, secondPoint.z);
			int secondZ = Mathf.Max(firstPoint.z, secondPoint.z);

			firstX = Mathf.Max(firstX - settings.TunnelRadius, 0);
			secondX = Mathf.Min(secondX + settings.TunnelRadius, settings.ChunkCubicSize.x - 1);
			firstY = Mathf.Max(firstY - settings.TunnelRadius, 0);
			secondY = Mathf.Min(secondY + settings.TunnelRadius, settings.ChunkCubicSize.y - 1);
			firstZ = Mathf.Max(firstZ - settings.TunnelRadius, 0);
			secondZ = Mathf.Min(secondZ + settings.TunnelRadius, settings.ChunkCubicSize.z - 1);

			List<Vector3Int> tunnelCells = new List<Vector3Int>();

			for (int x = firstX; x <= secondX; x++)
			{
				for (int y = firstY; y <= secondY; y++)
				{
					for (int z = firstZ; z < secondZ; z++)
					{
						float distance = DistanceFromPointToLine(new Vector3Int(x, y, z), firstPoint, secondPoint);

						if (distance > settings.TunnelRadius)
							continue;

						cells[x, y, z] = CellType.Hollow;

						tunnelCells.Add(new Vector3Int(x, y, z));
					}
				}
			}

			return tunnelCells;
		}

		private static float DistanceFromPointToLine(Vector3Int point, Vector3Int lineFirstPoint, Vector3Int lineSecondPoint)
		{
			Vector3Int lineDirection = lineSecondPoint - lineFirstPoint;
			Vector3Int pointDirection = lineFirstPoint - point;

			float distance = Vector3.Cross(pointDirection, lineDirection).magnitude / lineDirection.magnitude;
			return distance;
		}

		private static (HollowGroup hollow, Vector3Int firstPoint, Vector3Int secondPoint) ClosestNotConnectedHollow(HollowGroup hollow, List<HollowGroup> hollows, List<HollowGroup> alreadyConnectedCaves)
		{
			Vector3Int firstPoint = default;
			Vector3Int secondPoint = default;
			float minSqrDistance = 0;
			HollowGroup closestHollow = null;

			foreach (HollowGroup nextHollow in hollows)
			{
				if (nextHollow == hollow || alreadyConnectedCaves.Contains(nextHollow))
					continue;

				Random rand = new Random();

				int firstPointIndex = rand.Next(0, hollow.CellChunkCoordinates.Count);
				int secondPointIndex = rand.Next(0, nextHollow.CellChunkCoordinates.Count);

				return (nextHollow, hollow.CellChunkCoordinates[firstPointIndex], nextHollow.CellChunkCoordinates[secondPointIndex]);

				foreach (Vector3Int firstCoordinate in hollow.CellChunkCoordinates)
				{
					foreach (Vector3Int secondCoordinate in nextHollow.CellChunkCoordinates)
					{
						float sqrDistance = 0;
						//float sqrDistance = (firstCoordinate - secondCoordinate).sqrMagnitude;

						if (EmergencyShutdown.Instance.CheckForShutdownTimer())
							return (null, Vector3Int.zero, Vector3Int.zero);

						if (sqrDistance < minSqrDistance || closestHollow == null)
						{
							firstPoint = firstCoordinate;
							secondPoint = secondCoordinate;
							minSqrDistance = sqrDistance;
							closestHollow = nextHollow;
						}
					}
				}
			}

			return (closestHollow, firstPoint, secondPoint);
		}
	}
}