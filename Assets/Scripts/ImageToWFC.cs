using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Color equality comparer that handles floating-point precision issues
/// </summary>
public class ColorEqualityComparer : IEqualityComparer<Color>
{
    // Tolerance for color comparison to handle compression artifacts and filtering
    private const float COLOR_TOLERANCE = 0.01f; // 2.55/255 (1% tolerance), handles JPEG/PNG compression and filtering
    
    public bool Equals(Color c1, Color c2)
    {
        // Compare with tolerance to handle floating-point precision and compression artifacts
        return Mathf.Abs(c1.r - c2.r) <= COLOR_TOLERANCE &&
               Mathf.Abs(c1.g - c2.g) <= COLOR_TOLERANCE &&
               Mathf.Abs(c1.b - c2.b) <= COLOR_TOLERANCE &&
               Mathf.Abs(c1.a - c2.a) <= COLOR_TOLERANCE;
    }

    public int GetHashCode(Color c)
    {
        // Quantize to 256 levels (0-255) for consistent hashing
        // This ensures colors that are equal within tolerance get the same hash
        int r = Mathf.RoundToInt(c.r * 255f);
        int g = Mathf.RoundToInt(c.g * 255f);
        int b = Mathf.RoundToInt(c.b * 255f);
        int a = Mathf.RoundToInt(c.a * 255f);
        
        // Combine hash codes
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + r;
            hash = hash * 31 + g;
            hash = hash * 31 + b;
            hash = hash * 31 + a;
            return hash;
        }
    }
}

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
    /// <param name="image">Source image to analyze. 
    /// For best results, ensure texture import settings are:
    /// - Read/Write Enabled: TRUE
    /// - Compression: None
    /// - Filter Mode: Point (no filter)
    /// - Texture Type: Default or Sprite
    /// </param>
    /// <param name="tileSize">Size of each tile in pixels (default: 1 for per-pixel analysis)</param>
    public ImageToWFC(Texture2D image, int tileSize = 1)
    {
        this.sourceImage = image;
        this.tileSize = tileSize;
        this.colorToTileName = new Dictionary<Color, string>(new ColorEqualityComparer());
        this.tileNameToColor = new Dictionary<string, Color>();
        this.adjacencyRules = new Dictionary<string, HashSet<string>>();
        
        // Warn if texture filtering is enabled (can cause color bleeding)
        if (image.filterMode != FilterMode.Point)
        {
            Debug.LogWarning($"ImageToWFC: Texture '{image.name}' has filter mode '{image.filterMode}'. " +
                           "For accurate color analysis, set Filter Mode to 'Point (no filter)' in import settings.");
        }
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
        
        // Log summary after analysis
        Debug.Log($"ImageToWFC Analysis Complete: Found {nextTileId} unique colors");
        
        // If too many colors, warn the user
        if (nextTileId > 50)
        {
            Debug.LogWarning($"ImageToWFC: Found {nextTileId} unique colors. This may create a complex ruleset. " +
                           "Consider using higher tileSize or simplifying your image.");
        }
        
        // Debug: Log each unique color found (only if count is reasonable)
        if (nextTileId <= 20)
        {
            foreach (var kvp in tileNameToColor)
            {
                Color c = kvp.Value;
                Debug.Log($"  {kvp.Key}: RGB({c.r:F3}, {c.g:F3}, {c.b:F3}, {c.a:F3})");
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
