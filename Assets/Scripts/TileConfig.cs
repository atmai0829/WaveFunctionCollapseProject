using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileConfig
{
    public string tileName;
    public Color tileColor;
    public List<string> allowedNeighbors;
}

[Serializable]
public class CustomTileset
{
    public List<TileConfig> tiles;
}
