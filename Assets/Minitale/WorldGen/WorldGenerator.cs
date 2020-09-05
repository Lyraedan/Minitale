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
        public Dictionary<string, GameObject> chunks = new Dictionary<string, GameObject>();
        public int seed = 0;

        // Start is called before the first frame update
        void Start()
        {
            DestroyImmediate(GetComponent<MeshFilter>());
            DestroyImmediate(GetComponent<Renderer>());

            if (seed == 0) seed = DateTime.Now.Millisecond * 10000;

            for (int x = 0; x < 30; x++)
            {
                for(int z = 0; z < 30; z++)
                {
                    GenerateChunkAt(new Vector3(x * (PLANE_SCALE * Chunk.chunkWidth), 0f, z * (PLANE_SCALE * Chunk.chunkHeight)));
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
 
        }

        public void SmoothChunks()
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    Vector3 location = new Vector3(x * (PLANE_SCALE * Chunk.chunkWidth), 0f, z * (PLANE_SCALE * Chunk.chunkHeight));
                    string key = $"Chunk_{location.ToString()}";
                    chunks[key].GetComponent<Chunk>().Smooth(seed);
                }
            }
        }

        public void GenerateChunkAt(Vector3 location)
        {
            string key = $"Chunk_{location.ToString()}";
            if (chunks.ContainsKey(key)) return;
            GameObject chunk = Instantiate(this.chunk, location, Quaternion.identity);
            chunk.name = key;
            chunk.transform.SetParent(transform);
            chunks.Add(key, chunk);

            Chunk c = chunk.GetComponent<Chunk>();
            c.GenerateChunk(seed);
        }
    }
}