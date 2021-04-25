using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class WorldTile
    {
        public Vector3Int LocalPlace { get; set; }

        public Vector3 WorldLocation { get; set; }

        public TileBase TileBase { get; set; }

        public Tilemap TilemapMember { get; set; }

        public WorldChunk WorldChunkMember { get; set; }

        public float Durability { get; set; }

        public string Name { get; set; }

        public LadderTile Ladder { get; set; }

        public OreTile Ore { get; set; }

        public string Biome { get; set; }
    }

    public class LadderTile
    {
        public LadderTile(bool hasLadder, Tilemap tilemap)
        {
            HasLadder = hasLadder;
            TilemapMember = tilemap;
        }
        public bool HasLadder { get; set; }
        public Tilemap TilemapMember { get; set; }
    }

    public class OreTile
    {
        public OreTile(OreData oreData, Tilemap tilemap)
        {
            Data = oreData;
            TilemapMember = tilemap;
        }

        public OreData Data { get; set; }
        public Tilemap TilemapMember { get; set; }
    }
}
