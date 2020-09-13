using Minitale.Utils;
using Minitale.WorldGen;
using Mirror;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Minitale.WorldGen
{
    public class Chunk : NetworkBehaviour
    {

        //Swapping out biome enum for biome grid
        public static int chunkWidth = 16; // 100 is really detailed but really laggy
        public static int chunkHeight = 16; // 100 is really detailed but relly laggy
        public TileList tiles;
        public float scale = 0.1f;

        [Header("Foilage")]
        public GameObject tree;
        public GameObject grass;

        [Header("Networking")]
        public GameObject playerSpawn;

        private Dictionary<string, NavMeshSurface> navMesh = new Dictionary<string, NavMeshSurface>();
        private Dictionary<string, TileData> tileCache = new Dictionary<string, TileData>();

        /// <summary>
        /// Generate the chunks tiles!
        /// </summary>
        [ServerCallback]
        public void GenerateChunk(int seed)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    string key = $"Tile_{x}_{z}";
                    int chosen = Random.Range(0, tiles.tiles.Length);
                    Tile t = tiles.tiles[chosen];
                    Vector3 spawn = new Vector3(transform.position.x + (x * WorldGenerator.PLANE_SCALE), transform.position.y, transform.position.z + (z * WorldGenerator.PLANE_SCALE));
                    GameObject tile = Instantiate(t.prefab, spawn, Quaternion.identity);
                    tile.name = key;
                    tile.transform.SetParent(transform);

                    if (t.animated)
                    {
                        AnimationPlayer animator = tile.AddComponent<AnimationPlayer>();
                        animator.frames = t.frames;
                        animator.delay = t.animationSkip;
                        animator.randomiseStartingIndex = t.randomIndex;
                        animator.Init();
                    }
                    NavMeshSurface navmesh = null;
                    if (t.navmeshEnabled)
                    {
                        navmesh = Utils.Utils.AddComponent(tile, t.navSurface.GetComponent<NavMeshSurface>());
                        navMesh.Add(key, navmesh);
                    }
                    //Setup tile from tilelist
                    Renderer tileRenderer = tile.GetComponent<Renderer>();
                    tileRenderer.material.mainTexture = t.texture;

                    TileData data = tile.AddComponent<TileData>();
                    data.tile = chosen;
                    data.renderer = tileRenderer;
                    data.prefab = t.prefab;
                    data.position = spawn;
                    data.worldObject = tile;
                    data.key = key;
                    data.navmesh = navmesh;

                    tileCache.Add(key, data);
                    NetworkServer.Spawn(tile);
                }
            }
            ApplyBiome();
            Smooth(seed);
            RenderChunk(false);
        }

        public void RenderChunk(bool state)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    //GetTileAt(x, z).renderer.enabled = state;
                    GetTileAt(x, z).worldObject.SetActive(state);
                }
            }
        }

        /// <summary>
        /// Mark a section of the world with a biome
        /// </summary>
        public void ApplyBiome()
        {

        }

        public void PlantFoliage(int x, int z, int seed)
        {
            float perlinX = (transform.position.x + (x * WorldGenerator.PLANE_SCALE)) / chunkWidth * scale;
            float perlinZ = (transform.position.z + (z * WorldGenerator.PLANE_SCALE)) / chunkHeight * scale;
            float perlin = Mathf.PerlinNoise(perlinX + seed, perlinZ + seed);
            //Debug.Log($"Foliage perlin: {perlin}");

            if (perlin > .2f && perlin <= .4f) AddFoliage(this.tree, x, z, 0, "Tree");
            else if (perlin > .4 && perlin < .45f) AddFoliage(this.grass, x, z, 0, "Grass");
        }

        void AddFoliage(GameObject prefab, int x, int z, int plantOnID, string foliageName)
        {
            TileData tile = GetTileAt(x, z);
            if (tile.tile == plantOnID)
            {
                GameObject foliage = Instantiate(prefab, tile.worldObject.transform.position, Quaternion.identity);
                foliage.name = $"Foilage_{foliageName}";
                foliage.transform.SetParent(tile.worldObject.transform);
                NetworkServer.Spawn(foliage);
            }
        }

        /// <summary>
        /// Smooth out the chunks to produce good looking terrain
        /// </summary>
        /// <param name="seed"></param>
        public void Smooth(int seed)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    float perlinX = (transform.position.x + (x * WorldGenerator.PLANE_SCALE)) / chunkWidth * scale;
                    float perlinZ = (transform.position.z + (z * WorldGenerator.PLANE_SCALE)) / chunkHeight * scale;
                    float perlin = SimplexNoise.SimplexNoise.Generate(perlinX + seed, perlinZ + seed);
                    //Debug.Log("Noise: " + perlin);

                    //             .25f
                    if (perlin <= -.5f) UpdateWorldPrefabs(x, z, 4); //Deep water
                    else if (perlin > -.5f && perlin <= 0f) UpdateWorldPrefabs(x, z, 1); //Water
                    else if (perlin > 0 && perlin <= .25f) UpdateWorldPrefabs(x, z, 2); // Sand
                    else if (perlin > .25f && perlin <= .6f) UpdateWorldPrefabs(x, z, 0); // Grass
                    else if (perlin > .6f) UpdateWorldPrefabs(x, z, 3, 5f); // Stone
                    PlantFoliage(x, z, seed);
                }
            }
        }

        /// <summary>
        /// Networking - Place Mirror spawn points for players
        /// </summary>
        public void PlaceSpawns()
        {
            List<Transform> spawns = new List<Transform>();
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    bool placeSpawn = Random.value > 0.7f;
                    if (placeSpawn)
                    {
                        TileData tile = GetTileAt(x, z);
                        if (tile.tile == 0) // Grass
                        {
                            if (tile.worldObject.transform.childCount < 1)
                            {
                                // This tile is clear add a spawn
                                Vector3 point = new Vector3(tile.worldObject.transform.position.x, tile.worldObject.transform.position.y + 2.5f, tile.worldObject.transform.position.z);
                                GameObject spawn = Instantiate(playerSpawn, point, Quaternion.identity);
                                spawn.name = "Player_Spawn";
                                spawn.transform.SetParent(tile.worldObject.transform);
                                spawns.Add(tile.worldObject.transform);
                            }
                        }
                    }
                }
            }

            // No spawns were made?!
            if (spawns.Count < 1)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkHeight; z++)
                    {
                        TileData tile = GetTileAt(x, z);
                        if (tile.tile == 0) // grass
                        {
                            if (tile.worldObject.transform.childCount < 1)
                            {
                                Vector3 point = new Vector3(tile.worldObject.transform.position.x, tile.worldObject.transform.position.y + 5, tile.worldObject.transform.position.z);
                                GameObject spawn = Instantiate(playerSpawn, point, Quaternion.identity);
                                spawn.name = "Player_Spawn";
                                spawn.transform.SetParent(tile.worldObject.transform);
                                spawns.Add(tile.worldObject.transform);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replace a tile with another tile in world space
        /// </summary>
        /// <param name="x">Tile X coordinate</param>
        /// <param name="z">Tile Z coordinate</param>
        /// <param name="next">The ID of the next tile</param>
        /// <param name="yOffset">the Y offset</param>
        [ServerCallback]
        public void UpdateWorldPrefabs(float x, float z, int next, float yOffset = 0f)
        {
            TileData tileAt = GetTileAt(x, z);
            Tile t = tiles.tiles[next];
            GameObject spawn = t.prefab;
            navMesh.Remove(tileAt.key);
            NetworkServer.Destroy(tileAt.worldObject);
            Destroy(tileAt.worldObject);
            Vector3 placeAt = new Vector3(tileAt.position.x, tileAt.position.y + yOffset, tileAt.position.z);
            GameObject tile = Instantiate(spawn, placeAt, Quaternion.identity);
            tile.name = tileAt.key;
            tile.transform.SetParent(transform);
            if (t.animated)
            {
                AnimationPlayer animator = tile.AddComponent<AnimationPlayer>();
                animator.frames = t.frames;
                animator.delay = t.animationSkip;
                animator.randomiseStartingIndex = t.randomIndex;
                animator.Init();
            }
            if (t.navmeshEnabled)
            {
                NavMeshSurface navmesh = Utils.Utils.AddComponent(tile, t.navSurface.GetComponent<NavMeshSurface>());
                tileAt.navmesh = navmesh;
                navMesh.Add(tileAt.key, navmesh);
            }
            tileAt.tile = next;
            tileAt.worldObject = tile;
            tileAt.renderer = tile.GetComponent<Renderer>();
            tileAt.renderer.material.mainTexture = t.texture;
            NetworkServer.Spawn(tile);
        }

        /// <summary>
        /// User for updating tiles where prefab updates are not required
        /// </summary>
        /// <param name="x">Tile coord X</param>
        /// <param name="z">Tile coord Z</param>
        /// <param name="next">The ID of the tile you wish to replace the current one with </param>
        public void UpdateTileAt(float x, float z, int next)
        {
            TileData tileAt = GetTileAt(x, z);
            tileAt.tile = next;
            tileAt.renderer.material.mainTexture = tiles.tiles[next].texture;
        }

        public TileData GetTileAt(float x, float z)
        {
            return tileCache[$"Tile_{x}_{z}"];
        }

    }

    public class TileData : MonoBehaviour
    {

        public GameObject worldObject;
        public GameObject prefab;
        public Vector3 position;
        public int tile;
        public new Renderer renderer;
        public string key;
        public NavMeshSurface navmesh;

        public string ToJSON()
        {
            return JsonUtility.ToJson(this);
        }
    }
}