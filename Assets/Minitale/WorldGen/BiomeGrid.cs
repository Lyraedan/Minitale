using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.WorldGen
{
    public class BiomeGrid : MonoBehaviour
    {

        private void Start()
        {

        }

    }

    public class Biome : MonoBehaviour
    {
        public enum Type
        {
            PLAINS, WOODS, TUNDRA, DESERT, OCEAN
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(transform.position.x + (Chunk.chunkWidth * WorldGenerator.PLANE_SCALE) / 2, transform.position.y, transform.position.z + (Chunk.chunkWidth * WorldGenerator.PLANE_SCALE) / 2), new Vector3());
        }
    }
}