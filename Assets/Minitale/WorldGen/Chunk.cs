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

                    //Setup tile from tilelist
                    Renderer tileRenderer = tile.GetComponent<Renderer>();
                    tileRenderer.material.mainTexture = t.texture;

                    tileCache.Add(key, new TileData().SetTile(chosen).SetRenderer(tileRenderer).SetPrefab(t.prefab).SetPosition(spawn).SetWorldObject(tile).SetKey(key));
                }
            }
            ApplyBiome();
            Smooth(seed);
            BakeNav();
            RenderChunk(false);
        }

        public void RenderChunk(bool state)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    GetTileAt(x, z).renderer.enabled = state;
                }
            }
        }

        /// <summary>
        /// Mark a section of the world with a biome
        /// </summary>
        public void ApplyBiome()
        {

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

                    if (perlin <= -.25f)
                    {
                        UpdateTileAt(x, z, 4);
                        UpdateWorldPrefabs(x, z, 4);
                    }
                    else if (perlin > -.25f && perlin <= 0f)
                    {
                        UpdateTileAt(x, z, 1); // Water
                        UpdateWorldPrefabs(x, z, 1);
                    }
                    else if (perlin > 0 && perlin <= .25f)
                    {
                        UpdateTileAt(x, z, 2); // Sand
                        UpdateWorldPrefabs(x, z, 2);
                    }
                    else if (perlin > .25f && perlin <= .6f)
                    {
                        UpdateTileAt(x, z, 0); // Grass
                        UpdateWorldPrefabs(x, z, 0);
                    }
                    else if (perlin > .6f)
                    {
                        UpdateTileAt(x, z, 3); // Stone
                        UpdateWorldPrefabs(x, z, 3);
                    }
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
            GameObject spawn = tiles.tiles[next].prefab;
            Destroy(tileAt.worldObject);
            GameObject tile = Instantiate(spawn, tileAt.position, Quaternion.identity);
            tile.name = tileAt.key;
            tile.transform.SetParent(transform);
            tileAt.worldObject = tile;
            tileAt.renderer = tile.GetComponent<Renderer>();
            tileAt.renderer.material.mainTexture = tiles.tiles[next].texture;
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