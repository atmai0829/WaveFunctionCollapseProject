using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wave Function Collapse Algorithm Implementation
/// </summary>
public class WaveFunctionCollapse
{
    // Algorithm constants
    private const float ENTROPY_NOISE_FACTOR = 0.1f;
    private const float ENTROPY_THRESHOLD = 0.01f;
    private const int DEFAULT_MAX_ITERATIONS = 10000;

    private int width;
    private int height;
    private string[] tiles;
    private Dictionary<string, List<string>> rules;
    private Cell[,] grid;
    private int seed;
    private System.Random rng;

    private class Cell
    {
        public bool collapsed;
        public List<string> options;

        public Cell(string[] allTiles)
        {
            collapsed = false;
            options = new List<string>(allTiles);
        }
    }

    public WaveFunctionCollapse(int width, int height, string[] tiles, Dictionary<string, List<string>> rules, int? seed = null)
    {
        this.width = width;
        this.height = height;
        this.tiles = tiles;
        this.rules = rules;
        this.seed = seed ?? Environment.TickCount;
        this.rng = new System.Random(this.seed);

        InitializeGrid();
    }

    public int GetSeed()
    {
        return seed;
    }

    private void InitializeGrid()
    {
        grid = new Cell[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = new Cell(tiles);
            }
        }
    }

    private List<Vector2Int> GetNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (x > 0) neighbors.Add(new Vector2Int(x - 1, y));
        if (x < width - 1) neighbors.Add(new Vector2Int(x + 1, y));
        if (y > 0) neighbors.Add(new Vector2Int(x, y - 1));
        if (y < height - 1) neighbors.Add(new Vector2Int(x, y + 1));
        return neighbors;
    }

    private Vector2Int? FindMinEntropyCell()
    {
        float minEntropy = float.MaxValue;
        List<Vector2Int> candidates = new List<Vector2Int>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = grid[y, x];
                if (!cell.collapsed && cell.options.Count > 0)
                {
                    float entropy = cell.options.Count + ((float)rng.NextDouble() * ENTROPY_NOISE_FACTOR);

                    if (entropy < minEntropy)
                    {
                        minEntropy = entropy;
                        candidates.Clear();
                        candidates.Add(new Vector2Int(x, y));
                    }
                    else if (Mathf.Abs(entropy - minEntropy) < ENTROPY_THRESHOLD)
                    {
                        candidates.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (candidates.Count == 0) return null;

        int index = rng.Next(candidates.Count);
        return candidates[index];
    }

    private bool CollapseCell(int x, int y)
    {
        Cell cell = grid[y, x];
        if (cell.options.Count == 0) return false;

        int index = rng.Next(cell.options.Count);
        string chosen = cell.options[index];

        cell.options.Clear();
        cell.options.Add(chosen);
        cell.collapsed = true;

        return true;
    }

    private bool Propagate(int x, int y)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(x, y));

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            Cell currentCell = grid[current.y, current.x];

            List<Vector2Int> neighbors = GetNeighbors(current.x, current.y);

            foreach (Vector2Int neighbor in neighbors)
            {
                Cell neighborCell = grid[neighbor.y, neighbor.x];

                if (neighborCell.collapsed) continue;

                List<string> validOptions = new List<string>();

                foreach (string option in neighborCell.options)
                {
                    bool isValid = false;

                    foreach (string currentOption in currentCell.options)
                    {
                        if (CanBeAdjacent(option, currentOption))
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        validOptions.Add(option);
                    }
                }

                if (validOptions.Count < neighborCell.options.Count)
                {
                    neighborCell.options = validOptions;

                    if (validOptions.Count == 0)
                    {
                        return false;
                    }

                    stack.Push(neighbor);
                }
            }
        }

        return true;
    }

    private bool CanBeAdjacent(string tile1, string tile2)
    {
        return rules.ContainsKey(tile1) && rules[tile1].Contains(tile2);
    }

    private bool IsComplete()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!grid[y, x].collapsed)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool Run(int maxIterations = DEFAULT_MAX_ITERATIONS)
    {
        int iterations = 0;

        while (!IsComplete() && iterations < maxIterations)
        {
            Vector2Int? cell = FindMinEntropyCell();

            if (!cell.HasValue)
            {
                break;
            }

            if (!CollapseCell(cell.Value.x, cell.Value.y))
            {
                Debug.LogError("Contradiction during collapse");
                return false;
            }

            if (!Propagate(cell.Value.x, cell.Value.y))
            {
                Debug.LogError("Contradiction during propagation");
                return false;
            }

            iterations++;
        }

        if (iterations >= maxIterations)
        {
            Debug.LogWarning("Max iterations reached");
            return false;
        }

        return IsComplete();
    }

    public string[,] GetResult()
    {
        string[,] result = new string[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = grid[y, x];
                result[y, x] = cell.options.Count > 0 ? cell.options[0] : null;
            }
        }
        return result;
    }
}
