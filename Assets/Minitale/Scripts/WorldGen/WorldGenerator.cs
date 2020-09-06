using Minitale.Utils;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minitale.WorldGen
{
    public class WorldGenerator : MonoBehaviour
    {
        public static float PLANE_SCALE = 10F;

        public GameObject chunk;
        public static Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();
        public int seed = 0, initTicks = 0;

        public static WorldGenerator generator;

        private void Awake()
        {
            generator = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            DestroyImmediate(GetComponent<MeshFilter>());
            DestroyImmediate(GetComponent<Renderer>());

            //UnityEngine.Random.InitState(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            //SimplexNoise.SimplexNoise.Seed = seed;
            //if (seed == 0) seed = (int) UnityEngine.Random.value; //SimplexNoise.SimplexNoise.Seed;
            initTicks = (int)DateTime.UtcNow.Ticks;
            Random.InitState(initTicks);
            if (seed == 0) seed = Random.Range(-100000, 100000);

            for(int x = -1; x <= 1; x++)
            {
                for(int z = -1; z <= 1; z++)
                {
                    GenerateChunkAt(x, 0f, z);
                    GetChunkAt(x, 0f, z).RenderChunk(true);
                }
            }
        }

        public void ConfigureWorld(int initTicks, int seed)
        {
            Random.InitState(initTicks);
            this.initTicks = initTicks;
            this.seed = seed;
        }

        public void GenerateChunkAt(Vector3 location)
        {
            GenerateChunkAt(location.x, location.y, location.z);
        }

        public void GenerateChunkAt(float x, float y, float z)
        {
            string key = $"Chunk_{x}_{y}_{z}";
            if (chunks.ContainsKey(key)) return;

            GameObject chunk = Instantiate(this.chunk, new Vector3(x * (PLANE_SCALE * Chunk.chunkWidth), y, z * (PLANE_SCALE * Chunk.chunkHeight)), Quaternion.identity);
            chunk.name = key;
            chunk.transform.SetParent(transform);
            chunks.Add(key, chunk);

            Chunk c = chunk.GetComponent<Chunk>();
            c.GenerateChunk(seed);
            if (x == 0 && z == 0) c.PlaceSpawns();
        }

        public static Chunk GetChunkAt(Vector3 location)
        {
            return GetChunkAt(location.x, location.y, location.z);
        }

        public static Chunk GetChunkAt(float x, float y, float z)
        {
            return GetChunkAtAsGameObject(x, y, z).GetComponent<Chunk>();
        }

        public static GameObject GetChunkAtAsGameObject(float x, float y, float z)
        {
            string key = $"Chunk_{x}_{y}_{z}";
            if (!ChunkExistsAt(x, y, z)) generator.GenerateChunkAt(x, y, z);
            return chunks[key];
        }

        public static bool ChunkExistsAt(float x, float y, float z)
        {
            return chunks.ContainsKey($"Chunk_{x}_{y}_{z}");
        }

        public void ClearChunks()
        {
            foreach(string key in chunks.Keys)
            {
                GameObject chunk = chunks[key];
                Destroy(chunk);
                chunks.Remove(key);
            }
        }
    }
}