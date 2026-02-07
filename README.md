# Wave Function Collapse Project

An interactive web-based implementation of the Wave Function Collapse algorithm for procedural tile map generation.

## Features

- **Three-Tile Color System**: Generate maps with blue, green, and yellow tiles
- **Adjacency Rules**: 
  - Yellow can touch blue and green
  - Blue and green cannot touch each other
  - Blue can touch yellow
  - Green can touch yellow
- **Seed-Based Generation**: Input custom seeds for reproducible map generation
- **Adjustable Map Size**: Configure map dimensions (default: 20x20, range: 5-100)
- **Custom Tileset Support**: Define your own tiles and adjacency rules via JSON
- **Interactive UI**: Easy-to-use interface with real-time map generation

## Usage

1. Open `index.html` in a web browser
2. Configure your preferences:
   - Enter a seed (or leave empty for random generation)
   - Set map width and height
   - Choose between default or custom tileset
3. Click "Generate Map" to create your tile map
4. Use "Random Seed" to generate a random seed value

## Custom Tileset Format

To use a custom tileset, select "Custom Tileset" mode and enter a JSON configuration:

```json
{
  "tiles": ["blue", "green", "yellow", "red"],
  "rules": {
    "blue": ["yellow", "red"],
    "green": ["yellow", "red"],
    "yellow": ["blue", "green", "red"],
    "red": ["blue", "green", "yellow"]
  }
}
```

The `rules` object defines which tiles can be adjacent to each other.

## Files

- `index.html` - Main HTML structure and UI
- `style.css` - Styling and layout
- `wfc.js` - Wave Function Collapse algorithm implementation
- `main.js` - UI interaction and map rendering logic

## Algorithm

The Wave Function Collapse algorithm works by:
1. Starting with all tiles in a superposition (all possibilities)
2. Collapsing the cell with minimum entropy (fewest options)
3. Propagating constraints to neighboring cells
4. Repeating until the entire grid is collapsed

This implementation uses a seeded random number generator for reproducible results.