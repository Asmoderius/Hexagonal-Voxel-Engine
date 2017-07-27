using UnityEngine;
using System.Collections;

public abstract class Biome
{
    /*
        Class: CubeBlockOperations.
        Date: 11-03-2016.
        Author: Louis Fleron.
        Description:
        Abstract class used to describe functionality that all biomes contains.
        
        Rough idea:
      
        Biome bidding system.
        Each biome bids with the following parameters: GroundHeight(Rockiness), LowerGroundHeight, Temperature and Moisture.
        Return the biome with highest score. 
        Use that biome to generate Block. 
     
      
     */
    private float ideal_Temperature;
    private float ideal_Moisture;
    private int ideal_HeightStart;
    private int ideal_HeightEnd;


    /**
     * Very basic bidding system
     * */

    public abstract Block GenerateBlock(float noise_X, float noise_Y, float noise_Z, short y, short lowerGroundHeight, short groundHeight, bool topSoil, float temperature, float moisture);

    public abstract float Bid(float temperatureValue, float moistureValue, int groundHeight);

    public float CalculateNoiseValue(float x, float y, float z, Vector3 offset, float scale, float max, float power)
    {
        float noise_X = (x + offset.x) * scale;
        float noise_Y = (y + offset.y) * scale;
        float noise_Z = (z + offset.z) * scale;
        if (noise_X < 0) noise_X *= -1;
        if (noise_Z < 0) noise_Z *= -1;

        float noise = (SimplexNoise.SimplexNoise.Generate(noise_X, noise_Y, noise_Z) + 1f) * (max / 2f);
        noise = Mathf.Pow(noise, power);
        return noise;
    }

    public float CalculateNoiseValueWithNegative(float x, float y, float z, Vector3 offset, float scale, float max, float power)
    {
        float noise_X = (x + offset.x) * scale;
        float noise_Y = (y + offset.y) * scale;
        float noise_Z = (z + offset.z) * scale;
        if (noise_X < 0) noise_X *= -1;
        if (noise_Z < 0) noise_Z *= -1;

        float noise = SimplexNoise.SimplexNoise.Generate(noise_X, noise_Y, noise_Z) * (max);
        noise = Mathf.Pow(noise, power);
        return noise;
    }

    public float CalculateNoiseValue(float x, float z, Vector3 offset, float scale, float max, float power)
    {
        float noise_X = (x + offset.x) * scale;
        float noise_Z = (z + offset.z) * scale;
        if (noise_X < 0) noise_X *= -1;
        if (noise_Z < 0) noise_Z *= -1;

        float noise = (SimplexNoise.SimplexNoise.Generate(noise_X, noise_Z) + 1f) * (max / 2f);
        noise = Mathf.Pow(noise, power);
        return noise;
    }

 



}
