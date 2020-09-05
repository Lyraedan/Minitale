using Boo.Lang.Environments;
using Minitale.WorldGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Player
{
    public class CameraControl : MonoBehaviour
    {
        private Vector3 position;

        // Start is called before the first frame update
        public void Init()
        {
            position = transform.position;

            GenerateChunksAroundMe();
            MakeChunksAroundMeVisible();
        }

        // Update is called once per frame
        public void HandleWorld()
        {
            if (Vector3.Distance(position, transform.position) < 1f) return;
            position = transform.position;

            GenerateChunksAroundMe();
            MakeChunksAroundMeVisible();
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

            var maxSize = 3;
            var renderSize = 1;
            for (float mz = z - maxSize; mz <= z + maxSize; ++mz)
            {
                for (float mx = x - maxSize; mx <= x + maxSize; ++mx)
                {
                    if ((Mathf.Abs(mx) <= renderSize) && (Mathf.Abs(mz) <= renderSize))
                    {
                        WorldGenerator.generator.GenerateChunkAt(mx, 0, mz);
                        continue;
                    }
                    WorldGenerator.GetChunkAt(mx, 0f, mz).RenderChunk(false);
                }
            }

            /*
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

            //Corners
            WorldGenerator.GetChunkAt(x - 2, 0f, z + 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x + 2, 0f, z + 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x - 2, 0f, z - 2).RenderChunk(false);
            WorldGenerator.GetChunkAt(x + 2, 0f, z - 2).RenderChunk(false);
            */
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