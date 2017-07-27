using UnityEngine;
using System.Collections;

public class BlockUtility
{


}

public struct Tuple<T, K>
{
    public T x;
    public K y;

    public Tuple(T x, K y)
    {
        this.x = x;
        this.y = y;
    }

}

public struct Tuple3D<T, K, B>
{
    public T x;
    public K y;
    public B z;

    public Tuple3D(T x, K y, B z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public struct Tuple4D<T, K, B, C>
{
    public T x;
    public K y;
    public B z;
    public C c;

    public Tuple4D(T x, K y, B z, C c)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.c = c;
    }
}

public enum BuildMode
{
    Cube,
    Hexagon

}
