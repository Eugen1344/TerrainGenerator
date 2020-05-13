using System;
using System.Collections.Generic;
using UnityEngine;

namespace Caves.Cells
{
	public class CaveTunnel
	{
		public List<Vector3Int> CellChunkCoordinates;
		public CaveHollowGroup FirstCave;
		public CaveHollowGroup SecondCave;
		public Vector3Int FirstCaveConnectionPoint;
		public Vector3Int SecondCaveConnectionPoint;

		public CaveTunnel(List<Vector3Int> cellChunkCoordinates, CaveHollowGroup firstCave, CaveHollowGroup secondCave, Vector3Int firstCaveConnectionPoint, Vector3Int secondCaveConnectionPoint)
		{
			CellChunkCoordinates = cellChunkCoordinates;
			FirstCave = firstCave;
			SecondCave = secondCave;
			FirstCaveConnectionPoint = firstCaveConnectionPoint;
			SecondCaveConnectionPoint = secondCaveConnectionPoint;
		}

		public static List<CaveTunnel> CreateTunnelsAndConnectCaves(ref CellType[,,] cells, List<CaveHollowGroup> hollows, CaveCellSettings settings)
		{
			List<CaveHollowGroup> alreadyConnectedCaves = new List<CaveHollowGroup>();
			List<CaveTunnel> tunnels = new List<CaveTunnel>();

			foreach (CaveHollowGroup firstHollow in hollows)
			{
				(CaveHollowGroup secondHollow, Vector3Int firstPoint, Vector3Int secondPoint) = ClosestHollow(firstHollow, hollows, alreadyConnectedCaves);

				int firstX = firstPoint.x < secondPoint.x ? firstPoint.x : secondPoint.x;
				int secondX = secondPoint.x > firstPoint.x ? secondPoint.x : firstPoint.x;
				int firstY = firstPoint.y < secondPoint.y ? firstPoint.y : secondPoint.y;
				int secondY = secondPoint.y > firstPoint.y ? secondPoint.y : firstPoint.y;

				firstX = firstX - settings.TunnelWidth < 0 ? 0 : firstX - settings.TunnelWidth;
				secondX = secondX + settings.TunnelWidth >= settings.TerrainCubicSize.x ? secondX : secondX + settings.TunnelWidth;
				firstY = firstY - settings.TunnelWidth < 0 ? 0 : firstY - settings.TunnelWidth;
				secondY = secondY + settings.TunnelWidth >= settings.TerrainCubicSize.x ? secondY : secondY + settings.TunnelWidth;

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

				CaveTunnel tunnel = new CaveTunnel(tunnelCells, firstHollow, secondHollow, firstPoint, secondPoint);
				tunnels.Add(tunnel);

				alreadyConnectedCaves.Add(firstHollow);
			}

			return tunnels;
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

		private static (CaveHollowGroup hollow, Vector3Int firstPoint, Vector3Int secondPoint) ClosestHollow(CaveHollowGroup hollow, List<CaveHollowGroup> hollows, List<CaveHollowGroup> alreadyConnectedCaves)
		{
			Vector3Int firstPoint = default;
			Vector3Int secondPoint = default;
			float minDistance = 0;
			CaveHollowGroup closestHollow = null;

			foreach (CaveHollowGroup nextHollow in hollows)
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