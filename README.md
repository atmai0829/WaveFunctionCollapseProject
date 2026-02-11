# Wave Function Collapse Project (Unity)

A Unity implementation of the Wave Function Collapse algorithm for procedural tile map generation with interactive UI.

## Features

- **Three-Tile Color System**: Generate maps with blue, green, and yellow tiles
- **Image Import**: Import any image and generate maps based on its patterns and colors
- **Adjacency Rules**: 
  - Yellow can touch blue and green
  - Blue and green cannot touch each other
  - Blue can touch yellow
  - Green can touch yellow
- **Seed-Based Generation**: Input custom seeds for reproducible map generation
- **Adjustable Map Size**: Configure map dimensions (default: 20x20, range: 5-100)
- **Custom Tileset Support**: Define your own tiles and adjacency rules via JSON
- **Interactive Unity UI**: Modern UI with TextMeshPro controls

## Project Structure

```
Assets/
├── Scripts/
│   ├── WaveFunctionCollapse.cs  - Core WFC algorithm
│   ├── WFCManager.cs             - Main UI controller and map generator
│   ├── TileConfig.cs             - Tile configuration classes
│   ├── ImageToWFC.cs             - Image analysis for WFC generation
│   └── ImageWFCExample.cs        - Example script for programmatic image import
├── Scenes/
│   └── MainScene.unity           - Main scene with UI and map display
└── Prefabs/
    └── Tile.prefab               - Tile prefab for rendering
```

## Setup Instructions

### Requirements
- Unity 2021.3 or later
- TextMeshPro package (should be auto-imported)

### Opening the Project
1. Open Unity Hub
2. Click "Add" and select this project folder
3. Open the project with Unity 2021.3 or later
4. Open the `Assets/Scenes/MainScene.unity` scene

### Scene Setup

The MainScene should contain:
- **Canvas** with UI elements:
  - Seed input field (TMP_InputField)
  - Width input field (TMP_InputField)
  - Height input field (TMP_InputField)
  - Tileset mode dropdown (TMP_Dropdown)
  - Custom tileset input (TMP_InputField, multiline)
  - Generate button
  - Random Seed button
  - Current seed text display
- **WFCManager** GameObject with the `WFCManager.cs` script attached
- **Map Container** Empty GameObject to hold generated tiles
- **Main Camera** set to Orthographic mode

### Script Configuration

Attach the `WFCManager.cs` script to a GameObject and assign:
- All UI references (input fields, buttons, text)
- Map Container transform
- Tile prefab (optional, will create quads if not provided)
- Default tile configurations (blue, green, yellow)

## Usage

### In Unity Editor
1. Press Play in the Unity Editor
2. The map will generate automatically with default settings
3. Modify seed, width, height values in the UI
4. Click "Generate Map" to create a new map
5. Click "Random Seed" to generate a random seed value

### Generate from Image

You can import an image and generate maps based on its patterns:

#### Using WFCManager (UI Mode)
1. In the WFCManager inspector, find the "Image Import" section
2. Assign a Texture2D to the "Source Image" field
3. Set "Image Tile Size" (1 = per-pixel analysis, higher values = larger pattern tiles)
4. In the Tileset Mode dropdown, select "From Image" (option 2)
5. Click "Generate Map" to create a map based on the image patterns

**Important**: The source image must have proper import settings:
- Select the image in the Project window
- In the Inspector, configure these settings:
  - **Read/Write Enabled**: ✓ Check this box
  - **Compression**: None (to preserve exact colors)
  - **Filter Mode**: Point (no filter) - prevents color bleeding
  - **Format**: Automatic or RGBA32 for best color accuracy
- Click "Apply"

**Note**: If you're getting extra colors that aren't in your source image, make sure:
1. Filter Mode is set to "Point (no filter)" - other modes blend pixels causing color variations
2. Compression is set to "None" - compression can introduce color artifacts
3. Your image is saved in a lossless format (PNG recommended, not JPEG)

#### Using ImageWFCExample Script (Programmatic)
1. Create an empty GameObject
2. Add the `ImageWFCExample` component
3. Assign your source image (must be readable)
4. Configure generation settings (output size, seed, tile size)
5. Press Play and then press Spacebar to generate

#### How It Works
The image analyzer:
1. Scans the source image and identifies unique colors/tiles
2. Analyzes adjacency patterns between neighboring tiles
3. Builds a ruleset based on which colors appear next to each other
4. Uses WFC to generate new maps that follow the same patterns

Example use cases:
- Import a small hand-drawn tile pattern to generate larger variations
- Analyze existing game maps to create similar procedural maps
- Use pixel art as input to generate similar-looking patterns

### Custom Tileset Format

To use a custom tileset, select "Custom Tileset" mode and enter JSON:

```json
{
  "tiles": [
    {
      "tileName": "blue",
      "tileColor": {"r": 0.2, "g": 0.6, "b": 0.86, "a": 1.0},
      "allowedNeighbors": ["yellow", "red"]
    },
    {
      "tileName": "green",
      "tileColor": {"r": 0.18, "g": 0.8, "b": 0.44, "a": 1.0},
      "allowedNeighbors": ["yellow", "red"]
    },
    {
      "tileName": "yellow",
      "tileColor": {"r": 0.95, "g": 0.77, "b": 0.06, "a": 1.0},
      "allowedNeighbors": ["blue", "green", "red"]
    },
    {
      "tileName": "red",
      "tileColor": {"r": 0.91, "g": 0.3, "b": 0.24, "a": 1.0},
      "allowedNeighbors": ["blue", "green", "yellow"]
    }
  ]
}
```

## Algorithm

The Wave Function Collapse algorithm works by:
1. Starting with all tiles in a superposition (all possibilities)
2. Collapsing the cell with minimum entropy (fewest options)
3. Propagating constraints to neighboring cells
4. Repeating until the entire grid is collapsed

This implementation uses a seeded random number generator for reproducible results.

## Default Tile Rules

- **Blue**: Can be adjacent to blue and yellow tiles
- **Green**: Can be adjacent to green and yellow tiles  
- **Yellow**: Can be adjacent to blue, green, and yellow tiles

This ensures that blue and green tiles never touch each other, while yellow acts as a separator.

## License

MIT License