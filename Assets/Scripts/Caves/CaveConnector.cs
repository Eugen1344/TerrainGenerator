using System;
using System.Collections.Generic;
using UnityEngine;

public class CaveConnector
{
	public static List<CaveTunnel> ConnectCaves(ref CellType[,,] cells, List<CaveHollowGroup> hollows)
	{
		LinkedList<CaveHollowGroup> connectedCaves = new LinkedList<CaveHollowGroup>();

		List<CaveTunnel> tunnels = new List<CaveTunnel>();

		foreach (CaveHollowGroup hollow in hollows)
		{
			CaveTunnel closestHollow = CreateClosestTunnel(hollow, hollows);
		}
	}

	private static CaveTunnel CreateClosestTunnel(CaveHollowGroup hollow, List<CaveHollowGroup> hollows)
	{
		foreach (CaveHollowGroup nextHollow in hollows)
		{
			CaveHollowGroup closestCave = ClosestHollow(hollow, hollows);
		}
	}

	private static CaveHollowGroup ClosestHollow(CaveHollowGroup hollow, List<CaveHollowGroup> hollows)
	{
		Vector3Int firstPoint = default;
		Vector3Int secondPoint = default;
		float minDistance = -1;
		CaveHollowGroup closestHollow = null;

		foreach (CaveHollowGroup nextHollow in hollows)
		{
			foreach (Vector3Int firstCoordinate in first.CellChunkCoordinates)
			{
				foreach (Vector3Int secondCoordinate in second.CellChunkCoordinates)
				{
					float distance = Vector3Int.Distance(firstCoordinate, secondCoordinate);

					if (distance < minDistance || minDistance < 0)
					{
						firstPoint = firstCoordinate;
						secondPoint = secondCoordinate;
						minDistance = distance;
					}
				}
			}
		}

		return closestHollow;
	}
}