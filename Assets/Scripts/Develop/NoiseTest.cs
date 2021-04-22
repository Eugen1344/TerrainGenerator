using Caves.Cells.SimplexNoise;
using SimplexNoise;
using UnityEngine;

namespace Develop
{
	public class NoiseTest : MonoBehaviour
	{
		public Color HollowColor;
		public Color CaveColor;
		public CellSettings Settings;

		[ContextMenu("Generate noise png")]
		public void GenerateNoisePng()
		{
			Texture2D noiseTexture = new Texture2D(Settings.ChunkCubicSize.x, Settings.ChunkCubicSize.y);

			Noise noiseGenerator = new Noise(Settings.Seed);

			float[,] noise = noiseGenerator.Calc2D(Settings.ChunkCubicSize.x, Settings.ChunkCubicSize.y, Settings.NoiseScale);

			//float[,] noise = new float[Settings.ChunkCubicSize.x, Settings.ChunkCubicSize.y];

			/*for (int i = 0; i < Settings.ChunkCubicSize.x; i++)
			{
				for (int j = 0; j < Settings.ChunkCubicSize.y; j++)
				{
					noise[i, j] = TestNoise.Noise.Simplex2D(new Vector3(i, j, 0), Settings.NoiseScale).value;
				}
			}*/

			for (int i = 0; i < Settings.ChunkCubicSize.x; i++)
			{
				for (int j = 0; j < Settings.ChunkCubicSize.y; j++)
				{
					float normalizedNoise = (noise[i, j] + 1f) / 2f;
					Color color = new Color(normalizedNoise, normalizedNoise, normalizedNoise, 1f);

					noiseTexture.SetPixel(i, j, color);
				}
			}

			byte[] bytes = noiseTexture.EncodeToPNG();
			string filename = "Assets/noise.png";
			System.IO.File.WriteAllBytes(filename, bytes);

			Texture2D resultTexture = new Texture2D(Settings.ChunkCubicSize.x, Settings.ChunkCubicSize.y);

			for (int i = 0; i < Settings.ChunkCubicSize.x; i++)
			{
				for (int j = 0; j < Settings.ChunkCubicSize.y; j++)
				{
					float normalizedNoise = (noise[i, j] + 1f) / 2f;

					Color color = normalizedNoise <= Settings.RandomHollowCellsPercent ? HollowColor : CaveColor;

					resultTexture.SetPixel(i, j, color);
				}
			}

			bytes = resultTexture.EncodeToPNG();
			filename = "Assets/noise_result.png";
			System.IO.File.WriteAllBytes(filename, bytes);
		}
	}
}