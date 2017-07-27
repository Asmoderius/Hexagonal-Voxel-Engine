using UnityEngine;
using System.Collections;
using System;
/*
Class: Arctic.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Arctic inherits from Climate class. Arctic is used to store and retrieve biomes within an arctic climate. 
*/
public class Arctic : Climate
{
    private float ideal_Temperature_Start = 0;
    private float ideal_Temperature_End = 9;


    public Arctic()
    {
        biomes = new Biome[3];
        biomes[0] = new SnowPlain();
        biomes[1] = new SnowHill();
        biomes[2] = new SnowMountain();
    }
    public override float Bid(float temperatureValue)
    {
        float bid = 0f;
        if (temperatureValue >= ideal_Temperature_Start && temperatureValue < ideal_Temperature_End)
        {
            bid += 1;
        }
        return bid;
    }

    public override Biome GetIdealBiome(float temperatureValue, float moistureValue, int groundHeight)
    {
        Biome idealBiome = biomes[0];
        float bestBid = 0f;
        foreach (Biome biome in biomes)
        {
            float bid = biome.Bid(temperatureValue, moistureValue, groundHeight);
            if (bid > bestBid)
            {
                bestBid = bid;
                idealBiome = biome;
            }
        }
        return idealBiome;
    }


}
