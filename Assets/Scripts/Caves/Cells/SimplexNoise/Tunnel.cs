using System.Collections.Generic;
using UnityEngine;

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

		public static List<Tunnel> CreateTunnelsAndConnectCaves(ref CellType[,,] cells, List<HollowGroup> hollows, CellularAutomata.CellSettings settings)
		{
			List<HollowGroup> alreadyConnectedCaves = new List<HollowGroup>();
			List<Tunnel> tunnels = new List<Tunnel>();

			foreach (HollowGroup firstHollow in hollows)
			{
				(HollowGroup secondHollow, Vector3Int firstPoint, Vector3Int secondPoint) = ClosestNotConnectedHollow(firstHollow, hollows, alreadyConnectedCaves);

				List<Vector3Int> tunnelCells = GetTunnelCellsAndConnectCaves(ref cells, settings, firstPoint, secondPoint);

				Tunnel tunnel = new Tunnel(tunnelCells, firstHollow, secondHollow, firstPoint, secondPoint);
				tunnels.Add(tunnel);

				alreadyConnectedCaves.Add(firstHollow);
			}

			return tunnels;
		}

		private static List<Vector3Int> GetTunnelCellsAndConnectCaves(ref CellType[,,] cells, CellularAutomata.CellSettings settings, Vector3Int firstPoint, Vector3Int secondPoint)
		{
			int firstX = Mathf.Min(firstPoint.x, secondPoint.x);
			int secondX = Mathf.Max(firstPoint.x, secondPoint.x);
			int firstY = Mathf.Min(firstPoint.y, secondPoint.y); //TODO test, may not work
			int secondY = Mathf.Max(firstPoint.y, secondPoint.y);

			firstX = Mathf.Max(firstX - settings.TunnelWidth, 0);
			secondX = Mathf.Min(secondX + settings.TunnelWidth, settings.TerrainCubicSize.x - 1);
			firstY = Mathf.Max(firstY - settings.TunnelWidth, 0);
			secondY = Mathf.Min(secondY + settings.TunnelWidth, settings.TerrainCubicSize.y - 1);

			List<Vector3Int> tunnelCells = new List<Vector3Int>();

			for (int x = firstX; x <= secondX; x++)
			{
				for (int y = firstY; y <= secondY; y++)
				{
					float distance = DistanceFromPointToLine(firstPoint, secondPoint, x, y);

					if (distance > settings.TunnelWidth)
						continue;

					int maxHeight = Mathf.Min(settings.TunnelHeight, settings.TerrainCubicSize.z);

					for (int z = 0; z < maxHeight; z++)
					{
						cells[x, y, z] = CellType.Hollow;

						tunnelCells.Add(new Vector3Int(x, y, z));
					}
				}
			}

			return tunnelCells;
		}

		private static float DistanceFromPointToLine(Vector3Int lineFirstPoint, Vector3Int lineSecondPoint, int x, int y)
		{
			if (lineFirstPoint.x == lineSecondPoint.x)
				return Mathf.Abs(lineFirstPoint.x - x);

			if (lineFirstPoint.y == lineSecondPoint.y)
				return Mathf.Abs(lineFirstPoint.y - y);

			return Mathf.Abs((lineSecondPoint.y - lineFirstPoint.y) * x - (lineSecondPoint.x - lineFirstPoint.x) * y + lineSecondPoint.x * lineFirstPoint.y - lineSecondPoint.y * lineFirstPoint.x) /
				   Mathf.Sqrt(Mathf.Pow(lineSecondPoint.x - lineFirstPoint.x, 2) + Mathf.Pow(lineSecondPoint.y - lineFirstPoint.y, 2));
		}

		private static (HollowGroup hollow, Vector3Int firstPoint, Vector3Int secondPoint) ClosestNotConnectedHollow(HollowGroup hollow, List<HollowGroup> hollows, List<HollowGroup> alreadyConnectedCaves)
		{
			Vector3Int firstPoint = default;
			Vector3Int secondPoint = default;
			float minDistance = 0;
			HollowGroup closestHollow = null;

			foreach (HollowGroup nextHollow in hollows)
			{
				if (nextHollow == hollow || alreadyConnectedCaves.Contains(nextHollow))
					continue;

				foreach (Vector3Int firstCoordinate in hollow.CellChunkCoordinates)
				{
					if (firstCoordinate.z != 0)
						continue;

					foreach (Vector3Int secondCoordinate in nextHollow.CellChunkCoordinates)
					{
						if (secondCoordinate.z != 0)
							continue;

						float distance = Vector3Int.Distance(firstCoordinate, secondCoordinate);

						if (distance < minDistance || closestHollow == null)
						{
							firstPoint = firstCoordinate;
							secondPoint = secondCoordinate;
							minDistance = distance;
							closestHollow = nextHollow;
						}
					}
				}
			}

			return (closestHollow, firstPoint, secondPoint);
		}
	}
}