using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class GameTiles : MonoBehaviour
    {
        public static GameTiles instance;

        public PlayerController playerController;
        public Rigidbody2D playerRB;

        [Header("Overworld")]
        public Tilemap overworldTilemap;
        public Vector3Int overworldSize = new Vector3Int(20, 20, 0);
        public TileBase skyTile;

        [Header("Underworld")]
        public int chunksCount = 20;
        public int chunkRange = 2;
        public Vector3Int chunkSize = new Vector3Int(40, 40, 0);
        public Tilemap underworldMap;
        public Tilemap underworldOres;
        public Tilemap underworldLadders;
        public Tilemap underworldBackground;
        public TilemapCollider2D mapCollider; 
        public TilemapCollider2D ladderCollider;

        public TileBase groundTile;
        public TileBase ladderTile;
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

        public List<WorldChunk> worldChunks = new List<WorldChunk>();
        private int currentChunk = 0;
        private float seed;
        SceneFade sceneFade;

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

            seed = Random.Range(0f, 10000f);

            underworldMap.size = new Vector3Int(chunkSize.x, chunkSize.y * chunksCount, 0);
            underworldOres.size = new Vector3Int(chunkSize.x, chunkSize.y * chunksCount, 0);
            underworldLadders.size = new Vector3Int(chunkSize.x, chunkSize.y * chunksCount, 0);
            underworldBackground.size = new Vector3Int(chunkSize.x, chunkSize.y * chunksCount, 0);

            GenerateOverworld();
            GenerateWorldChunk();
            LoadNearbyChunks();
            sceneFade = FindObjectOfType<SceneFade>();
            sceneFade.BeginFade(-1);
            playerRB.bodyType = RigidbodyType2D.Dynamic;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                seed = Random.Range(0f, 10000f);

                GenerateWorldChunk();
                LoadNearbyChunks();
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                if(Input.GetKey(KeyCode.DownArrow) && currentChunk < chunksCount - 2)
                    currentChunk++;
                else if (Input.GetKey(KeyCode.UpArrow) && currentChunk > 0)
                    currentChunk--;
                LoadNearbyChunks();
            }
            CheckChunksNearPlayer();

        }

        void CheckChunksNearPlayer()
        {
            int targetChunk = currentChunk;
            float dist;
            float minDist;

            minDist = Mathf.Abs(playerController.Depth - worldChunks[currentChunk].StartDepth + chunkSize.y / 2);

            if (currentChunk > 0)
            {
                dist = Mathf.Abs(playerController.Depth - worldChunks[currentChunk - 1].StartDepth + chunkSize.y / 2);
                if (dist < minDist)
                    targetChunk = currentChunk - 1;
            }

            if(currentChunk < chunksCount - 2)
            {
                dist = Mathf.Abs(playerController.Depth - worldChunks[currentChunk + 1].StartDepth + chunkSize.y / 2);
                if (dist < minDist)
                    targetChunk = currentChunk + 1;
            }

            if(targetChunk != currentChunk)
            {
                currentChunk = targetChunk;
                LoadNearbyChunks();
            }
        }
        private void GenerateOverworld()
        {
            overworldTilemap.size = overworldSize;
            for (int x = 0; x < overworldSize.x; x++)
            {
                for (int y = 1; y < overworldSize.y; y++)
                {
                    overworldTilemap.SetTile(new Vector3Int(x, y, 0), skyTile);
                }
            }
        }

        private void GenerateWorldChunk()
        {
            worldChunks.Clear();
       
            TileBase tile = null;
            OreData oreData = rockData;
            currentChunk = 0;

            for (int chunks = 0; chunks < chunksCount; chunks++)
            {
                WorldChunk chunk = new WorldChunk();
                int yDepth = -Mathf.RoundToInt((worldChunks.Count) * chunkSize.y);
                chunk.StartDepth = yDepth;

                for (int x = 0; x <  chunkSize.x; x++)
                {
                    for (int y = yDepth; y > yDepth - chunkSize.y; y--)
                    {
                        if (x == 0 || x == chunkSize.x - 1 || y == yDepth - chunkSize.y -1 ||
                            y == 0 && x < 10)
                        {
                            oreData = bedRockData;
                            tile = bedRockData.tile;
                        }
                        else if (y > -5)
                        {
                            oreData = rockData;
                            tile = groundTile;
                        }
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
                            }
                            else
                            {
                                oreData = null;
                                tile = null;
                            }
                                
                        }

                        Vector3Int pos = new Vector3Int(x, y, 0);
                        SetChunkTileData(chunk, tile, pos, oreData, "Underworld");
                        tile = null;
                    }
                }
                worldChunks.Add(chunk);
            }
        }

        private void LoadNearbyChunks()
        {
            if (currentChunk == 0) // First chunk
            {
                DisplayChunkTiles(worldChunks[0]);
                DisplayChunkTiles(worldChunks[1]);

                //Previous
                HideChunkTiles(worldChunks[2]);
            }
            else if (currentChunk == chunksCount - 1) // Last chunk
            {
                DisplayChunkTiles(worldChunks[chunksCount - 2]);
                DisplayChunkTiles(worldChunks[chunksCount - 1]);

                //Previous
                HideChunkTiles(worldChunks[chunksCount - 3]);
            }
            else // Chunks inbetween first and last chunks
            {
                DisplayChunkTiles(worldChunks[currentChunk - 1]);
                DisplayChunkTiles(worldChunks[currentChunk]);
                DisplayChunkTiles(worldChunks[currentChunk + 1]);

                if(currentChunk > 1)
                    HideChunkTiles(worldChunks[currentChunk - 2]);

                if(currentChunk < chunksCount - 2)
                    HideChunkTiles(worldChunks[currentChunk + 2]);
            }

            if (mapCollider)
                mapCollider.ProcessTilemapChanges();
        }

        private OreData GetOreAtDensity(float density, int depth)
        {
            // lower density requirement the further down we go!
            // density += depth * 0.01f;
            //Debug.Log("Density: " + density + ", Depth: " + depth);

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


        public WorldTile GetTileAt(Vector3 position)
        {
            position.x = Mathf.RoundToInt(position.x);
            position.y = Mathf.RoundToInt(position.y);

            foreach (WorldChunk chunk in worldChunks)
            {
                if(chunk.Displayed)
                {
                    WorldTile worldTile = chunk.tiles.Find(tile => tile.WorldLocation == position);
                    if (worldTile != null)
                        return worldTile;
                }
            }
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
                tile.TilemapMember.SetTile(tile.LocalPlace, null);
                tile.Ore.TilemapMember.SetTile(tile.LocalPlace, null);
                TilemapCollider2D collider = tile.TilemapMember.gameObject.GetComponent<TilemapCollider2D>();
                if (collider)
                    collider.ProcessTilemapChanges();

                //tile.WorldChunkMember.tiles.Remove(tile);
            }
        }

        public bool TileHasLadder(Vector3 position)
        {
            WorldTile tile = GetTileAt(position);
            Debug.Log("Ladder Tile: " + tile);
            if (tile != null && tile.Ladder != null && tile.Ladder.HasLadder)
                return true;
            else
                return false;
        }

        public bool PlaceLadderTile(Vector3 position)
        {
            WorldTile tile = GetTileAt(position);
            if(tile == null)
            {
                Debug.LogWarning("No tile found at " + position + "!");
                return false;
            }

            tile.Ladder.HasLadder = true;
            tile.Ladder.TilemapMember.SetTile(tile.LocalPlace, ladderTile);

            if (ladderCollider)
                ladderCollider.ProcessTilemapChanges();
            return true;
        }

        void HideChunkTiles(WorldChunk chunk)
        {
            if (!chunk.Displayed)
                return;

            foreach (WorldTile tile in chunk.tiles)
            {
                underworldMap.SetTile(tile.LocalPlace, null);
                underworldBackground.SetTile(tile.LocalPlace, null);

                if (tile.Ore == null || tile.Ore.Data == null)
                    continue;

                if (tile.Ore.Data.ore != Ore.Rock || tile.Ore.Data.ore != Ore.BedRock)
                    underworldOres.SetTile(tile.LocalPlace, null);
            }

            chunk.Displayed = false;
        }

        void DisplayChunkTiles(WorldChunk chunk)
        {
            if (chunk.Displayed)
                return;

            foreach (WorldTile tile in chunk.tiles)
            {
                underworldBackground.SetTile(tile.LocalPlace, backgroundTile);
                underworldMap.SetTile(tile.LocalPlace, tile.TileBase);

                if (tile.Ore == null || tile.Ore.Data == null)
                    continue;

                if (tile.Ore.Data.ore != Ore.Rock || tile.Ore.Data.ore != Ore.BedRock)
                    underworldOres.SetTile(tile.LocalPlace, tile.Ore.Data.tile);
            }


            chunk.Displayed = true;
        }

        void SetChunkTileData(WorldChunk chunk, TileBase tile, Vector3Int localPlace, OreData oreData, string biome)
        {
            float durability = 0;
            if (oreData != null)
                durability = oreData.durability;

            var worldTile = new WorldTile
            {
                LocalPlace = localPlace,
                WorldLocation = underworldMap.CellToWorld(localPlace),
                TileBase = tile,
                TilemapMember = underworldMap,
                WorldChunkMember = chunk,
                Name = localPlace.x + "," + localPlace.y,
                Ore = new OreTile(oreData, underworldOres),
                Ladder = new LadderTile(false, underworldLadders),
                Durability = durability,
                Biome = biome
            };

            chunk.tiles.Add(worldTile);
        }
    }

    [System.Serializable]
    public class WorldChunk
    {
        public bool Displayed = false;
        public int StartDepth;
        public List<WorldTile> tiles = new List<WorldTile>();
    }
}
