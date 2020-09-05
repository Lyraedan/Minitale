using Minitale.WorldGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Player {
    public class CameraControl : MonoBehaviour
    {

        private Camera camera;

        // Start is called before the first frame update
        void Start()
        {
            camera = Camera.main;

            GenerateChunksAroundMe();
        }

        // Update is called once per frame
        void Update()
        {
            GenerateChunksAroundMe();
            MakeChunksAroundMeVisible();
        }

        public void MakeChunksAroundMeVisible()
        {
            float x = Mathf.Round(transform.position.x / (WorldGenerator.PLANE_SCALE * Chunk.chunkWidth));
            float z = Mathf.Round(transform.position.z / (WorldGenerator.PLANE_SCALE * Chunk.chunkHeight));

            for (float xx = x - 1; xx < x + 1; xx++)
            {
                for(float zz = z - 1; zz < z + 1; zz++)
                {
                    WorldGenerator.GetChunkAt(x, z).RenderChunk(true);
                }
            }
        }

        public void GenerateChunksAroundMe()
        {
            float x = Mathf.Round(transform.position.x / (WorldGenerator.PLANE_SCALE * Chunk.chunkWidth));
            float z = Mathf.Round(transform.position.z / (WorldGenerator.PLANE_SCALE * Chunk.chunkHeight));

            for (float xx = x - 1; xx < x + 1; xx++)
            {
                for (float zz = z - 1; zz < z + 1; zz++)
                {
                    WorldGenerator.generator.GenerateChunkAt(xx, 0f, zz);
                }
            }
        }

        public string ChunkCoords()
        {
            float x = Mathf.Round(transform.position.x / (WorldGenerator.PLANE_SCALE * Chunk.chunkWidth));
            float z = Mathf.Round(transform.position.z / (WorldGenerator.PLANE_SCALE * Chunk.chunkHeight));
            return $"{x}_{z}";
        }
    }
}