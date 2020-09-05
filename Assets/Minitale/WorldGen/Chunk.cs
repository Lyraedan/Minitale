using Minitale.Utils;
using Minitale.WorldGen;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.WorldGen
{
    public class Chunk : MonoBehaviour
    {

        //Swapping out biome enum for biome grid
        public static int chunkWidth = 16; // 100 is really detailed but really laggy
        public static int chunkHeight = 16; // 100 is really detailed but relly laggy
        public TileList tiles;
        public float scale = 0.1f;

        [Header("Foilage")]
        public GameObject tree;
        public GameObject grass;

        private Dictionary<string, TileData> tileCache = new Dictionary<string, TileData>();

        /// <summary>
        /// Generate the chunks tiles!
        /// </summary>
        public void GenerateChunk(int seed)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for(int z = 0; z < chunkHeight; z++)
                {
                    string key = $"Tile_{x}_{z}";
                    int chosen = Random.Range(0, tiles.tiles.Length);
                    Tile t = tiles.tiles[chosen];
                    Vector3 spawn = new Vector3(transform.position.x + (x * WorldGenerator.PLANE_SCALE), transform.position.y, transform.position.z + (z * WorldGenerator.PLANE_SCALE));
                    GameObject tile = Instantiate(t.prefab, spawn, Quaternion.identity);
                    tile.name = key;
                    tile.transform.SetParent(transform);

                    if(t.animated)
                    {
                        AnimationPlayer animator = tile.AddComponent<AnimationPlayer>();
                        animator.frames = t.frames;
                        animator.delay = t.animationSkip;
                        animator.randomiseStartingIndex = t.randomIndex;
                        animator.Init();
                    }

                    //Setup tile from tilelist
                    Renderer tileRenderer = tile.GetComponent<Renderer>();
                    tileRenderer.material.mainTexture = t.texture;

                    tileCache.Add(key, new TileData().SetTile(chosen).SetRenderer(tileRenderer).SetPrefab(t.prefab).SetPosition(spawn).SetWorldObject(tile).SetKey(key));
                }
            }
            ApplyBiome();
            Smooth(seed);
            PlantFoilage();

            BakeNav();
            RenderChunk(false);
        }

        public void RenderChunk(bool state)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    //GetTileAt(x, z).renderer.enabled = state;
                    // I don't know about this solution
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

        public void PlantFoilage()
        {
            for(int x = 0; x < chunkWidth; x++)
            {
                for(int z = 0; z < chunkHeight; z++)
                {
                    bool plantTrees = Random.value > 0.85f;
                    if(plantTrees)
                    {
                        TileData tile = GetTileAt(x, z);
                        if (tile.tile == 0) // Grass
                        {
                            GameObject tree = Instantiate(this.tree, tile.worldObject.transform.position, Quaternion.identity/*Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))*/);
                            tree.name = "Foilage_Tree";
                            tree.transform.SetParent(tile.worldObject.transform);
                        }
                    }

                    bool plantGrass = Random.value > 0.5 && Random.value <= 0.85;
                    if (plantGrass)
                    {
                        TileData tile = GetTileAt(x, z);
                        if (tile.tile == 0) // Grass
                        {
                            GameObject tree = Instantiate(this.grass, tile.worldObject.transform.position, Quaternion.identity);
                            tree.name = "Foilage_Grass";
                            tree.transform.SetParent(tile.worldObject.transform);
                        }
                    }
                }
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

                    if (perlin <= -.25f) UpdateWorldPrefabs(x, z, 4); //Deep water
                    else if (perlin > -.25f && perlin <= 0f) UpdateWorldPrefabs(x, z, 1); //Water
                    else if (perlin > 0 && perlin <= .25f) UpdateWorldPrefabs(x, z, 2); // Sand
                    else if (perlin > .25f && perlin <= .6f) UpdateWorldPrefabs(x, z, 0); // Grass
                    else if (perlin > .6f) UpdateWorldPrefabs(x, z, 3); // Stone
                }
            }
        }

        /// <summary>
        /// Bake the navmesh
        /// </summary>
        public void BakeNav()
        {

        }

        public void UpdateWorldPrefabs(float x, float z, int next)
        {
            TileData tileAt = GetTileAt(x, z);
            Tile t = tiles.tiles[next];
            GameObject spawn = t.prefab;
            Destroy(tileAt.worldObject);
            GameObject tile = Instantiate(spawn, tileAt.position, Quaternion.identity);
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
            tileAt.tile = next;
            tileAt.worldObject = tile;
            tileAt.renderer = tile.GetComponent<Renderer>();
            tileAt.renderer.material.mainTexture = t.texture;
        }

        public void UpdateTileAt(float x, float z, int next)
        {
            TileData tileAt = GetTileAt(x, z);
            tileAt.tile = next;
            tileAt.renderer.material.mainTexture = tiles.tiles[next].texture;
        }

        public TileData GetTileAt(float x, float z) {
            return tileCache[$"Tile_{x}_{z}"];
        } 

    }

    public class TileData
    {

        public GameObject worldObject;
        public GameObject prefab;
        public Vector3 position;
        public int tile;
        public Renderer renderer;
        public string key;

        public TileData SetTile(int tile)
        {
            this.tile = tile;
            return this;
        }

        public TileData SetRenderer(Renderer renderer)
        {
            this.renderer = renderer;
            return this;
        }

        public TileData SetPrefab(GameObject gameObject)
        {
            this.prefab = gameObject;
            return this;
        }

        public TileData SetPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public TileData SetWorldObject(GameObject gameObject)
        {
            this.worldObject = gameObject;
            return this;
        }

        public TileData SetKey(string key)
        {
            this.key = key;
            return this;
        }
    }
}