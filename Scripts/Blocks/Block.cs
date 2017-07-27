using UnityEngine;
using System.Collections;

/*
Struct: Block.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Central piece in the game. 
*/
public struct Block
{
    public short ID;
    public bool Surface;
    public bool Solid;
    public bool Transparent;
    public bool Indestructible;
    public byte blockOriginalHealth;
    public byte blockCurrentHealth;
    public bool Connected;
    //Block coloring 
    public byte Color_R;
    public byte Color_G;
    public byte Color_B;
    public byte Color_A;
    public bool LoadedFromDisk;


    public Block(short ID, bool isSurface, bool isSolid, bool isTransparent, bool isIndestructible, byte blockHealth, byte color_R, byte color_G, byte color_B, byte color_A)
    {
        this.ID = ID;
        this.Surface = isSurface;
        this.Solid = isSolid;
        this.Transparent = isTransparent;
        this.Indestructible = isIndestructible;
        this.blockOriginalHealth = blockHealth;
        this.blockCurrentHealth = blockHealth;
        this.Connected = false;
        this.Color_A = color_A;
        this.Color_R = color_R;
        this.Color_G = color_G;
        this.Color_B = color_B;
        LoadedFromDisk = false;
    }



    public Color32 Colorize(bool fade = false, byte fadeScale = 10)
    {
        byte r = this.Color_R;
        byte g = this.Color_G;
        byte b = this.Color_B;
        if (fade)
        {
            if(this.Color_R >= fadeScale)
            {
                r = (byte)(this.Color_R - fadeScale);
            }
            if (this.Color_G >= fadeScale)
            {
                g = (byte)(this.Color_G - fadeScale);
            }
            if (this.Color_B >= fadeScale)
            {
                b = (byte)(this.Color_B - fadeScale);
            }
        }
        return new Color32(r, g, b, this.Color_A);
    }

 


}


