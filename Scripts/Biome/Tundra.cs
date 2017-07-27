using UnityEngine;

/*
    Class: Tundra.
    Inherits: Biome.
    Date: 11-03-2016.
    Author: Louis Fleron.
    Description:
    Flat terrain found in Cold climate (Tundra). 
    
 */
public class Tundra : Biome {

    public float caveSize = 0.025f; //Higher value = smaller caves. 
    public float caveFrequency = 4f;
    private float ideal_Temperature_Start = 9f;
    private float ideal_Temperature_End = 16f;
    private float ideal_Moisture = 5;
    private int ideal_HeightStart = 31;
    private int ideal_HeightEnd = 40;

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
        //Step 1 : Ground to int density - Makes it easier
        int groundDensity = Mathf.RoundToInt(groundHeight);
        //Step 2: Adjust density. This is Biome specific, replace code with biome specific details.
        if (groundDensity <= 32)
        {
            groundDensity += Mathf.CeilToInt(CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain2Offset, 0.008f, 1f, 1f));
        }
        //Step 3: Block to return
        Block blockToBuild = Blocks.Get(0);
        //Step 4: If density is greater or equal to y
        if (y <= groundDensity)
        {

            float caveChance = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, caveSize, 100, 1);
            //Should it be a cave or terrain?
            if (caveFrequency < caveChance)
            {
                //Calculating extra noise values.
                float smallDecal = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.04f, 3f, 1f);
                float randomCluster = CalculateNoiseValue(noise_X, noise_Y, noise_Z, Settings.currentSettings.grain0Offset, 0.5f, 100f, 1f);
                blockToBuild = Blocks.Get(8);
                /*
                if (topSoil)
                {
                    blockToBuild = new Block(BlockType.HighlandGrass, false, true, false, false, 2, 0, 70, 10, 255);
                }
                else
                {
                    blockToBuild = new Block(BlockType.Dirt_Dark, false, true, false, false, 2, 130, 80, 0, 255);
                }
                */
            }
        }
        return blockToBuild;
    }
}
