using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple test to validate ImageToWFC functionality
/// This creates a test pattern and validates the analysis
/// </summary>
public class TestImageWFC : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool runTestOnStart = true;

    void Start()
    {
        if (runTestOnStart)
        {
            RunTests();
        }
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    public void RunTests()
    {
        Debug.Log("=== Starting ImageToWFC Tests ===");
        
        TestSimplePattern();
        TestCheckerboardPattern();
        TestSingleColor();
        
        Debug.Log("=== All ImageToWFC Tests Completed ===");
    }

    /// <summary>
    /// Test 1: Simple 2x2 pattern with 2 colors
    /// </summary>
    private void TestSimplePattern()
    {
        Debug.Log("\n--- Test 1: Simple 2x2 Pattern ---");
        
        // Create a 2x2 test image with 2 colors
        // Red | Blue
        // Blue | Red
        Texture2D testImage = new Texture2D(2, 2, TextureFormat.RGB24, false);
        testImage.SetPixel(0, 0, Color.blue);  // Bottom-left
        testImage.SetPixel(1, 0, Color.red);   // Bottom-right
        testImage.SetPixel(0, 1, Color.red);   // Top-left
        testImage.SetPixel(1, 1, Color.blue);  // Top-right
        testImage.Apply();

        // Analyze the image
        ImageToWFC analyzer = new ImageToWFC(testImage, 1);
        analyzer.Analyze();

        // Validate results
        string[] tiles = analyzer.GetTiles();
        Dictionary<string, List<string>> rules = analyzer.GetRules();
        Dictionary<string, Color> colorMap = analyzer.GetColorMap();

        Debug.Log($"Found {tiles.Length} unique tiles (expected: 2)");
        Assert(tiles.Length == 2, "Should find exactly 2 unique tiles");

        Debug.Log($"Color map contains {colorMap.Count} entries");
        Assert(colorMap.Count == 2, "Color map should have 2 entries");

        // Each tile should be able to be adjacent to the other and itself
        foreach (string tile in tiles)
        {
            Assert(rules.ContainsKey(tile), $"Rules should contain entry for {tile}");
            Debug.Log($"Tile '{tile}' can be adjacent to {rules[tile].Count} tiles");
        }

        Debug.Log("✓ Test 1 Passed");
        
        DestroyImmediate(testImage);
    }

    /// <summary>
    /// Test 2: Checkerboard pattern
    /// </summary>
    private void TestCheckerboardPattern()
    {
        Debug.Log("\n--- Test 2: Checkerboard 4x4 Pattern ---");
        
        // Create a 4x4 checkerboard
        Texture2D testImage = new Texture2D(4, 4, TextureFormat.RGB24, false);
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Color color = ((x + y) % 2 == 0) ? Color.black : Color.white;
                testImage.SetPixel(x, y, color);
            }
        }
        testImage.Apply();

        // Analyze
        ImageToWFC analyzer = new ImageToWFC(testImage, 1);
        analyzer.Analyze();

        string[] tiles = analyzer.GetTiles();
        Dictionary<string, List<string>> rules = analyzer.GetRules();

        Debug.Log($"Found {tiles.Length} unique tiles (expected: 2)");
        Assert(tiles.Length == 2, "Checkerboard should have 2 unique tiles");

        // In a checkerboard, each color should only be adjacent to the other color
        foreach (string tile in tiles)
        {
            Debug.Log($"Tile '{tile}' adjacency rules: {string.Join(", ", rules[tile])}");
            // Each tile should be adjacent to itself and the other tile
            Assert(rules[tile].Count == 2, $"Each tile should have 2 adjacencies in checkerboard");
        }

        Debug.Log("✓ Test 2 Passed");
        
        DestroyImmediate(testImage);
    }

    /// <summary>
    /// Test 3: Single color image
    /// </summary>
    private void TestSingleColor()
    {
        Debug.Log("\n--- Test 3: Single Color 3x3 ---");
        
        // Create a 3x3 image with single color
        Texture2D testImage = new Texture2D(3, 3, TextureFormat.RGB24, false);
        Color green = new Color(0.0f, 1.0f, 0.0f);
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                testImage.SetPixel(x, y, green);
            }
        }
        testImage.Apply();

        // Analyze
        ImageToWFC analyzer = new ImageToWFC(testImage, 1);
        analyzer.Analyze();

        string[] tiles = analyzer.GetTiles();
        Dictionary<string, List<string>> rules = analyzer.GetRules();

        Debug.Log($"Found {tiles.Length} unique tiles (expected: 1)");
        Assert(tiles.Length == 1, "Single color should result in 1 tile");

        // The single tile should only be adjacent to itself
        Assert(rules[tiles[0]].Count == 1, "Single tile should only be adjacent to itself");

        Debug.Log("✓ Test 3 Passed");
        
        DestroyImmediate(testImage);
    }

    /// <summary>
    /// Simple assertion helper
    /// </summary>
    private void Assert(bool condition, string message)
    {
        if (!condition)
        {
            Debug.LogError($"ASSERTION FAILED: {message}");
        }
    }
}
