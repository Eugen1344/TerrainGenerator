public class PlayerPlacer : Placer
{
	public bool SpawnInRandomCave;
	public int CaveIndexToSpawn;

	private void Start()
	{
		if (SpawnInRandomCave)
			PlaceInRandomCave();
		else
			PlaceInCave(CaveIndexToSpawn);
	}
}