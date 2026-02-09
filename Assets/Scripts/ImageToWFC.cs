using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Analyzes an input image to extract tiles and adjacency rules for WFC algorithm
/// </summary>
public class ImageToWFC
{
    private Texture2D sourceImage;
    private int tileSize;
    private Dictionary<Color, string> colorToTileName;
    private Dictionary<string, Color> tileNameToColor;
    private Dictionary<string, HashSet<string>> adjacencyRules;
    private int nextTileId = 0;

    /// <summary>
    /// Create an ImageToWFC analyzer
    /// </summary>
    /// <param name="image">Source image to analyze</param>
    /// <param name="tileSize">Size of each tile in pixels (default: 1 for per-pixel analysis)</param>
    public ImageToWFC(Texture2D image, int tileSize = 1)
    {
        this.sourceImage = image;
        this.tileSize = tileSize;
        this.colorToTileName = new Dictionary<Color, string>();
        this.tileNameToColor = new Dictionary<string, Color>();
        this.adjacencyRules = new Dictionary<string, HashSet<string>>();
    }

    /// <summary>
    /// Analyze the image and extract tiles and adjacency rules
    /// </summary>
    public void Analyze()
    {
        // First pass: identify unique tiles (colors)
        for (int y = 0; y < sourceImage.height; y += tileSize)
        {
            for (int x = 0; x < sourceImage.width; x += tileSize)
            {
                Color tileColor = GetTileColor(x, y);
                if (!colorToTileName.ContainsKey(tileColor))
                {
                    string tileName = $"tile_{nextTileId}";
                    colorToTileName[tileColor] = tileName;
                    tileNameToColor[tileName] = tileColor;
                    adjacencyRules[tileName] = new HashSet<string>();
                    // Allow tiles to be adjacent to themselves
                    adjacencyRules[tileName].Add(tileName);
                    nextTileId++;
                }
            }
        }

        // Second pass: build adjacency rules by analyzing neighboring tiles
        for (int y = 0; y < sourceImage.height; y += tileSize)
        {
            for (int x = 0; x < sourceImage.width; x += tileSize)
            {
                Color currentColor = GetTileColor(x, y);
                string currentTile = colorToTileName[currentColor];

                // Check right neighbor
                if (x + tileSize < sourceImage.width)
                {
                    Color rightColor = GetTileColor(x + tileSize, y);
                    string rightTile = colorToTileName[rightColor];
                    adjacencyRules[currentTile].Add(rightTile);
                    adjacencyRules[rightTile].Add(currentTile);
                }

                // Check bottom neighbor
                if (y + tileSize < sourceImage.height)
                {
                    Color bottomColor = GetTileColor(x, y + tileSize);
                    string bottomTile = colorToTileName[bottomColor];
                    adjacencyRules[currentTile].Add(bottomTile);
                    adjacencyRules[bottomTile].Add(currentTile);
                }
            }
        }
    }

    /// <summary>
    /// Get the representative color for a tile at the given position
    /// </summary>
    private Color GetTileColor(int x, int y)
    {
        if (tileSize == 1)
        {
            return sourceImage.GetPixel(x, y);
        }
        
        // For larger tiles, sample the center pixel or average
        int centerX = x + tileSize / 2;
        int centerY = y + tileSize / 2;
        
        // Clamp to image bounds
        centerX = Mathf.Clamp(centerX, 0, sourceImage.width - 1);
        centerY = Mathf.Clamp(centerY, 0, sourceImage.height - 1);
        
        return sourceImage.GetPixel(centerX, centerY);
    }

    /// <summary>
    /// Get the array of tile names extracted from the image
    /// </summary>
    public string[] GetTiles()
    {
        string[] tiles = new string[tileNameToColor.Count];
        int i = 0;
        foreach (string tileName in tileNameToColor.Keys)
        {
            tiles[i++] = tileName;
        }
        return tiles;
    }

    /// <summary>
    /// Get the adjacency rules as a dictionary
    /// </summary>
    public Dictionary<string, List<string>> GetRules()
    {
        Dictionary<string, List<string>> rules = new Dictionary<string, List<string>>();
        foreach (var kvp in adjacencyRules)
        {
            rules[kvp.Key] = new List<string>(kvp.Value);
        }
        return rules;
    }

    /// <summary>
    /// Get the color mapping for rendering tiles
    /// </summary>
    public Dictionary<string, Color> GetColorMap()
    {
        return new Dictionary<string, Color>(tileNameToColor);
    }

    /// <summary>
    /// Get the number of unique tiles found
    /// </summary>
    public int GetTileCount()
    {
        return tileNameToColor.Count;
    }
}
