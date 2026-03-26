using System;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaveDataGenerator : MonoBehaviour
{
    // The types of blocks in our game
    public enum BlockType { Air, Rock, Dirt, Ore, Slate, Door }

    [Header("Generation Settings")]
    public int baseSize = 50;               // Grid size
    int size;                               // Actual size based on current day
    public int baseDepthLevels = 3;         // How many floors down we go
    int depth;                              // Actual depth based on current day
    public int floorHeight = 5;             // 4 blocks for the drop + 1 for the floor itself
    int walkIterations;                     // How long the drunken walk lasts per floor
    public float growthMultiplier = 1.5f;   // Tweak to make the late-game more or less extreme

    [Header("Block Prefabs")]
    public GameObject rockPrefab;
    public GameObject dirtPrefab;
    public GameObject stygianSlatePrefab;
    public GameObject[] mineralPrefabs;
    public GameObject stairPrefab;
    public GameObject doorPrefab;

    [Header("Organization")]
    public Transform environmentContainer; // Assign an empty GameObject here to keep your hierarchy clean

    // The Master Grid holding all our block data
    public BlockType[,,] grid;

    // The center of each level
    private Vector2[] levelCenters;

    private void Awake()
    {
        GameManager.Instance.SetGenerator(this);
    }

    public Vector3 GenerateLevel(int currentDay)
    {   
        // Exponential scaling: baseSize + ( (Day - 1)^2 * multiplier )
        // Day 1 = baseSize. Day 2 = baseSize + 1^2 * multiplier. Day 3 = baseSize + 2^2 * multiplier, etc.
        
        size = baseSize + Mathf.RoundToInt(Mathf.Pow(GameManager.Instance.CurrentDay - 1, 2) * growthMultiplier);

        // Adds 1 new floor every 3 days.
        // Day 1-3 = baseDepth. Day 4-6 = baseDepth + 1. Day 7-9 = baseDepth + 2, etc.
        depth = baseDepthLevels + ((GameManager.Instance.CurrentDay - 1) / 3);

        // A density of 0.2f means 20% of the solid rock per floor gets carved into playable halls.
        // Day 1 = 500 iterations per floor. Day 2 = 625. Day 3 = 900, etc.
        float caveDensity = 0.2f; 
        walkIterations = Mathf.RoundToInt((size * size) * caveDensity);

        int totalHeight = depth * floorHeight;
        grid = new BlockType[size, totalHeight, size];
        levelCenters = new Vector2[depth]; 

        // 1. Initialize the World
        for (int x = 0; x < size; x++)
            for (int y = 0; y < totalHeight; y++)
                for (int z = 0; z < size; z++)
                    grid[x, y, z] = BlockType.Rock;

        // 2 & 3. The Drunken Walk & Stairs
        Vector3Int currentPos = new Vector3Int(size / 2, totalHeight - 2, size / 2); 
        
        for (int level = 0; level < depth; level++)
        {
            levelCenters[level] = new Vector2(currentPos.x, currentPos.z);
            currentPos = RunDrunkenWalk(currentPos, walkIterations);
            
            if (level < depth - 1)
                currentPos = CarveShaft(currentPos, floorHeight);
        }

        // 4. Smooth Cellular Automata (Create biomes)
        GenerateDirtBlobs();

        // 5. Spawn Ores based on the new moving centers
        SpawnOres(currentDay);

        // 6. Wrap the exact layout in a 2-block limit of Stygian Slate
        EncaseInStygianSlate();

        // 7. Force the absolute outer edges to be Bedrock to fix the void bug
        ForceOuterShell();

        // 8. Carve the starting room, set the door, and set the player's position
        var playerPos = CreateEntrance();
        
        // 9. Spawn the GameObjects
        SpawnBlocks();

        Debug.Log($"Day {currentDay} Data Generated!");
        return playerPos;
    }

    private Vector3Int RunDrunkenWalk(Vector3Int startPos, int iterations)
    {
        Vector3Int pos = startPos;
        Vector3Int[] directions = { Vector3Int.right, Vector3Int.left, Vector3Int.forward, Vector3Int.back };

        Vector3Int furthestPos = startPos;
        float maxDist = 0;

        for (int i = 0; i < iterations; i++)
        {
            if (IsInBounds(pos.x, pos.y, pos.z)) grid[pos.x, pos.y, pos.z] = BlockType.Air;
            if (IsInBounds(pos.x, pos.y - 1, pos.z)) grid[pos.x, pos.y - 1, pos.z] = BlockType.Air;
            if (IsInBounds(pos.x, pos.y - 2, pos.z)) grid[pos.x, pos.y - 2, pos.z] = BlockType.Air;

            pos += directions[Random.Range(0, directions.Length)];
            pos.x = Mathf.Clamp(pos.x, 2, size - 3);
            pos.z = Mathf.Clamp(pos.z, 2, size - 3);

            // Track the furthest point to place the stairs
            float dist = Vector3.Distance(startPos, pos);
            if (dist > maxDist)
            {
                maxDist = dist;
                furthestPos = pos;
            }
        }
        
        return furthestPos; // Return the end of the line
    }

    private Vector3Int CarveShaft(Vector3Int topPos, int dropDistance)
    {
        Vector3Int current = topPos;
        for (int i = 0; i < dropDistance; i++)
        {
            current.y -= 1; // Move down one block
            if (IsInBounds(current.x, current.y, current.z))
            {
                grid[current.x, current.y, current.z] = BlockType.Air;
                
                // Widen the shaft by 1 block so the player can easily fall in
                if (IsInBounds(current.x + 1, current.y, current.z)) 
                    grid[current.x + 1, current.y, current.z] = BlockType.Air;
            }
        }
        return current; // Return the new starting position for the next floor
    }

    private void SpawnOres(int currentDay)
    {
        int totalHeight = grid.GetLength(1);

        for (int x = 1; x < size - 1; x++)
        {
            for (int y = 1; y < totalHeight - 1; y++)
            {
                for (int z = 1; z < size - 1; z++)
                {
                    if (grid[x, y, z] == BlockType.Rock || grid[x, y, z] == BlockType.Dirt)
                    {
                        // Figure out which floor we are currently on to get the right center
                        int floorIndex = (totalHeight - 1 - y) / floorHeight;
                        floorIndex = Mathf.Clamp(floorIndex, 0, depth - 1);
                        Vector2 currentCenter = levelCenters[floorIndex];

                        float distFromCenter = Vector2.Distance(new Vector2(x, z), currentCenter);
                        float depthMultiplier = 1f + ((totalHeight - y) * 0.1f); 
                        float oreChance = (0.005f + (distFromCenter * 0.002f) + (currentDay * 0.005f)) * depthMultiplier;

                        if (Random.value < oreChance)
                        {
                            grid[x, y, z] = BlockType.Ore;
                        }
                    }
                }
            }
        }
    }

    private void EncaseInStygianSlate()
    {
        int totalHeight = grid.GetLength(1);
        BlockType[,,] nextGrid = (BlockType[,,])grid.Clone();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (grid[x, y, z] == BlockType.Air) continue;

                    bool isNearAirHorizontal = false;
                    bool isFloorOrCeiling = false;

                    // Check if it's within 2 blocks horizontally of a hallway
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dz = -2; dz <= 2; dz++)
                        {
                            if (IsInBounds(x + dx, y, z + dz) && grid[x + dx, y, z + dz] == BlockType.Air)
                            {
                                isNearAirHorizontal = true; 
                                break;
                            }
                        }
                        if (isNearAirHorizontal) break;
                    }

                    // Check if it is strictly a floor or ceiling (1 block up or down from Air)
                    if (IsInBounds(x, y + 1, z) && grid[x, y + 1, z] == BlockType.Air) isFloorOrCeiling = true;
                    if (IsInBounds(x, y - 1, z) && grid[x, y - 1, z] == BlockType.Air) isFloorOrCeiling = true;

                    // Apply the Slate limits
                    if (!isNearAirHorizontal || isFloorOrCeiling) 
                    {
                        nextGrid[x, y, z] = BlockType.Slate;
                    }
                }
            }
        }
        grid = nextGrid;
    }

    public void SpawnBlocks()
    {
        // Destroy old blocks if we are generating a new day
        foreach (Transform child in environmentContainer)
        {
            Destroy(child.gameObject);
        }

        int totalHeight = grid.GetLength(1);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    BlockType currentType = grid[x, y, z];

                    // -- CULLING LOGIC --
                    // Don't spawn anything for Air
                    if (currentType == BlockType.Air) continue;

                    if (currentType == BlockType.Slate)
                    {
                        // If it ONLY touches other Slate, cull it to save performance
                        if (!HasNonSlateNeighbor(x, y, z)) continue;
                    }
                    // Notice we don't cull Rock, Dirt, or Ore at all. 
                    // Since the walls are only 2 blocks deep, we just spawn all of em

                    // We need to cull faces that aren't shown to the camera but I think Unity can do that on its own,
                    // will look into it down the line if performance starts to tank

                    // -- SPAWNING LOGIC --

                    GameObject prefabToSpawn = null;

                    // Figure out which prefab to use
                    switch (currentType)
                    {
                        case BlockType.Rock:
                            prefabToSpawn = rockPrefab;
                            break;
                        case BlockType.Dirt:
                            prefabToSpawn = dirtPrefab;
                            break;
                        case BlockType.Slate:
                            prefabToSpawn = stygianSlatePrefab;
                            break;
                        case BlockType.Door:
                            prefabToSpawn = doorPrefab;
                            break;
                        case BlockType.Ore:
                            if (mineralPrefabs.Length > 0)
                            {
                                int randomIndex = Random.Range(0, mineralPrefabs.Length);
                                prefabToSpawn = mineralPrefabs[randomIndex];
                            }
                            break;
                    }

                    // Spawn the block and parent it to the container
                    if (prefabToSpawn != null)
                    {
                        Vector3 spawnPosition = new Vector3(x, y, z);
                        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, environmentContainer);
                    }
                }
            }
        }
        
        Debug.Log("Prefabs successfully spawned!");
    }

    private Vector3 CreateEntrance()
    {
        int totalHeight = grid.GetLength(1);
        
        // The exact coordinate where Level 1's Drunken Walk started
        int startX = size / 2;
        int startY = totalHeight - 2;
        int startZ = size / 2;

        // 1. Clear a 3x3 safe starting room so the player is never trapped
        for (int x = startX - 1; x <= startX + 1; x++)
        {
            // y-2 is the floor of the hallway, y is the ceiling
            for (int y = startY - 2; y <= startY; y++) 
            {
                for (int z = startZ - 1; z <= startZ + 1; z++)
                {
                    if (IsInBounds(x, y, z)) grid[x, y, z] = BlockType.Air;
                }
            }
        }

        // 2. Place the Door block on the wall behind the player (z - 2)
        if (IsInBounds(startX, startY - 2, startZ - 2))
        {
            grid[startX, startY - 2, startZ - 2] = BlockType.Door;
            
            // Back the door with Bedrock so there is no void behind it
            grid[startX, startY - 2, startZ - 3] = BlockType.Slate;
            grid[startX, startY - 1, startZ - 3] = BlockType.Slate;
            grid[startX, startY, startZ - 3] = BlockType.Slate;
        }

        return new Vector3(startX, startY - 3f, startZ);
    }

    // --- Helper Functions ---

    private bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < size && y >= 0 && y < grid.GetLength(1) && z >= 0 && z < size;
    }

    private bool HasAdjacentAir(int x, int y, int z)
    {
        // Simple cross-check (up, down, left, right, forward, back)
        if (IsInBounds(x + 1, y, z) && grid[x + 1, y, z] == BlockType.Air) return true;
        if (IsInBounds(x - 1, y, z) && grid[x - 1, y, z] == BlockType.Air) return true;
        if (IsInBounds(x, y + 1, z) && grid[x, y + 1, z] == BlockType.Air) return true;
        if (IsInBounds(x, y - 1, z) && grid[x, y - 1, z] == BlockType.Air) return true;
        if (IsInBounds(x, y, z + 1) && grid[x, y, z + 1] == BlockType.Air) return true;
        if (IsInBounds(x, y, z - 1) && grid[x, y, z - 1] == BlockType.Air) return true;
        return false;
    }

    private bool HasNonSlateNeighbor(int x, int y, int z)
    {
        // Check all 6 cardinal directions for anything that ISN'T Slate
        if (IsInBounds(x + 1, y, z) && grid[x + 1, y, z] != BlockType.Slate) return true;
        if (IsInBounds(x - 1, y, z) && grid[x - 1, y, z] != BlockType.Slate) return true;
        if (IsInBounds(x, y + 1, z) && grid[x, y + 1, z] != BlockType.Slate) return true;
        if (IsInBounds(x, y - 1, z) && grid[x, y - 1, z] != BlockType.Slate) return true;
        if (IsInBounds(x, y, z + 1) && grid[x, y, z + 1] != BlockType.Slate) return true;
        if (IsInBounds(x, y, z - 1) && grid[x, y, z - 1] != BlockType.Slate) return true;
        
        return false; // Everything around it is Slate
    }

    private void GenerateDirtBlobs()
    {
        int totalHeight = grid.GetLength(1);

        // Pass 1: Random Noise (Only affect Rock)
        for (int x = 1; x < size - 1; x++)
        {
            for (int y = 1; y < totalHeight - 1; y++)
            {
                for (int z = 1; z < size - 1; z++)
                {
                    if (grid[x, y, z] == BlockType.Rock)
                    {
                        // 45% chance to start as dirt
                        if (Random.value < 0.45f) grid[x, y, z] = BlockType.Dirt;
                    }
                }
            }
        }

        // Pass 2: Smooth it out (Run 3 times for nice, chunky blobs)
        for (int i = 0; i < 3; i++)
        {
            BlockType[,,] nextGrid = (BlockType[,,])grid.Clone();

            for (int x = 1; x < size - 1; x++)
            {
                for (int y = 1; y < totalHeight - 1; y++)
                {
                    for (int z = 1; z < size - 1; z++)
                    {
                        if (grid[x, y, z] == BlockType.Rock || grid[x, y, z] == BlockType.Dirt)
                        {
                            int dirtNeighbors = GetNeighborCount(x, y, z, BlockType.Dirt);
                            int rockNeighbors = GetNeighborCount(x, y, z, BlockType.Rock);

                            // CA Rule: Majority rules
                            if (dirtNeighbors > rockNeighbors)
                                nextGrid[x, y, z] = BlockType.Dirt;
                            else if (rockNeighbors > dirtNeighbors)
                                nextGrid[x, y, z] = BlockType.Rock;
                        }
                    }
                }
            }
            grid = nextGrid; // Apply the smoothed grid
        }
    }

    private int GetNeighborCount(int cx, int cy, int cz, BlockType type)
    {
        int count = 0;
        // Check a 3x3x3 cube around the target block
        for (int x = cx - 1; x <= cx + 1; x++)
        {
            for (int y = cy - 1; y <= cy + 1; y++)
            {
                for (int z = cz - 1; z <= cz + 1; z++)
                {
                    if (x == cx && y == cy && z == cz) continue; // Skip the center block
                    if (IsInBounds(x, y, z) && grid[x, y, z] == type)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    private void ForceOuterShell()
    {
        int totalHeight = grid.GetLength(1);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    // If it is on the absolute outer boundary of the array, make it Stygian Slate
                    if (x == 0 || x == size - 1 || y == 0 || y == totalHeight - 1 || z == 0 || z == size - 1)
                    {
                        grid[x, y, z] = BlockType.Slate;
                    }
                }
            }
        }
    }
}