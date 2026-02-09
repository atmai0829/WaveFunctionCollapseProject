# Quick Start Guide - Wave Function Collapse Unity Project

## Overview
This Unity project implements the Wave Function Collapse algorithm for procedural tile map generation with three colors (blue, green, yellow) following specific adjacency rules.

## Opening the Project

1. **Install Unity Hub** (if not already installed)
   - Download from: https://unity.com/download

2. **Open Project**
   - Open Unity Hub
   - Click "Add" button
   - Navigate to this project folder
   - Select the folder and click "Select Folder"

3. **Choose Unity Version**
   - Project requires Unity 2021.3 or later
   - If you don't have it, Unity Hub will offer to install it

4. **Open Project**
   - Click on the project name in Unity Hub
   - Wait for Unity to load (first time may take a few minutes)

## Scene Setup (Required First Time)

The project includes the core scripts but you need to set up the UI scene. Follow these steps:

### 1. Import TextMeshPro
- When prompted, click "Import TMP Essentials"
- This is needed for the UI text and input fields

### 2. Set Up the Scene
Follow the detailed instructions in `SETUP.md` to create:
- UI Canvas with input fields
- Buttons for generation
- WFCManager GameObject
- Map container

Or, create a simpler version for testing:

#### Minimal Setup (Testing Only)
1. Create an empty GameObject named "WFCManager"
2. Add the `WFCManager` script component
3. Leave all UI references empty (script will work without UI)
4. Configure Default Tiles in Inspector:
   - Set size to 3
   - Add blue, green, yellow with colors and allowed neighbors
5. Press Play - map should generate in scene view

## Understanding the Code

### WaveFunctionCollapse.cs
Core algorithm that:
- Initializes grid with all possible tiles
- Finds minimum entropy cells
- Collapses cells to specific tiles
- Propagates constraints to neighbors

### WFCManager.cs
Unity-specific controller that:
- Manages UI inputs
- Runs the WFC algorithm
- Renders tiles as GameObjects
- Handles custom tilesets

### TileConfig.cs
Data structures for:
- Tile definitions (name, color)
- Adjacency rules (allowed neighbors)
- Custom tileset configurations

## Default Tile Rules

The default configuration includes three tiles:

**Blue Tile**
- Color: #3498DB (bright blue)
- Can touch: Blue, Yellow

**Green Tile**
- Color: #2ECC71 (bright green)
- Can touch: Green, Yellow

**Yellow Tile**
- Color: #F1C40F (golden yellow)
- Can touch: Blue, Green, Yellow

**Result**: Blue and green tiles never touch each other. Yellow acts as a connector.

## Testing Without UI

You can test the algorithm without setting up the full UI:

1. Create a test script:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class TestWFC : MonoBehaviour
{
    void Start()
    {
        string[] tiles = { "blue", "green", "yellow" };
        
        Dictionary<string, List<string>> rules = new Dictionary<string, List<string>>
        {
            { "blue", new List<string> { "blue", "yellow" } },
            { "green", new List<string> { "green", "yellow" } },
            { "yellow", new List<string> { "blue", "green", "yellow" } }
        };
        
        WaveFunctionCollapse wfc = new WaveFunctionCollapse(10, 10, tiles, rules, 12345);
        bool success = wfc.Run();
        
        if (success)
        {
            string[,] map = wfc.GetResult();
            Debug.Log("Map generated successfully!");
            
            // Print map to console
            for (int y = 0; y < 10; y++)
            {
                string row = "";
                for (int x = 0; x < 10; x++)
                {
                    row += map[y, x][0] + " ";
                }
                Debug.Log(row);
            }
        }
    }
}
```

2. Attach to any GameObject
3. Press Play
4. Check Console for output

## Common Issues

### "TMP_InputField not found"
- Import TextMeshPro essentials: Window → TextMeshPro → Import TMP Essential Resources

### "Missing reference"
- Check WFCManager Inspector
- Assign all required UI references
- Or remove UI-dependent code for testing

### "Map not visible"
- Switch to Scene view (not Game view)
- Check Main Camera is Orthographic (not Perspective)
- Verify MapContainer has generated child objects
- For ImageWFCExample: The camera is automatically adjusted, but ensure there's a camera tagged as "MainCamera"
- Check the Console for camera adjustment messages
- Verify the camera is positioned at (0, 0, -10) or another appropriate position

### "ImageWFCExample tiles not showing"
- Ensure Main Camera is set to **Orthographic** projection
- Check that the camera is tagged as "MainCamera"
- Look in Scene view instead of Game view to see the tiles
- Verify the Console shows "Camera adjusted to fit..." message after generation
- Check that tiles are being created (look in Hierarchy window for child objects)

### Compilation Errors
- Make sure all three scripts are in Assets/Scripts/
- Check Unity is using .NET 4.x or later (Project Settings → Player → Other Settings)

### "Texture is not readable"
- Select the texture in Project window
- In Inspector, enable "Read/Write Enabled" under Advanced settings
- Click Apply

### "No unique tiles found" or "Failed to generate from image"
- Check that your image has distinct colors
- Try a simpler image with fewer colors
- Increase imageTileSize if using a large detailed image

## Next Steps

1. Complete full UI setup (see SETUP.md)
2. Test with different seeds
3. Try custom tilesets
4. Try image import feature (see below)
5. Modify tile colors and rules
6. Export generated maps

## Using Image Import (New Feature!)

### Quick Test Without UI

1. Create an empty GameObject in your scene
2. Add the `TestImageWFC` component
3. Press Play
4. Check Console for test results

This validates that the image analysis system is working correctly.

### Example with ImageWFCExample

1. **Set up the camera** (if not already set):
   - Select Main Camera in Hierarchy
   - Set Projection to **Orthographic**
   - Set Position to (0, 0, -10)
   - The script will automatically adjust the orthographic size to fit the generated map

2. Create an empty GameObject
3. Add `ImageWFCExample` script component
4. In Inspector, assign:
   - Source Image: Any texture with Read/Write enabled
   - Output Width/Height: Desired map size (e.g., 20x20)
   - Tile Size: 1 for pixel-level, higher for pattern tiles
   - Map Container: (Optional) An empty GameObject to organize tiles
5. Press Play
6. Press **Spacebar** to generate
7. A new map will be created based on the image patterns!
8. The camera will automatically adjust to show the entire generated map

### Preparing Images for Import

For any image you want to use:
1. Select the image in Project window
2. In Inspector, find "Advanced" section
3. Check **"Read/Write Enabled"**
4. Click **Apply**

### Tips for Best Results

- **Small, simple images** (e.g., 10x10 pixels) work best for learning patterns
- **Pixel art** creates distinct, recognizable patterns
- **Hand-drawn tile samples** can generate larger variations
- **Too many unique colors** (100+) may create complex rulesets that fail to generate

### Creating Test Images

You can create simple test images in any image editor:
1. Create a small image (e.g., 5x5 pixels)
2. Paint a simple pattern with distinct colors
3. Save as PNG
4. Import to Unity and enable Read/Write
5. Assign to WFCManager or ImageWFCExample

## Support

For detailed scene setup: See `SETUP.md`
For algorithm details: See `README.md`
For Unity scripting: Check inline code comments

## License

MIT License - Feel free to use and modify!
