using UnityEngine;


namespace Assets.Scripts
{
    public class SmoothCamera2D : MonoBehaviour
    {
        public Vector2 cameraOffset = new Vector2(0, 0.5f);
        public Vector2 cameraHorizontalBounds = new Vector2(0, 0.5f);
        public float playerOffsetMultiplier = 1f;
        public float dampTime = 0.15f;
        private Vector3 velocity = Vector3.zero;
        public PlayerController target;

        Vector3 destination;
        Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        void Update()
        {
            if (target)
            {
                Vector3 delta = target.transform.position - cam.ViewportToWorldPoint(new Vector2(cameraOffset.x, cameraOffset.y));
                destination = transform.position + delta + (Vector3)target.CurrentVelocity * playerOffsetMultiplier;
                destination.z = -10f;
                destination.x = Mathf.Clamp(destination.x, cameraHorizontalBounds.x, cameraHorizontalBounds.y);
            }
        }

        private void LateUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }
}