﻿using Minitale.WorldGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Player {
    public class CameraControl : MonoBehaviour
    {

        private Camera camera;

        // Start is called before the first frame update
        public void Init()
        {
            camera = Camera.main;

            GenerateChunksAroundMe();
        }

        // Update is called once per frame
        public void HandleWorld()
        {
            GenerateChunksAroundMe();
            MakeChunksAroundMeVisible();
        }

        public void HideOffscreenChunks()
        {
            float x = ChunkX();
            float z = ChunkZ();
        }

        public void MakeChunksAroundMeVisible()
        {
            float x = ChunkX();
            float z = ChunkZ();

            //Top row
            WorldGenerator.GetChunkAt(x - 1, 0f, z - 1).RenderChunk(true);
            WorldGenerator.GetChunkAt(x, 0f, z - 1).RenderChunk(true);
            WorldGenerator.GetChunkAt(x + 1, 0f, z - 1).RenderChunk(true);

            //Middle row
            WorldGenerator.GetChunkAt(x - 1, 0f, z).RenderChunk(true);
            WorldGenerator.GetChunkAt(x, 0f, z).RenderChunk(true);
            WorldGenerator.GetChunkAt(x + 1, 0f, z).RenderChunk(true);

            //Bottom row
            WorldGenerator.GetChunkAt(x - 1, 0f, z + 1).RenderChunk(true);
            WorldGenerator.GetChunkAt(x, 0f, z + 1).RenderChunk(true);
            WorldGenerator.GetChunkAt(x + 1, 0f, z + 1).RenderChunk(true);
        }

        public void GenerateChunksAroundMe()
        {
            float x = ChunkX();
            float z = ChunkZ();

            //Top row
            WorldGenerator.generator.GenerateChunkAt(x - 1, 0f, z - 1);
            WorldGenerator.generator.GenerateChunkAt(x, 0f, z - 1);
            WorldGenerator.generator.GenerateChunkAt(x + 1, 0f, z - 1);

            //Middle row
            WorldGenerator.generator.GenerateChunkAt(x - 1, 0f, z);
            WorldGenerator.generator.GenerateChunkAt(x, 0f, z);
            WorldGenerator.generator.GenerateChunkAt(x + 1, 0f, z);

            //Bottom row
            WorldGenerator.generator.GenerateChunkAt(x - 1, 0f, z + 1);
            WorldGenerator.generator.GenerateChunkAt(x, 0f, z + 1);
            WorldGenerator.generator.GenerateChunkAt(x + 1, 0f, z + 1);

            //Hide
            //Left
            WorldGenerator.GetChunkAt(x - 2, 0f, z - 1).RenderChunk(false);
            WorldGenerator.GetChunkAt(x - 2, 0f, z).RenderChunk(false);
            WorldGenerator.GetChunkAt(x - 2, 0f, z + 1).RenderChunk(false);

            //Right
            WorldGenerator.GetChunkAt(x + 2, 0f, z - 1).RenderChunk(false);
            WorldGenerator.GetChunkAt(x + 2, 0f, z).RenderChunk(false);
            WorldGenerator.GetChunkAt(x + 2, 0f, z + 1).RenderChunk(false);

            //Bottom
            WorldGenerator.GetChunkAt(x - 1, 0f, z - 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x, 0f, z - 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x + 1, 0f, z - 2).RenderChunk(false);

            //Top
            WorldGenerator.GetChunkAt(x - 1, 0f, z + 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x, 0f, z + 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x + 1, 0f, z + 2).RenderChunk(false);
        }

        public string ChunkCoords()
        {
            return $"{ChunkX()}_{ChunkZ()}";
        }

        float ChunkX()
        {
            return Mathf.Round(transform.position.x / (WorldGenerator.PLANE_SCALE * Chunk.chunkWidth));
        }

        float ChunkZ()
        {
            return Mathf.Round(transform.position.z / (WorldGenerator.PLANE_SCALE * Chunk.chunkHeight));
        }
    }
}