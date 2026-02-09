using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Example script demonstrating how to use the ImageToWFC functionality programmatically
/// This can be attached to a GameObject for testing the image import feature
/// </summary>
public class ImageWFCExample : MonoBehaviour
{
    [Header("Image Input")]
    [Tooltip("Source image to analyze and generate from")]
    public Texture2D sourceImage;
    
    [Tooltip("Tile size in pixels (1 = per-pixel analysis)")]
    public int tileSize = 1;
    
    [Header("Generation Settings")]
    [Tooltip("Width of the output map")]
    public int outputWidth = 20;
    
    [Tooltip("Height of the output map")]
    public int outputHeight = 20;
    
    [Tooltip("Random seed (0 for random)")]
    public int seed = 0;
    
    [Header("Rendering")]
    [Tooltip("Parent transform for generated tiles")]
    public Transform mapContainer;
    
    [Tooltip("Size of rendered tiles")]
    public float renderTileSize = 1f;
    
    private List<GameObject> generatedTiles = new List<GameObject>();

    /// <summary>
    /// Generate map from image when spacebar is pressed
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateFromImage();
        }
    }

    /// <summary>
    /// Generate a map from the source image
    /// </summary>
    public void GenerateFromImage()
    {
        if (sourceImage == null)
        {
            Debug.LogError("No source image assigned!");
            return;
        }

        if (!sourceImage.isReadable)
        {
            Debug.LogError("Source image must be readable! Enable Read/Write in import settings.");
            return;
        }

        // Clear previous tiles
        ClearMap();

        // Analyze the image
        ImageToWFC analyzer = new ImageToWFC(sourceImage, tileSize);
        analyzer.Analyze();

        // Get tiles and rules
        string[] tiles = analyzer.GetTiles();
        Dictionary<string, List<string>> rules = analyzer.GetRules();
        Dictionary<string, Color> colorMap = analyzer.GetColorMap();

        Debug.Log($"Analyzed image: {analyzer.GetTileCount()} unique tiles found");

        // Generate map using WFC
        int actualSeed = (seed == 0) ? System.Environment.TickCount : seed;
        WaveFunctionCollapse wfc = new WaveFunctionCollapse(outputWidth, outputHeight, tiles, rules, actualSeed);
        
        bool success = wfc.Run();
        
        if (!success)
        {
            Debug.LogError("WFC generation failed! Try different settings.");
            return;
        }

        // Render the result
        string[,] result = wfc.GetResult();
        RenderMap(result, colorMap);
        
        Debug.Log($"Successfully generated {outputWidth}x{outputHeight} map from image using seed {actualSeed}");
    }

    /// <summary>
    /// Render the generated map
    /// </summary>
    private void RenderMap(string[,] map, Dictionary<string, Color> colorMap)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        // Calculate offset to center the map
        float offsetX = -(width * renderTileSize) / 2f;
        float offsetY = -(height * renderTileSize) / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                string tileName = map[y, x];
                if (string.IsNullOrEmpty(tileName)) continue;

                // Calculate position
                Vector3 position = new Vector3(
                    offsetX + x * renderTileSize + renderTileSize / 2f,
                    offsetY + (height - 1 - y) * renderTileSize + renderTileSize / 2f,
                    0
                );

                // Create tile
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.position = position;
                tile.transform.localScale = new Vector3(renderTileSize * 0.95f, renderTileSize * 0.95f, 1);
                
                if (mapContainer != null)
                {
                    tile.transform.parent = mapContainer;
                }

                // Set color
                if (colorMap.ContainsKey(tileName))
                {
                    Renderer renderer = tile.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = colorMap[tileName];
                    }
                }

                generatedTiles.Add(tile);
            }
        }
    }

    /// <summary>
    /// Clear the generated map
    /// </summary>
    private void ClearMap()
    {
        foreach (GameObject tile in generatedTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        generatedTiles.Clear();
    }

    void OnDestroy()
    {
        ClearMap();
    }
}
