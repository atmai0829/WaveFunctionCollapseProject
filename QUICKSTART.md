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
- Check Main Camera is Orthographic
- Verify MapContainer has generated child objects

### Compilation Errors
- Make sure all three scripts are in Assets/Scripts/
- Check Unity is using .NET 4.x or later (Project Settings → Player → Other Settings)

## Next Steps

1. Complete full UI setup (see SETUP.md)
2. Test with different seeds
3. Try custom tilesets
4. Modify tile colors and rules
5. Export generated maps

## Support

For detailed scene setup: See `SETUP.md`
For algorithm details: See `README.md`
For Unity scripting: Check inline code comments

## License

MIT License - Feel free to use and modify!
