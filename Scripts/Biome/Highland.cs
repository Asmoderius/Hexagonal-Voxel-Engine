using UnityEngine;
using System.Collections;
/*
    Class: Highland.
    Inherits: Biome.
    Date: 11-03-2016.
    Author: Louis Fleron.
    Description:
    Highland is raised terrain that contains many hills. Used together with Grassland
 */
public class Highland : Biome {

    public float caveSize = 0.025f; //Higher value = smaller caves. 
    public float caveFrequency = 8;
    private float ideal_Temperature_Start = 16f;
    private float ideal_Temperature_End = 25f;
    private float ideal_Moisture = 5;
    private int ideal_HeightStart = 80;
    private int ideal_HeightEnd = 104;


    public override float Bid(float temperatureValue, float moistureValue, int groundHeight)
    {
        float bid = 0f;
        if (groundHeight > ideal_HeightStart && groundHeight <= ideal_HeightEnd)
        {
            bid = 100;
        }
        if (temperatureValue >= ideal_Temperature_Start && temperatureValue < ideal_Temperature_End)
        {
            bid += 1;
        }

        return bid;
    }

    public override Block GenerateBlock(float noise_X, float noise_Y, float noise_Z, short y, short lowergroundDensityHeight, short groundDensityHeight, bool topSoil, float temperature, float moisture)
    {
        int terrainDetail = Mathf.CeilToInt(CalculateNoiseValueWithNegative(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain2Offset, 0.015f, 2f, 1f));
       
        int groundDensity = groundDensityHeight + terrainDetail;
        Block blockToBuild = Blocks.Get(0);
        if (y <= groundDensity)
        {

            float caveChance = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, caveSize, 100, 1);
            if (caveFrequency < caveChance)
            {
                float smallDecal = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.09f, 3f, 1f);
                float randomCluster = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.5f, 100f, 1f);
                blockToBuild = Blocks.Get(3);
                /*
                if (topSoil)
                {
                    blockToBuild = new Block(BlockType.Grass_Dark, false, true, false, false, 2, 0, 80, 0, 255);
                }
                else
                {
                    blockToBuild = new Block(BlockType.Rock, false, true, false, false, 2, 110, 110, 110, 255);
                }
                */
            }
        }
            return blockToBuild;
    }

    
}
