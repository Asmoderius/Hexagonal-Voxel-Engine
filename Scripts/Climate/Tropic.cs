using UnityEngine;
using System.Collections;
using System;
/*
Class: Tropic.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Tropic inherits from Climate class. Tropic is used to store and retrieve biomes within an Tropic climate. 
*/
public class Tropic : Climate {

    private float ideal_Temperature_Start = 32;
    private float ideal_Temperature_End = 41;

    public Tropic()
    {
        biomes = new Biome[3];
        biomes[0] = new Desert();
        biomes[1] = new Dune();
        biomes[2] = new Mountain();
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
