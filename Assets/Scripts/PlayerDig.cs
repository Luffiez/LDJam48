using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerDig : MonoBehaviour
    {
        public float DigSpeed = 1f;
        public Transform digParticlePivot;
        public ParticleSystem digParticles;
        GameTiles gameTiles;
        PlayerController playerController;
        PlayerResources playerResources;
        PlayerEnergy playerEnergy;
        SoundManager soundManager;

        private void Awake()
        {
            gameTiles = FindObjectOfType<GameTiles>();
            playerResources = FindObjectOfType<PlayerResources>();
            playerEnergy = FindObjectOfType<PlayerEnergy>();
            playerController = GetComponent<PlayerController>();
            soundManager = FindObjectOfType<SoundManager>();
        }

        private void Update()
        {
            if(!playerController.IsDigging && digParticles.isPlaying)
            {
                digParticles.Stop();
            }
        }

        float GetTierMultiplier()
        {
            switch (playerResources.drillTier)
            {
                case 1: return 1.2f;
                case 2: return 1.4f;
                case 3: return 1.6f;
                case 4: return 1.8f;
                case 5: return 2.0f;
                default: return 1.2f;
            }
        }

        internal void TryDig(DigDirection direction)
        {
            Vector3 position = GetDigPosition(direction);

            WorldTile tileToDig = gameTiles.GetTileAt(position);

            if (tileToDig != null && tileToDig.Ore.Data != null && tileToDig.Ore.Data.ore != Ore.BedRock)
            {
                if (tileToDig.TilemapMember.GetTile(tileToDig.LocalPlace) == null)
                    return;

                if (!digParticles.isPlaying)
                    digParticles.Play();

                RotateDigParticles(direction);
                tileToDig.Durability -= Time.deltaTime * DigSpeed * GetTierMultiplier();
                playerEnergy.DecreaseEnergy(Time.deltaTime);
                if (tileToDig.Durability <= 0)
                {
                    Debug.Log("Mined " + tileToDig.Ore.Data.ore.ToString());
                    playerResources.AddOre(tileToDig.Ore.Data.ore);
                    gameTiles.RemoveTile(tileToDig);
                    soundManager.PlaySfx("Destroy", 0.65f);
                }
            }
            else if(digParticles.isPlaying)
            {
                digParticles.Stop();
            }
        }

        private void RotateDigParticles(DigDirection direction)
        {
            float z = 0;

            switch (direction)
            {
                case DigDirection.Up:   z = -90f; break;
                case DigDirection.Down: z =  90f; break;
            }

            digParticlePivot.localRotation = Quaternion.Euler(0, 0, z);
        }

        private Vector3 GetDigPosition(DigDirection direction)
        {
            switch (direction)
            {
                case DigDirection.Up: return transform.position + Vector3.up;
                case DigDirection.Down: return transform.position + Vector3.down;
                case DigDirection.Left: return transform.position + Vector3.left;
                case DigDirection.Right: return transform.position + Vector3.right;
                default: return transform.position + Vector3.up;
            }
        }
    }

    public enum DigDirection { Up, Down, Left, Right };
}