using UnityEngine;
using System.Collections;
using System.Threading;
/*
    Class: Grassland.
    Inherits: Biome.
    Date: 11-03-2016.
    Author: Louis Fleron.
    Description:
    Grassland is a biome for lush, fresh green land. 
 */
public class Grassland : Biome {

    private float ideal_Temperature_Start = 16f;
    private float ideal_Temperature_End = 25f;
    private float ideal_Moisture = 5;
    private int ideal_HeightStart = 64;
    private int ideal_HeightEnd = 80;


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
       
        int groundDensity = Mathf.RoundToInt(groundHeight);
        /*
        if (groundDensity <= 64)
        {
            groundDensity += Mathf.CeilToInt(CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain2Offset, 0.02f, 2.5f, 1f));
        }
        */
        Block blockToBuild = Blocks.Get(0);
        if (y <= groundDensity)
        {
            // float caveSize = DetermineCaveSize(y);
            // float caveFrequency = DetermineCaveFrequency(y);

            //float caveChance = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, caveSize, 100, 1);
            blockToBuild = Blocks.Get(2);
            /*
            if (caveFrequency < caveChance)
            {
                float smallDecal = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.05f, 7f, 1f);
                float randomCluster = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.5f, 100f, 1f);
                if (topSoil && y >= 64) 
                {
                    //Above layer
                    blockToBuild = GenerateTopLayer(y, lowerGroundHeight, groundDensity, smallDecal, randomCluster);
                }
                else if (y < lowerGroundHeight - smallDecal)
                {
                    //Rock layer
                    blockToBuild = GenerateRockLayer(y, lowerGroundHeight, groundDensity, smallDecal, randomCluster);
                }
                else if(y < 32 - (smallDecal+randomCluster))
                {
                    //Core layer
                    blockToBuild = GenerateCoreLayer(y, lowerGroundHeight, groundDensity, smallDecal, randomCluster);
                }
                else
                {
                    blockToBuild = GenerateDirtLayer(y, lowerGroundHeight, groundDensity, smallDecal, randomCluster);
                }
            }
            */
        }
        return blockToBuild;
    }

    private Block GenerateTopLayer(short y, short lowerGroundHeight, int groundDensity, float smallDecal, float randomCluster)
    {
        return Blocks.Get(2);
    }

    private Block GenerateDirtLayer(short y, short lowerGroundHeight, int groundDensity, float smallDecal, float randomCluster)
    {
        if (randomCluster > 50f)
        {
            return Blocks.Get(5);
        }
        else
        {
            return Blocks.Get(4);
        }
    }


    private Block GenerateRockLayer(short y, short lowerGroundHeight, int groundDensity, float smallDecal, float randomCluster)
    {
        if (randomCluster > 50f)
        {
            return Blocks.Get(12);
        }
        else
        {
            return Blocks.Get(13);
        }
    }


    private Block GenerateCoreLayer(short y, short lowerGroundHeight, int groundDensity, float smallDecal, float randomCluster)
    {
        return Blocks.Get(11);
    }

    private float DetermineCaveSize(short y)
    {
        return 0.025f; //Higher value = smaller caves. 
    }

    private float DetermineCaveFrequency(short y)
    {
        return 4f;
    }
    


}
