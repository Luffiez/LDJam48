using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerDig : MonoBehaviour
    {
        public float DigSpeed = 1f;
        public Transform digParticlePivot;
        public ParticleSystem digParticles;
        GameTiles tiles;
        PlayerController playerController;

        private void Awake()
        {
            tiles = FindObjectOfType<GameTiles>();
            playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (!playerController.IsDigging && digParticles.isPlaying)
                digParticles.Stop();
        }

        internal void TryDig(DigDirection direction)
        {
            Vector3 position = GetDigPosition(direction);
            Vector3Int pos = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);

            WorldTile tileToDig = tiles.GetTileAt(pos);

            if (tileToDig != null && tileToDig.Ore.Data.ore != Ore.BedRock)
            {
                if (!digParticles.isPlaying)
                    digParticles.Play();

                RotateDigParticles(direction);
                tileToDig.Ore.Data.durability -= Time.deltaTime * DigSpeed;

                if (tileToDig.Ore.Data.durability <= 0)
                {
                    Debug.Log("Mined " + tileToDig.Ore.Data.ore.ToString());
                    tiles.RemoveTile(tileToDig);
                }
            }
            else if (playerController.IsDigging && digParticles.isPlaying)
                digParticles.Stop();
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