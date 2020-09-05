using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [Header("Animation")]
        public bool animated;
        public bool randomIndex;
        public float animationSkip;
        public Texture[] frames;
    }
}