using UnityEngine;
using System.Collections;

public class Perlin : MonoBehaviour {

    public float amplitude = 1f;
    public bool applyAmplitudeDamping = false;
    public float frequency = 0.5f;
    public float scale=0.05f;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int octaves = 8;
    public Vector2 offset;
    float[,] array;

    public float[,] Generate(int size, int seed)
    {
        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(0, 10000) + offset.x;
            float offsetY = rng.Next(0, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        array = new float[size, size];

        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;


        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                //Note that amplitude has minor effect due to Normalization of Array. PerlinNoise * amplitude = bigger range if amplitude is > 1. However the range is reduced if amplitude is < 1. 
                //However since array is Normalized, the NoiseHeight will become the same. The algorithm calculates MaxNoiseHeight and MinNoiseHeight. It InverseLerps between those two values. 
                //Also note that SimplexNoise goes from -1 to 1. If the algorithm does not normalize, the Terrain will consider all values < 0 as 0, and all values > 1 as 1. 
                //This leads to severe cutoffs in terrain. 
                //Note: This has been fixed by implementing an Amplitude Damping. 
                float amp = amplitude;
                float freq = frequency;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * freq + octaveOffsets[i].x/scale;
                    float sampleY = y / scale * freq + octaveOffsets[i].y/scale;
                    float perlinNoise = SimplexNoise.SimplexNoise.Generate(sampleX, sampleY);
                   
                    noiseHeight += perlinNoise* amp;

                    amp *= persistence;
                    freq *= lacunarity;  
                }
             
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                array[x, y] = noiseHeight;
            }
        }

        NormalizeArray(size, minNoiseHeight, maxNoiseHeight);
        return array;
    }

    private void NormalizeArray(int size, float minNoiseHeight, float maxNoiseHeight)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                array[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, array[x, y]);
                if (applyAmplitudeDamping) array[x, y] *= amplitude;
            }
        }
    }



}
