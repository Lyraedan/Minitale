using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Minitale.WorldGen
{
    [CreateAssetMenu(fileName="TileList", menuName="Minitale/World/TileList")]
    public class TileList : ScriptableObject
    {

        public Tile[] tiles;

    }

    [System.Serializable]
    public class Tile {

        [Header("Base")]
        public string name;
        public int id;
        public GameObject prefab;
        public Texture2D texture;
        [Header("Navmesh")]
        public bool navmeshEnabled;
        public GameObject navSurface;
        [Header("Animation")]
        public bool animated;
        public bool randomIndex;
        public float animationSkip;
        public Texture[] frames;
    }
}