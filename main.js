/**
 * Main application logic and UI handling
 */

// Color mapping for tiles
const TILE_COLORS = {
    'blue': '#3498db',
    'green': '#2ecc71',
    'yellow': '#f1c40f',
    'red': '#e74c3c',
    'purple': '#9b59b6',
    'orange': '#e67e22',
    'cyan': '#1abc9c',
    'pink': '#fd79a8',
    'brown': '#8b4513',
    'gray': '#95a5a6'
};

// Canvas and context
let canvas, ctx;
let currentMap = null;
let currentTiles = DEFAULT_TILES;
let currentRules = DEFAULT_RULES;

/**
 * Initialize the application
 */
function init() {
    canvas = document.getElementById('mapCanvas');
    ctx = canvas.getContext('2d');
    
    // Set up event listeners
    document.getElementById('generateBtn').addEventListener('click', generateMap);
    document.getElementById('randomSeedBtn').addEventListener('click', generateRandomSeed);
    document.getElementById('tilesetMode').addEventListener('change', handleTilesetModeChange);
    
    // Generate initial map
    generateMap();
}

/**
 * Handle tileset mode change
 */
function handleTilesetModeChange() {
    const mode = document.getElementById('tilesetMode').value;
    const customDiv = document.getElementById('customTilesetDiv');
    
    if (mode === 'custom') {
        customDiv.style.display = 'block';
    } else {
        customDiv.style.display = 'none';
    }
}

/**
 * Generate a random seed
 */
function generateRandomSeed() {
    const seed = Math.floor(Math.random() * 1000000);
    document.getElementById('seed').value = seed;
}

/**
 * Parse custom tileset from JSON
 */
function parseCustomTileset() {
    const customTilesetText = document.getElementById('customTileset').value.trim();
    
    if (!customTilesetText) {
        alert('Please enter a custom tileset configuration');
        return null;
    }
    
    try {
        const config = JSON.parse(customTilesetText);
        
        if (!config.tiles || !Array.isArray(config.tiles) || config.tiles.length === 0) {
            alert('Tileset must have a "tiles" array with at least one tile');
            return null;
        }
        
        if (!config.rules || typeof config.rules !== 'object') {
            alert('Tileset must have a "rules" object');
            return null;
        }
        
        // Validate that all tiles have rules
        for (const tile of config.tiles) {
            if (!config.rules[tile]) {
                alert(`Tile "${tile}" is missing rules`);
                return null;
            }
        }
        
        return config;
    } catch (e) {
        alert('Invalid JSON format: ' + e.message);
        return null;
    }
}

/**
 * Generate the map using Wave Function Collapse
 */
function generateMap() {
    // Get parameters from UI
    const seedInput = document.getElementById('seed').value.trim();
    const seed = seedInput ? parseInt(seedInput) : null;
    const width = parseInt(document.getElementById('mapWidth').value);
    const height = parseInt(document.getElementById('mapHeight').value);
    const tilesetMode = document.getElementById('tilesetMode').value;
    
    // Validate inputs
    if (isNaN(width) || width < 5 || width > 100) {
        alert('Map width must be between 5 and 100');
        return;
    }
    
    if (isNaN(height) || height < 5 || height > 100) {
        alert('Map height must be between 5 and 100');
        return;
    }
    
    // Get tiles and rules based on mode
    let tiles, rules;
    
    if (tilesetMode === 'custom') {
        const config = parseCustomTileset();
        if (!config) return;
        
        tiles = config.tiles;
        rules = config.rules;
    } else {
        tiles = DEFAULT_TILES;
        rules = DEFAULT_RULES;
    }
    
    currentTiles = tiles;
    currentRules = rules;
    
    // Create and run WFC
    const wfc = new WaveFunctionCollapse(width, height, tiles, rules, seed);
    const success = wfc.run();
    
    if (!success) {
        alert('Failed to generate map. Try a different seed or configuration.');
        return;
    }
    
    currentMap = wfc.getResult();
    
    // Update seed display
    const actualSeed = wfc.seed;
    document.getElementById('currentSeed').textContent = `Current Seed: ${actualSeed}`;
    if (!seedInput) {
        document.getElementById('seed').value = actualSeed;
    }
    
    // Update legend
    updateLegend(tiles);
    
    // Render the map
    renderMap(currentMap);
}

/**
 * Update the legend based on current tiles
 */
function updateLegend(tiles) {
    const legendItems = document.getElementById('legendItems');
    legendItems.innerHTML = '';
    
    for (const tile of tiles) {
        const item = document.createElement('div');
        item.className = 'legend-item';
        
        const colorBox = document.createElement('div');
        colorBox.className = 'legend-color';
        colorBox.style.backgroundColor = TILE_COLORS[tile] || '#999';
        
        const label = document.createElement('span');
        label.textContent = tile.charAt(0).toUpperCase() + tile.slice(1);
        
        item.appendChild(colorBox);
        item.appendChild(label);
        legendItems.appendChild(item);
    }
}

/**
 * Render the map on the canvas
 */
function renderMap(map) {
    if (!map || map.length === 0) {
        return;
    }
    
    const height = map.length;
    const width = map[0].length;
    
    // Calculate tile size to fit canvas nicely
    const maxCanvasSize = 800;
    const tileSize = Math.floor(Math.min(maxCanvasSize / width, maxCanvasSize / height));
    
    // Set canvas size
    canvas.width = width * tileSize;
    canvas.height = height * tileSize;
    
    // Clear canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // Draw tiles
    for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
            const tile = map[y][x];
            
            if (tile) {
                // Fill tile
                ctx.fillStyle = TILE_COLORS[tile] || '#999';
                ctx.fillRect(x * tileSize, y * tileSize, tileSize, tileSize);
                
                // Draw border
                ctx.strokeStyle = '#333';
                ctx.lineWidth = 1;
                ctx.strokeRect(x * tileSize, y * tileSize, tileSize, tileSize);
            } else {
                // Empty tile (error)
                ctx.fillStyle = '#000';
                ctx.fillRect(x * tileSize, y * tileSize, tileSize, tileSize);
            }
        }
    }
}

/**
 * Validate that the map follows the rules
 */
function validateMap(map, rules) {
    const height = map.length;
    const width = map[0].length;
    
    for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
            const tile = map[y][x];
            
            // Check right neighbor
            if (x < width - 1) {
                const rightTile = map[y][x + 1];
                if (!rules[tile] || !rules[tile].includes(rightTile)) {
                    console.error(`Rule violation at (${x}, ${y}): ${tile} cannot be adjacent to ${rightTile}`);
                    return false;
                }
            }
            
            // Check down neighbor
            if (y < height - 1) {
                const downTile = map[y + 1][x];
                if (!rules[tile] || !rules[tile].includes(downTile)) {
                    console.error(`Rule violation at (${x}, ${y}): ${tile} cannot be adjacent to ${downTile}`);
                    return false;
                }
            }
        }
    }
    
    return true;
}

// Initialize on page load
window.addEventListener('DOMContentLoaded', init);
