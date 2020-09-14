using Minitale.Utils;
using Mirror;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minitale.WorldGen
{
    public class WorldGenerator : NetworkBehaviour
    {
        public static float PLANE_SCALE = 10F;

        public GameObject chunk;
        public static Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();
        [SyncVar(hook = nameof(SyncSeed))] public int seed = 0;
        [ReadOnly] [SyncVar(hook = nameof(SyncTicks))] public int initTicks = 0;
        [SyncVar] public int rainDensity = 0;
        [SyncVar] public int timeOfDay = 0;

        public static WorldGenerator generator;

        private void Awake()
        {
            generator = this;
        }

        void Start()
        {
            DestroyImmediate(GetComponent<MeshFilter>());
            DestroyImmediate(GetComponent<Renderer>());
        }

        public override void OnStartServer()
        {
            initTicks = (int)DateTime.UtcNow.Ticks;
            Random.InitState(initTicks);
            if (seed == 0) seed = Random.Range(-100000, 100000);

            for(int x = 0; x < Chunk.chunkWidth; x++)
            {
                for(int z = 0; z < Chunk.chunkHeight; z++)
                {
                    GenerateChunkAt(x, 0f, z);
                }
            }
        }

        public void SyncTicks(int oldTicks, int newTicks)
        {
            Random.InitState(newTicks);
        }

        public void SyncSeed(int oldSeed, int newSeed)
         {
            seed = newSeed;
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
            if (isServer)
                NetworkServer.Spawn(chunk);
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
            }
            chunks.Clear();
        }
    }
}