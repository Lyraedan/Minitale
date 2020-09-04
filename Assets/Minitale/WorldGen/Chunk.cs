using Minitale.Utils;
using Minitale.WorldGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.WorldGen
{
    public class Chunk : MonoBehaviour
    {

        public enum Biome {

            PLAINS, TUNDRA, DESERT, FOREST, OCEAN

        }

        public Biome biome = Biome.PLAINS;
        public static int chunkWidth = 20;
        public static int chunkHeight = 20;
        public TileList tiles;
        public GameObject tile;

        private Dictionary<string, TileData> tileCache = new Dictionary<string, TileData>();

        /// <summary>
        /// Generate the chunks tiles!
        /// </summary>
        public void GenerateChunk()
        {
            for(int x = 0; x < chunkWidth; x++)
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
            Smooth();
        }

        public void Smooth()
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    /*
                    TileData tileAt = tileCache[$"Tile_{x}_{z}"];

                    int xNegative = MathHelper.Clamp(x - 1, 0, chunkWidth);
                    int zNegative = MathHelper.Clamp(z - 1, 0, chunkHeight);

                    int xPositive = MathHelper.Clamp(x + 1, 0, chunkWidth);
                    int zPositive = MathHelper.Clamp(z + 1, 0, chunkHeight);

                    TileData tile = tileCache[$"Tile_{x}_{z}"];

                    TileData tileUp = tileCache[$"Tile_{x}_{zPositive}"];
                    TileData tileDown = tileCache[$"Tile_{x}_{zNegative}"];

                    TileData tileLeft = tileCache[$"Tile_{xNegative}_{z}"];
                    TileData tileRight = tileCache[$"Tile_{xPositive}_{z}"];

                    if (tileUp.tile == tile.tile) UpdateTileAt(zPositive, z, tile.tile);
                    if (tileDown.tile == tile.tile) UpdateTileAt(zNegative, z, tile.tile);

                    if (tileLeft.tile == tile.tile) UpdateTileAt(xNegative, z, tile.tile);
                    if (tileRight.tile == tile.tile) UpdateTileAt(xPositive, z, tile.tile);
                    */

                    int neighbourTiles = GetSurroundingTiles(x, z);

                    if(neighbourTiles > 4)
                    {
                        UpdateTileAt(x, z, 1);
                    } else if(neighbourTiles < 4)
                    {
                        UpdateTileAt(x, z, 0);
                    }
                }
            }
        }

        int GetSurroundingTiles(int gridX, int gridZ)
        {
            /*
            int wallCount = 0;
            for(int x = gridX - 1; x <= gridX + 1; x++)
            {
                for(int z = gridZ - 1; gridZ <= gridZ + 1; z++)
                {
                    if(x >= 0 && x < chunkWidth && z >= 0 && z < chunkHeight)
                    {
                        if(x != gridX || z != gridZ)
                        {
                            wallCount += 0;//tileCache[$"Tile_{x}_{z}"].tile;
                        }
                    } else
                    {
                        wallCount++;
                    }
                }
            }
            */
            int wallCount = 0;

            TileData tileAt = tileCache[$"Tile_{gridX}_{gridZ}"];

            int xNegative = MathHelper.Clamp(gridX - 1, 0, chunkWidth);
            int zNegative = MathHelper.Clamp(gridZ - 1, 0, chunkHeight);

            int xPositive = MathHelper.Clamp(gridX + 1, 0, chunkWidth);
            int zPositive = MathHelper.Clamp(gridZ + 1, 0, chunkHeight);

            TileData tile = tileCache[$"Tile_{gridX}_{gridZ}"];

            TileData tileUp = tileCache[$"Tile_{gridX}_{zPositive}"];
            TileData tileDown = tileCache[$"Tile_{gridX}_{zNegative}"];

            TileData tileLeft = tileCache[$"Tile_{xNegative}_{gridZ}"];
            TileData tileRight = tileCache[$"Tile_{xPositive}_{gridZ}"];

            if (tileUp.tile != tile.tile) wallCount++;
            if (tileDown.tile != tile.tile) wallCount++;

            if (tileLeft.tile != tile.tile) wallCount++;
            if (tileRight.tile != tile.tile) wallCount++;
            

            return wallCount;
        }

        public void UpdateTileAt(float x, float z, int next)
        {
            TileData tileAt = tileCache[$"Tile_{x}_{z}"];
            tileAt.tile = next;
            tileAt.renderer.material.mainTexture = tiles.tiles[next].texture;
        }
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