using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct GridTile
{
    public int X, Y;
    public TileType tileType;    

    public GridTile(int x, int y, TileType type)
    {
        X = x;
        Y = y;
        tileType = type;
    }
}

public enum TileType
{
    Normal,
    Impassable,
    Slow,
    Web,
    Trap,
    Dead,
    Victory
}
