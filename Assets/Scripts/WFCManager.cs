using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WFCManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField seedInputField;
    [SerializeField] private TMP_InputField widthInputField;
    [SerializeField] private TMP_InputField heightInputField;
    [SerializeField] private TMP_Dropdown tilesetModeDropdown;
    [SerializeField] private TMP_InputField customTilesetInput;
    [SerializeField] private GameObject customTilesetPanel;
    [SerializeField] private Button generateButton;
    [SerializeField] private Button randomSeedButton;
    [SerializeField] private TextMeshProUGUI currentSeedText;

    [Header("Map Rendering")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform mapContainer;
    [SerializeField] private float tileSize = 1f;

    [Header("Default Tileset")]
    [SerializeField] private List<TileConfig> defaultTiles;

    private string[,] currentMap;
    private Dictionary<string, Color> tileColorMap;
    private List<GameObject> activeTiles = new List<GameObject>();

    private void Start()
    {
        // Setup default tiles if not configured
        if (defaultTiles == null || defaultTiles.Count == 0)
        {
            SetupDefaultTiles();
        }

        // Setup UI listeners
        generateButton.onClick.AddListener(GenerateMap);
        randomSeedButton.onClick.AddListener(GenerateRandomSeed);
        tilesetModeDropdown.onValueChanged.AddListener(OnTilesetModeChanged);

        // Set default values
        widthInputField.text = "20";
        heightInputField.text = "20";

        // Generate initial map
        GenerateMap();
    }

    private void SetupDefaultTiles()
    {
        defaultTiles = new List<TileConfig>();

        // Blue tile
        TileConfig blue = new TileConfig
        {
            tileName = "blue",
            tileColor = new Color(0.2f, 0.6f, 0.86f), // #3498db
            allowedNeighbors = new List<string> { "blue", "yellow" }
        };

        // Green tile
        TileConfig green = new TileConfig
        {
            tileName = "green",
            tileColor = new Color(0.18f, 0.8f, 0.44f), // #2ecc71
            allowedNeighbors = new List<string> { "green", "yellow" }
        };

        // Yellow tile
        TileConfig yellow = new TileConfig
        {
            tileName = "yellow",
            tileColor = new Color(0.95f, 0.77f, 0.06f), // #f1c40f
            allowedNeighbors = new List<string> { "blue", "green", "yellow" }
        };

        defaultTiles.Add(blue);
        defaultTiles.Add(green);
        defaultTiles.Add(yellow);
    }

    private void OnTilesetModeChanged(int value)
    {
        customTilesetPanel.SetActive(value == 1);
    }

    private void GenerateRandomSeed()
    {
        int seed = UnityEngine.Random.Range(0, 1000000);
        seedInputField.text = seed.ToString();
    }

    private void GenerateMap()
    {
        // Parse inputs
        int? seed = null;
        if (!string.IsNullOrEmpty(seedInputField.text))
        {
            if (int.TryParse(seedInputField.text, out int parsedSeed))
            {
                seed = parsedSeed;
            }
        }

        if (!int.TryParse(widthInputField.text, out int width))
        {
            width = 20;
        }

        if (!int.TryParse(heightInputField.text, out int height))
        {
            height = 20;
        }

        // Validate dimensions
        width = Mathf.Clamp(width, 5, 100);
        height = Mathf.Clamp(height, 5, 100);

        // Get tiles and rules
        string[] tiles;
        Dictionary<string, List<string>> rules;

        if (tilesetModeDropdown.value == 1)
        {
            // Custom tileset
            if (!ParseCustomTileset(out tiles, out rules, out tileColorMap))
            {
                Debug.LogError("Failed to parse custom tileset");
                return;
            }
        }
        else
        {
            // Default tileset
            tiles = new string[defaultTiles.Count];
            rules = new Dictionary<string, List<string>>();
            tileColorMap = new Dictionary<string, Color>();

            for (int i = 0; i < defaultTiles.Count; i++)
            {
                tiles[i] = defaultTiles[i].tileName;
                rules[defaultTiles[i].tileName] = defaultTiles[i].allowedNeighbors;
                tileColorMap[defaultTiles[i].tileName] = defaultTiles[i].tileColor;
            }
        }

        // Run WFC algorithm
        WaveFunctionCollapse wfc = new WaveFunctionCollapse(width, height, tiles, rules, seed);
        bool success = wfc.Run();

        if (!success)
        {
            Debug.LogError("Failed to generate map. Try a different seed or configuration.");
            return;
        }

        currentMap = wfc.GetResult();
        int actualSeed = wfc.GetSeed();

        // Update UI
        currentSeedText.text = $"Current Seed: {actualSeed}";
        if (string.IsNullOrEmpty(seedInputField.text))
        {
            seedInputField.text = actualSeed.ToString();
        }

        // Render the map
        RenderMap(currentMap);
    }

    private bool ParseCustomTileset(out string[] tiles, out Dictionary<string, List<string>> rules, out Dictionary<string, Color> colorMap)
    {
        tiles = null;
        rules = null;
        colorMap = null;

        try
        {
            CustomTileset tileset = JsonUtility.FromJson<CustomTileset>(customTilesetInput.text);

            if (tileset == null || tileset.tiles == null || tileset.tiles.Count == 0)
            {
                Debug.LogError("Invalid tileset configuration");
                return false;
            }

            tiles = new string[tileset.tiles.Count];
            rules = new Dictionary<string, List<string>>();
            colorMap = new Dictionary<string, Color>();

            for (int i = 0; i < tileset.tiles.Count; i++)
            {
                tiles[i] = tileset.tiles[i].tileName;
                rules[tileset.tiles[i].tileName] = tileset.tiles[i].allowedNeighbors;
                colorMap[tileset.tiles[i].tileName] = tileset.tiles[i].tileColor;
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse custom tileset: {e.Message}");
            return false;
        }
    }

    private void RenderMap(string[,] map)
    {
        // Clear existing tiles
        foreach (GameObject tile in activeTiles)
        {
            Destroy(tile);
        }
        activeTiles.Clear();

        int height = map.GetLength(0);
        int width = map.GetLength(1);

        // Center the map
        float offsetX = -(width * tileSize) / 2f;
        float offsetY = -(height * tileSize) / 2f;

        // Create tiles
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                string tileName = map[y, x];
                if (string.IsNullOrEmpty(tileName)) continue;

                Vector3 position = new Vector3(
                    offsetX + x * tileSize + tileSize / 2f,
                    offsetY + (height - 1 - y) * tileSize + tileSize / 2f,
                    0
                );

                GameObject tileObj;
                if (tilePrefab != null)
                {
                    tileObj = Instantiate(tilePrefab, position, Quaternion.identity, mapContainer);
                }
                else
                {
                    tileObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tileObj.transform.position = position;
                    tileObj.transform.localScale = new Vector3(tileSize * 0.95f, tileSize * 0.95f, 1);
                    tileObj.transform.parent = mapContainer;
                }

                // Set color
                if (tileColorMap.ContainsKey(tileName))
                {
                    Renderer renderer = tileObj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = tileColorMap[tileName];
                    }
                }

                activeTiles.Add(tileObj);
            }
        }

        // Adjust camera to fit map
        AdjustCamera(width, height);
    }

    private void AdjustCamera(int width, int height)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float mapWidth = width * tileSize;
            float mapHeight = height * tileSize;

            // Calculate required orthographic size
            float verticalSize = mapHeight / 2f + 2f;
            float horizontalSize = (mapWidth / 2f + 2f) / mainCamera.aspect;

            mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}
