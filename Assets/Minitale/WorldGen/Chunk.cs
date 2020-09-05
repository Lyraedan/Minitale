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
        public static int chunkWidth = 16;
        public static int chunkHeight = 16;
        public TileList tiles;
        public GameObject tile;
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
                    string key = $"Tile_{x}_{z}"; ;
                    GameObject tile = Instantiate(this.tile, new Vector3(transform.position.x + (x * WorldGenerator.PLANE_SCALE), transform.position.y, transform.position.z + (z * WorldGenerator.PLANE_SCALE)), Quaternion.identity);
                    tile.name = key;
                    tile.transform.SetParent(transform);

                    //Setup tile from tilelist
                    Renderer tileRenderer = tile.GetComponent<Renderer>();
                    int chosen = Random.Range(0, tiles.tiles.Length);
                    tileRenderer.material.mainTexture = tiles.tiles[chosen].texture;

                    tileCache.Add(key, new TileData().SetTile(chosen).SetRenderer(tileRenderer));
                }
            }
            ApplyBiome();
            Smooth(seed);
            BakeNav();
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
                    float perlin = SimplexNoise.SimplexNoise.Generate(perlinX + seed, perlinZ + seed); //Mathf.PerlinNoise(perlinX + seed, perlinZ + seed);
                    Debug.Log("Noise: " + perlin);

                    if (perlin <= 0f) UpdateTileAt(x, z, 1); // Water
                    else if (perlin > 0 && perlin <= .25f) UpdateTileAt(x, z, 2); // Sand
                    else if (perlin > .25f && perlin <= .6f) UpdateTileAt(x, z, 0); // Grass
                    else if (perlin > .6f) UpdateTileAt(x, z, 3); // Stone
                }
            }
        }

        /// <summary>
        /// Bake the navmesh
        /// </summary>
        public void BakeNav()
        {

        }

        public void UpdateTileAt(float x, float z, int next)
        {
            TileData tileAt = tileCache[$"Tile_{x}_{z}"];
            tileAt.tile = next;
            tileAt.renderer.material.mainTexture = tiles.tiles[next].texture;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(transform.position.x + (Chunk.chunkWidth * WorldGenerator.PLANE_SCALE) / 2, transform.position.y, transform.position.z + (Chunk.chunkWidth * WorldGenerator.PLANE_SCALE) / 2), new Vector3());
        }
#endif
    }

    internal class TileData
    {

        public int tile;
        public Renderer renderer;

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
    }
}