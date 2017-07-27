using System;

[Serializable]
public class BlockGridPosition
{
    public int x;
    public int y;
    public int z;

    public BlockGridPosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

}
