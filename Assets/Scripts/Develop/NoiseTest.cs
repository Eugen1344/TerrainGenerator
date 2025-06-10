using System.IO;
using Caves.Cells.SimplexNoise;
using Caves.Chunks;
using SimplexNoise;
using UnityEngine;

namespace Develop
{
    public class NoiseTest : MonoBehaviour
    {
        [SerializeField] private Color _hollowColor;
        [SerializeField] private Color _caveColor;
        [SerializeField] private CaveChunkManager _chunkManager;
        [SerializeField] private int _seed;

        [ContextMenu("Generate noise png")]
        public void GenerateNoisePng()
        {
            Vector3Int gridSize = _chunkManager.BaseGeneratorSettings.GridSize;
            
            Texture2D noiseTexture = new Texture2D(gridSize.x, gridSize.y);

            Noise noiseGenerator = new Noise(_seed);

            float[,] noise = noiseGenerator.Calc2D(gridSize.x, gridSize.y, _chunkManager.BaseGeneratorSettings.NoiseScale);

            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    float normalizedNoise = (noise[i, j] + 1f) / 2f;
                    Color color = new Color(normalizedNoise, normalizedNoise, normalizedNoise, 1f);

                    noiseTexture.SetPixel(i, j, color);
                }
            }

            byte[] bytes = noiseTexture.EncodeToPNG();
            string filename = "Assets/noise.png";
            File.WriteAllBytes(filename, bytes);

            Texture2D resultTexture = new Texture2D(gridSize.x, gridSize.y);

            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    float normalizedNoise = (noise[i, j] + 1f) / 2f;

                    Color color = normalizedNoise <= _chunkManager.CavesSettings.HollowCellThreshold ? _hollowColor : _caveColor;

                    resultTexture.SetPixel(i, j, color);
                }
            }

            bytes = resultTexture.EncodeToPNG();
            filename = "Assets/noise_result.png";
            File.WriteAllBytes(filename, bytes);
        }
    }
}