using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "New Ore Data")]
    public class OreData : ScriptableObject
    {
        public Ore ore;
        public float durability;
        public int value;
        public TileBase tile;
        public float density;
        public int minDepth;
        public int maxDepth;
    }

    public enum Ore { Rock, BedRock, Silver, Saphire, Emerald, Amethyst, Ruby, Diamond };
}
