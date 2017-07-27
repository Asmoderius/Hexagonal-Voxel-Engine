using UnityEngine;
using System.Collections;

/*
Class: Earth.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Earth inherits from World class. Earth contains methods used to generate Earth-like worlds. 
*/

public class Earth : World 
{

    public Earth()
    {
        worldType = Worlds.Earth;
        InitializeClimates();
        
    }

    private void InitializeClimates()
    {
        climates = new Climate[1];
        /*
        climates[0] = new Arctic();
        climates[1] = new Boreal();
        climates[2] = new Temperate();
        climates[3] = new Subtropic();
        climates[4] = new Tropic();
        */
        climates[0] = new Temperate();
    }

    public override Biome GetIdealBiome(float temperature, float moisture, short groundHeight)
    {
        Climate idealClimate = climates[0];
        float bestBid = 0f;
        foreach (Climate climate in climates)
        {
            float bid = climate.Bid(temperature);
            if(bid > bestBid)
            {
                bestBid = bid;
                idealClimate = climate;
            }
        }

        return idealClimate.GetIdealBiome(temperature, moisture, groundHeight);
    }

    //MAY 2017 IF CONTINUE WITH PROJECT: REMOVE THESE METHODS AND IMPLEMENT CORRECT PERLIN NOISE SYSTEM. THIS IS FOR TESTING!!!!!
    /*
    GetUpperGroundHeight calculates how much that should be added to LowerGroundHeight. This affects mountains, hills, and terrain detail. 
    */
    public override float GetUpperGroundHeight(float x, float z, float lowerGroundHeight)
    {
        float noise_X = (x + Settings.currentSettings.grain1Offset.x);
        float noise_Y = ((x+z) + Settings.currentSettings.grain1Offset.y);
        float noise_Z = (z + Settings.currentSettings.grain1Offset.z);
        if (noise_X < 0) noise_X *= -1;
        if (noise_Y < 0) noise_Y *= -1;
        if (noise_Z < 0) noise_Z *= -1;
        float octave1 = SimplexNoise.SimplexNoise.Generate(noise_X * 0.00099f, noise_Y* 0.00098f, noise_Z * 0.00097f) * 0.4f;
        float octave2 = SimplexNoise.SimplexNoise.Generate(noise_X * 0.0019f, noise_Y* 0.0018f, noise_Z * 0.0017f) * 0.155f;
        float octave3 = SimplexNoise.SimplexNoise.Generate(noise_X * 0.0099f, noise_Y* 0.0098f, noise_Z * 0.0097f) * 0.149f;
        float octaveSum = octave1 + octave2 + octave3;
        if (octaveSum < 0) octaveSum = 0;
        return (Mathf.Pow(octaveSum,2.25f) * (Settings.currentSettings.chunk_Size_Y)) + (lowerGroundHeight);
    }

    /*
    GetLowerGroundHeight calculates how much the ground should be raised from 0. 
    */
    public override float GetLowerGroundHeight(float x, float z)
    {
        float noise_X = (x + Settings.currentSettings.grain0Offset.x);
        float noise_Y = ((x + z) + Settings.currentSettings.grain1Offset.y);
        float noise_Z = (z + Settings.currentSettings.grain0Offset.z);
        if (noise_X < 0) noise_X *= -1;
        if (noise_Z < 0) noise_Z *= -1;
        if (noise_Y < 0) noise_Y *= -1;

        float octave1 = SimplexNoise.SimplexNoise.Generate(noise_X * 0.00099f, noise_Y * 0.00098f, noise_Z * 0.00097f) * 0.4f;
        float octave2 = SimplexNoise.SimplexNoise.Generate(noise_X * 0.0019f, noise_Y * 0.0018f, noise_Z * 0.0017f) * 0.155f;
        float octave3 = SimplexNoise.SimplexNoise.Generate(noise_X * 0.0099f, noise_Y * 0.0098f, noise_Z * 0.0097f) * 0.149f;

        float lowerGroundHeight = octave1 + octave2 + octave3;
        int minimumGroundheight = 64;
        int bumpyness = 32;
        lowerGroundHeight = lowerGroundHeight* bumpyness+minimumGroundheight;

        return lowerGroundHeight;
    }

    public override Tuple<short, short> GetHeight(float noise_X, float noise_Z)
    {
            float lowerGroundHeight = GetLowerGroundHeight(noise_X, noise_Z);
        short groundHeight = (short)GetUpperGroundHeight(noise_X, noise_Z, lowerGroundHeight);
            if (groundHeight > Settings.currentSettings.chunk_Size_Y)
            {
                groundHeight = Settings.currentSettings.chunk_Size_Y;
            }
            return new Tuple<short, short>(groundHeight, (short)lowerGroundHeight);
    }
}
