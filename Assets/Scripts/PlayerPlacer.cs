using System.Diagnostics;

public class PlayerPlacer : Placer
{
	public bool SpawnInRandomCave;
	public int CaveIndexToSpawn;

	private async void Start()
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		UnityEngine.Debug.Log($"Started chunk generation of {ChunkGenerationRadius * ChunkGenerationRadius} chunks:");

		if (SpawnInRandomCave)
			await PlaceInRandomCave();
		else
			await PlaceInCave(CaveIndexToSpawn);

		watch.Stop();
		UnityEngine.Debug.Log($"Done generating chunk. Took: {watch.Elapsed.TotalSeconds}s");
	}
}