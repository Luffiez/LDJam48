using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class GameTiles : MonoBehaviour
    {
        public static GameTiles instance;

        [Header("Overworld")]
        public Tilemap overworldTilemap;
        public Vector3Int overworldSize = new Vector3Int(20, 20, 0);
        public TileBase skyTile;

        [Header("Underworld")]
        public Vector3Int chunkSize = new Vector3Int(40, 40, 0);
        public List<WorldWhunk> worldChunks = new List<WorldWhunk>();
        public TileBase groundTile;
        public TileBase backgroundTile;

        [Header("Cave Perlin")]
        public float caveDensity = 0.5f;
        public float caveScale = 1.5f;
        public float caveMultiplier = 4f;
        public float caveDivider = 0.33f;

        [Header("Ore Perlin")]
        public float oreScale = 1.5f;
        public float oreMultiplier = 4f;
        public float oreDivider = 0.33f;

        [Header("Ore Tiles")]
        public OreData rockData;
        public OreData bedRockData;
        public OreData silverData;
        public OreData saphireData;
        public OreData emeraldData;
        public OreData amethystData;
        public OreData rubyData;
        public OreData diamondData;

        private float chunkCount = 0;
        private float seed;
        public Dictionary<Vector3, WorldTile> tiles;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            tiles = new Dictionary<Vector3, WorldTile>();

            seed = Random.Range(0f, 10000f);

            GenerateOverworld();

            foreach (WorldWhunk chunk in worldChunks)
            {
                GenerateWorldChunk(chunk.Tilemap, chunk.OreMap, chunk.BackgroundMap);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                seed = Random.Range(0f, 10000f);

                foreach (WorldWhunk chunk in worldChunks)
                {
                    GenerateWorldChunk(chunk.Tilemap, chunk.OreMap, chunk.BackgroundMap);
                }
            }
        }

        private void GenerateWorldChunk(Tilemap tileMap, Tilemap oreMap, Tilemap backgroundMap)
        {
            tiles.Clear();
            tileMap.ClearAllTiles();
            oreMap.ClearAllTiles();
            backgroundMap.ClearAllTiles();
            tileMap.size = chunkSize;
            oreMap.size = chunkSize;
            backgroundMap.size = chunkSize;

            TileBase tile = null;
            OreData oreData = rockData;

            int xPos = Mathf.RoundToInt(tileMap.transform.position.x);
            int yPos = Mathf.RoundToInt(tileMap.transform.position.y);

            for (int x = xPos; x < xPos + chunkSize.x; x++)
            {
                for (int y = yPos - 1; y > yPos - chunkSize.y * chunkCount; y--)
                {
                    if (x == xPos || x == xPos + chunkSize.x - 1)
                    {
                        oreData = bedRockData;
                        tile = bedRockData.tile;
                    }
                    else if (y > -5)
                        tile = groundTile;
                    else
                    {
                        float xCoord = (float)x * caveScale + seed;
                        float yCoord = (float)y * caveScale + seed;

                        float caveNoise = SimplePerlin(xCoord, yCoord, caveMultiplier, caveDivider);

                        if (caveNoise < caveDensity)
                        {
                            tile = groundTile;

                            xCoord = (float)x * oreScale + seed;
                            yCoord = (float)y * oreScale + seed;

                            float oreNoise = SimplePerlin(xCoord, yCoord, oreMultiplier, oreDivider);
                            oreData = GetOreAtDensity(oreNoise, y);
                            Debug.Log(oreData);
                        }
                    }
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    backgroundMap.SetTile(new Vector3Int(x, y, 0), backgroundTile);

                    if (tile != null)
                        SetTile(tileMap, oreMap, tile, pos, oreData, "Underworld");

                    tile = null;
                }
            }

            TilemapCollider2D collider = tileMap.gameObject.GetComponent<TilemapCollider2D>();
            if (collider)
                collider.ProcessTilemapChanges();
            chunkCount++;
        }

        private OreData GetOreAtDensity(float density, int depth)
        {
            // lower density requirement the further down we go!
            // density += depth * 0.01f;
            Debug.Log("Density: " + density + ", Depth: " + depth);

            if (density >= diamondData.density && depth < diamondData.minDepth && depth > diamondData.maxDepth)
                return diamondData;
            else if (density >= rubyData.density && depth < rubyData.minDepth && depth > rubyData.maxDepth)
                return rubyData;
            else if (density >= amethystData.density && depth < amethystData.minDepth && depth > amethystData.maxDepth)
                return amethystData;
            else if (density >= emeraldData.density && depth < emeraldData.minDepth && depth > emeraldData.maxDepth)
                return emeraldData;
            else if (density >= saphireData.density && depth < saphireData.minDepth && depth > saphireData.maxDepth)
                return saphireData;
            else if (density >= silverData.density && depth < silverData.minDepth && depth > silverData.maxDepth)
                return silverData;
            else
                return rockData;
        }


        private float SimplePerlin(float x, float y, float multiplier, float divider)
        {
            float noise = Mathf.PerlinNoise(x, y);
            float highFrequencyNoise = Mathf.PerlinNoise(x * multiplier, y * multiplier);
            noise += highFrequencyNoise * divider;
            return noise;
        }

        private void GenerateOverworld()
        {
            overworldTilemap.size = overworldSize;
            for (int x = 0; x < overworldSize.x; x++)
            {
                for (int y = 0; y < overworldSize.y; y++)
                {
                    overworldTilemap.SetTile(new Vector3Int(x, y, 0), skyTile);
                }
            }
        }

        public WorldTile GetTileAt(Vector3 position)
        {
            position.x = Mathf.RoundToInt(position.x);
            position.y = Mathf.RoundToInt(position.y);

            if (tiles.ContainsKey(position))
                return tiles[position];

            return null;
        }

        public void RemoveTile(WorldTile tile)
        {
            if (tile == null)
            {
                Debug.LogWarning("TileToRemove is null!");
                return;
            }

            if (tile.Biome == "Underworld")
            {
                tiles.Remove(tile.WorldLocation);
                tile.TilemapMember.SetTile(tile.LocalPlace, null);
                tile.Ore.TilemapMember.SetTile(tile.LocalPlace, null);
                TilemapCollider2D collider = tile.TilemapMember.gameObject.GetComponent<TilemapCollider2D>();
                if (collider)
                    collider.ProcessTilemapChanges();
            }
        }

        void SetTile(Tilemap tileMap, Tilemap oreMap, TileBase tile, Vector3Int localPlace, OreData oreData, string biome)
        {
            var worldTile = new WorldTile
            {
                LocalPlace = localPlace,
                WorldLocation = tileMap.CellToWorld(localPlace),
                TileBase = tileMap.GetTile(localPlace),
                TilemapMember = tileMap,
                Name = localPlace.x + "," + localPlace.y,
                Ore = new OreTile(oreData, oreMap),
                Biome = biome
            };

            tiles.Add(worldTile.WorldLocation, worldTile);

            tileMap.SetTile(localPlace, tile);

            if (oreData.ore != Ore.Rock)
                oreMap.SetTile(localPlace, oreData.tile);
        }
    }

    [System.Serializable]
    public class WorldWhunk
    {
        public Tilemap Tilemap;
        public Tilemap OreMap;
        public Tilemap BackgroundMap;
    }
}
