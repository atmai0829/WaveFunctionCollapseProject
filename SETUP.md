# Unity Scene Setup Guide

This guide will help you set up the Wave Function Collapse scene in Unity.

## Prerequisites

- Unity 2021.3 or later installed
- TextMeshPro package (imported automatically when needed)

## Step 1: Import TextMeshPro

1. When you first open a scene with TextMeshPro components, Unity will prompt you to import TMP Essentials
2. Click "Import TMP Essentials" in the popup window
3. Wait for the import to complete

## Step 2: Create the UI Canvas

1. Right-click in the Hierarchy window
2. Select `UI > Canvas`
3. In the Canvas component, set:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

## Step 3: Create UI Elements

### Seed Input
1. Right-click Canvas → `UI > Input Field - TextMeshPro`
2. Rename to "SeedInput"
3. Configure placeholder text: "Enter seed (leave empty for random)"

### Width Input
1. Right-click Canvas → `UI > Input Field - TextMeshPro`
2. Rename to "WidthInput"
3. Set Content Type to: Integer Number
4. Set Text to: "20"

### Height Input
1. Right-click Canvas → `UI > Input Field - TextMeshPro`
2. Rename to "HeightInput"
3. Set Content Type to: Integer Number
4. Set Text to: "20"

### Tileset Mode Dropdown
1. Right-click Canvas → `UI > Dropdown - TextMeshPro`
2. Rename to "TilesetModeDropdown"
3. Add options:
   - "Default (Blue, Green, Yellow)"
   - "Custom Tileset"

### Custom Tileset Input
1. Right-click Canvas → `UI > Input Field - TextMeshPro`
2. Rename to "CustomTilesetInput"
3. Configure:
   - Line Type: Multi Line Newline
   - Set initial state to inactive (unchecked in Inspector)

### Generate Button
1. Right-click Canvas → `UI > Button - TextMeshPro`
2. Rename to "GenerateButton"
3. Set button text to: "Generate Map"

### Random Seed Button
1. Right-click Canvas → `UI > Button - TextMeshPro`
2. Rename to "RandomSeedButton"
3. Set button text to: "Random Seed"

### Current Seed Text
1. Right-click Canvas → `UI > Text - TextMeshPro`
2. Rename to "CurrentSeedText"
3. Set initial text to: "Current Seed: "

## Step 4: Create Map Container

1. Right-click in Hierarchy → Create Empty
2. Rename to "MapContainer"
3. Reset transform (Position: 0,0,0)

## Step 5: Create WFC Manager

1. Right-click in Hierarchy → Create Empty
2. Rename to "WFCManager"
3. Click "Add Component" and add the `WFCManager` script
4. Assign all UI references:
   - Seed Input Field → SeedInput
   - Width Input Field → WidthInput
   - Height Input Field → HeightInput
   - Tileset Mode Dropdown → TilesetModeDropdown
   - Custom Tileset Input → CustomTilesetInput
   - Custom Tileset Panel → CustomTilesetInput (or parent panel)
   - Generate Button → GenerateButton
   - Random Seed Button → RandomSeedButton
   - Current Seed Text → CurrentSeedText
   - Map Container → MapContainer transform

5. Configure Default Tiles (in Inspector):
   - Set size to 3
   - Element 0:
     - Tile Name: blue
     - Tile Color: RGB(52, 152, 219) or #3498DB
     - Allowed Neighbors: Add "blue", "yellow"
   - Element 1:
     - Tile Name: green
     - Tile Color: RGB(46, 204, 113) or #2ECC71
     - Allowed Neighbors: Add "green", "yellow"
   - Element 2:
     - Tile Name: yellow
     - Tile Color: RGB(241, 196, 15) or #F1C40F
     - Allowed Neighbors: Add "blue", "green", "yellow"

## Step 6: Configure Camera

1. Select Main Camera
2. Set:
   - Projection: Orthographic
   - Size: 15
   - Background: Choose a color you like (e.g., light gray)
   - Position: (0, 0, -10)

## Step 7: Test the Scene

1. Press Play
2. You should see a 20x20 tile map generated automatically
3. Try changing the seed, width, and height values
4. Click "Generate Map" to create new maps
5. Click "Random Seed" to generate random seeds

## Troubleshooting

### Missing TMP_InputField or TMP_Dropdown
- Make sure TextMeshPro is properly imported
- Go to Window → TextMeshPro → Import TMP Essential Resources

### UI not visible
- Check that Canvas is set to Screen Space - Overlay
- Verify all UI elements are children of the Canvas

### Map not generating
- Check Console for errors
- Verify all references are assigned in WFCManager
- Make sure Main Camera is set to Orthographic

### Tiles not showing correct colors
- Verify Default Tiles configuration in WFCManager
- Check that Material colors are being applied correctly

## Custom Tileset JSON Example

```json
{
  "tiles": [
    {
      "tileName": "blue",
      "tileColor": {"r": 0.2, "g": 0.6, "b": 0.86, "a": 1.0},
      "allowedNeighbors": ["yellow"]
    },
    {
      "tileName": "green",
      "tileColor": {"r": 0.18, "g": 0.8, "b": 0.44, "a": 1.0},
      "allowedNeighbors": ["yellow"]
    },
    {
      "tileName": "yellow",
      "tileColor": {"r": 0.95, "g": 0.77, "b": 0.06, "a": 1.0},
      "allowedNeighbors": ["blue", "green"]
    }
  ]
}
```

## Next Steps

- Customize the UI layout and styling
- Create a custom tile prefab for better visuals
- Add tile borders or patterns
- Implement save/load functionality
- Add export to image feature
