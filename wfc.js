/**
 * Wave Function Collapse Algorithm Implementation
 */

class WaveFunctionCollapse {
    constructor(width, height, tiles, rules, seed = null) {
        this.width = width;
        this.height = height;
        this.tiles = tiles;
        this.rules = rules; // rules[tileA] = [tileB, tileC] means tileA can be adjacent to tileB and tileC
        this.grid = [];
        this.seed = seed || Date.now();
        this.rng = this.seededRandom(this.seed);
        
        this.initializeGrid();
    }
    
    /**
     * Seeded random number generator using mulberry32
     */
    seededRandom(seed) {
        return function() {
            let t = seed += 0x6D2B79F5;
            t = Math.imul(t ^ t >>> 15, t | 1);
            t ^= t + Math.imul(t ^ t >>> 7, t | 61);
            return ((t ^ t >>> 14) >>> 0) / 4294967296;
        }
    }
    
    /**
     * Initialize the grid with all possible tiles for each cell
     */
    initializeGrid() {
        for (let y = 0; y < this.height; y++) {
            this.grid[y] = [];
            for (let x = 0; x < this.width; x++) {
                this.grid[y][x] = {
                    collapsed: false,
                    options: [...this.tiles]
                };
            }
        }
    }
    
    /**
     * Get valid neighbors for a position
     */
    getNeighbors(x, y) {
        const neighbors = [];
        if (x > 0) neighbors.push({x: x - 1, y: y, dir: 'left'});
        if (x < this.width - 1) neighbors.push({x: x + 1, y: y, dir: 'right'});
        if (y > 0) neighbors.push({x: x, y: y - 1, dir: 'up'});
        if (y < this.height - 1) neighbors.push({x: x, y: y + 1, dir: 'down'});
        return neighbors;
    }
    
    /**
     * Find the cell with minimum entropy (least options)
     */
    findMinEntropyCell() {
        let minEntropy = Infinity;
        let candidates = [];
        
        for (let y = 0; y < this.height; y++) {
            for (let x = 0; x < this.width; x++) {
                const cell = this.grid[y][x];
                if (!cell.collapsed && cell.options.length > 0) {
                    const entropy = cell.options.length + this.rng() * 0.1; // Add small random factor
                    
                    if (entropy < minEntropy) {
                        minEntropy = entropy;
                        candidates = [{x, y}];
                    } else if (Math.abs(entropy - minEntropy) < 0.01) {
                        candidates.push({x, y});
                    }
                }
            }
        }
        
        if (candidates.length === 0) return null;
        
        // Pick random candidate
        const index = Math.floor(this.rng() * candidates.length);
        return candidates[index];
    }
    
    /**
     * Collapse a cell by choosing one of its possible options
     */
    collapseCell(x, y) {
        const cell = this.grid[y][x];
        if (cell.options.length === 0) {
            return false; // Contradiction
        }
        
        // Choose random option
        const index = Math.floor(this.rng() * cell.options.length);
        const chosen = cell.options[index];
        
        cell.options = [chosen];
        cell.collapsed = true;
        
        return true;
    }
    
    /**
     * Propagate constraints to neighbors
     */
    propagate(x, y) {
        const stack = [{x, y}];
        
        while (stack.length > 0) {
            const current = stack.pop();
            const currentCell = this.grid[current.y][current.x];
            
            const neighbors = this.getNeighbors(current.x, current.y);
            
            for (const neighbor of neighbors) {
                const neighborCell = this.grid[neighbor.y][neighbor.x];
                
                if (neighborCell.collapsed) continue;
                
                // Calculate valid options for neighbor based on current cell
                const validOptions = [];
                
                for (const option of neighborCell.options) {
                    let isValid = false;
                    
                    for (const currentOption of currentCell.options) {
                        // Check if option can be adjacent to currentOption
                        if (this.canBeAdjacent(option, currentOption)) {
                            isValid = true;
                            break;
                        }
                    }
                    
                    if (isValid) {
                        validOptions.push(option);
                    }
                }
                
                // If options changed, propagate further
                if (validOptions.length < neighborCell.options.length) {
                    neighborCell.options = validOptions;
                    
                    if (validOptions.length === 0) {
                        return false; // Contradiction
                    }
                    
                    stack.push({x: neighbor.x, y: neighbor.y});
                }
            }
        }
        
        return true;
    }
    
    /**
     * Check if two tiles can be adjacent based on rules
     */
    canBeAdjacent(tile1, tile2) {
        return this.rules[tile1] && this.rules[tile1].includes(tile2);
    }
    
    /**
     * Check if the grid is fully collapsed
     */
    isComplete() {
        for (let y = 0; y < this.height; y++) {
            for (let x = 0; x < this.width; x++) {
                if (!this.grid[y][x].collapsed) {
                    return false;
                }
            }
        }
        return true;
    }
    
    /**
     * Run the Wave Function Collapse algorithm
     */
    run(maxIterations = 10000) {
        let iterations = 0;
        
        while (!this.isComplete() && iterations < maxIterations) {
            // Find cell with minimum entropy
            const cell = this.findMinEntropyCell();
            
            if (!cell) {
                break; // No more cells to collapse or contradiction
            }
            
            // Collapse the cell
            if (!this.collapseCell(cell.x, cell.y)) {
                console.error('Contradiction during collapse');
                return false;
            }
            
            // Propagate constraints
            if (!this.propagate(cell.x, cell.y)) {
                console.error('Contradiction during propagation');
                return false;
            }
            
            iterations++;
        }
        
        if (iterations >= maxIterations) {
            console.warn('Max iterations reached');
            return false;
        }
        
        return this.isComplete();
    }
    
    /**
     * Get the final collapsed grid
     */
    getResult() {
        const result = [];
        for (let y = 0; y < this.height; y++) {
            result[y] = [];
            for (let x = 0; x < this.width; x++) {
                const cell = this.grid[y][x];
                result[y][x] = cell.options.length > 0 ? cell.options[0] : null;
            }
        }
        return result;
    }
}

/**
 * Default tile configuration for the three-color system
 */
const DEFAULT_TILES = ['blue', 'green', 'yellow'];

/**
 * Default rules: yellow can touch blue and green, but blue and green cannot touch each other
 */
const DEFAULT_RULES = {
    'blue': ['blue', 'yellow'],
    'green': ['green', 'yellow'],
    'yellow': ['blue', 'green', 'yellow']
};
