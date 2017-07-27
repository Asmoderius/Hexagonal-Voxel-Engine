using UnityEngine;
using System.Collections;
/*
    Class: Mountain.
    Inherits: Biome.
    Date: 11-03-2016.
    Author: Louis Fleron.
    Description:
    Mountain biome for several climates. Standard mountain, nothing fancy. 
 */
public class Mountain : Biome {


    public float caveSize = 0.02f; //Higher value = smaller caves. 
    public float caveFrequency = 12;
    private float ideal_Temperature_Start = 9f;
    private float ideal_Temperature_End = 32f;
    private float ideal_Moisture = 5;
    private int ideal_HeightStart = 104;
    private int ideal_HeightEnd = 256;


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

    public override Block GenerateBlock(float noise_X, float noise_Y, float noise_Z, short y, short lowerGroundHeight, short groundHeight, bool topSoil, float temperature, float moisture)
    {
        float terrainDetail = CalculateNoiseValueWithNegative(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain2Offset, 0.0475f, 4f, 1f);
        int groundDensity = Mathf.CeilToInt(groundHeight + terrainDetail);
        
        Block blockToBuild = Blocks.Get(0);
        if (y <= groundDensity)
        {

            float caveChance = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, caveSize, 100, 1);
           if (caveFrequency < caveChance)
            {
                float smallDecal = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.03f, 5f, 1f);
                float randomCluster = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.5f, 100f, 1f);
                if (topSoil)
                {

                    if (topSoil)
                    {
                        blockToBuild = Blocks.Get(11);
                    }
                    else
                    {
                        blockToBuild = Blocks.Get(12);
                    }

                }
                else
                {
                    blockToBuild = Blocks.Get(11);
                }
               
            }

        }
        return blockToBuild;
    }

   


}
