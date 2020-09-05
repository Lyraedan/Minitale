using Minitale.Utils;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.WorldGen
{
    public class WorldGenerator : MonoBehaviour
    {
        public static float PLANE_SCALE = 10F;

        public GameObject chunk;
        public static Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();
        public int seed = 0;

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

            SimplexNoise.SimplexNoise.Seed = seed;
            if (seed == 0) seed = SimplexNoise.SimplexNoise.Seed;

            /*
            for (int x = 0; x < 6; x++)
            {
                for(int z = 0; z < 6; z++)
                {
                    GenerateChunkAt(new Vector3(x, 0f, z));
                }
            }
            */
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenerateChunkAt(Vector3 location)
        {
            GenerateChunkAt(location.x, location.y, location.z);
        }

        public void GenerateChunkAt(float x, float y, float z)
        {
            string key = $"Chunk_{x}_{z}";
            if (chunks.ContainsKey(key)) return;

            GameObject chunk = Instantiate(this.chunk, new Vector3(x * (PLANE_SCALE * Chunk.chunkWidth), y, z * (PLANE_SCALE * Chunk.chunkHeight)), Quaternion.identity);
            chunk.name = key;
            chunk.transform.SetParent(transform);
            chunks.Add(key, chunk);

            Chunk c = chunk.GetComponent<Chunk>();
            c.GenerateChunk(seed);
        }

        public static Chunk GetChunkAt(Vector3 location)
        {
            return GetChunkAt(location.x, location.z);
        }

        public static Chunk GetChunkAt(float x, float z)
        {
            return GetChunkAtAsGameObject(x, z).GetComponent<Chunk>();
        }

        public static GameObject GetChunkAtAsGameObject(float x, float z)
        {
            string key = $"Chunk_{x}_{z}";
            if (!chunks.ContainsKey(key)) {
                generator.GenerateChunkAt(new Vector3(x, 0f, z));
                return chunks[key];
            }
            return chunks[key];
        }
    }
}