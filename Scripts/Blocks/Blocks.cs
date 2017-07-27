using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
/*
Class: Blocks.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
This class loads the creates all the blocks based upon a text file. The blocks are stored in a dictionary for fast retrieval.  
*/
public class Blocks
{

    public static Dictionary<short, Block> blocks;

    public static void Initialize()
    {
        blocks = new Dictionary<short, Block>();
        string[] blockLines = File.ReadAllLines("blocks.txt");
        foreach (string s in blockLines)
        {
            if(!s.StartsWith("#") && !string.IsNullOrEmpty(s)) CreateBlock(s);
        }
    }

    public static Block Get(short id)
    {
        Block b;
        blocks.TryGetValue(id, out b);
        return b;
    }

    private static void CreateBlock(string blockInfo)
    {

        //Format: Block Name, short ID, bool isSurface, bool isSolid, bool isTransparent, bool isIndestructible, byte blockHealth, byte color_R, byte color_G, byte color_B, byte color_A
        string[] blockParts = blockInfo.Split(',');
        short id;
        bool isSurface;
        bool isSolid;
        bool isTransparent;
        bool isIndestructible;
        byte blockHealth;
        byte color_R;
        byte color_G;
        byte color_B;
        byte color_A;

        short.TryParse(blockParts[1], out id);
        bool.TryParse(blockParts[2], out isSurface);
        bool.TryParse(blockParts[3], out isSolid);
        bool.TryParse(blockParts[4], out isTransparent);
        bool.TryParse(blockParts[5], out isIndestructible);
        byte.TryParse(blockParts[6], out blockHealth);
        byte.TryParse(blockParts[7], out color_R);
        byte.TryParse(blockParts[8], out color_G);
        byte.TryParse(blockParts[9], out color_B);
        byte.TryParse(blockParts[10], out color_A);

        if(!blocks.ContainsKey(id))
        {
            blocks.Add(id, new Block(id, isSurface, isSolid, isTransparent, isIndestructible, blockHealth, color_R, color_G, color_B, color_A));
        }

        
    }




}
