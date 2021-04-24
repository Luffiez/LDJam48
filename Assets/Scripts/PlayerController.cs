using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public LayerMask groundMask;
        public float moveVelocity = 3f;
        public float groundCheckOffset = 0.5f;
        public float groundCheckRadius = 0.25f;

        [Header("Jump Settings")]
        public float jumpVelocity = 3f;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        bool jumpRequest = false;
        Rigidbody2D rb;
        Animator anim;

        bool isDigging = false;
        bool isFacingRight = false;
        bool isGrounded;
        float hInput;
        PlayerDig playerDig;

        DigDirection digDirection;
        private Vector2 currentVelocity;

        public bool IsGrounded { get => isGrounded; private set => isGrounded = value; }
        public Vector2 CurrentVelocity { get => currentVelocity; private set => currentVelocity = value; }
        public bool IsDigging { get => isDigging; private set => isDigging = value; }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            playerDig = GetComponent<PlayerDig>();
        }


        void Update()
        {
            CheckIfGrounded();
            GetPlayerInput();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            UpdateFallVelocity();

            if (jumpRequest)
                Jump();
        }

        private void LateUpdate()
        {
            AnimatePlayer();

            currentVelocity = rb.velocity;
        }

        private void CheckIfGrounded()
        {
            IsGrounded = Physics2D.CircleCast(transform.position - Vector3.down * groundCheckOffset, groundCheckRadius, Vector3.down, groundCheckRadius, groundMask);
        }

        private void UpdateFallVelocity()
        {
            if (rb.velocity.y < 0)
                rb.gravityScale = fallMultiplier;
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
                rb.gravityScale = lowJumpMultiplier;
            else
                rb.gravityScale = 1f;
        }

        void GetPlayerInput()
        {
            hInput = Input.GetAxis("Horizontal");
            digDirection = GetDigDirection();

            if (hInput > 0 && !isFacingRight ||
                hInput < 0 && isFacingRight)
                Flip();

            if (IsGrounded && !jumpRequest && Input.GetKeyDown(KeyCode.Space))
            {
                jumpRequest = true;
                isDigging = false;
            }

            if (isGrounded && Input.GetKey(KeyCode.LeftControl))
            {
                //Debug.Log("Dig Direction: " + direction.ToString());
                playerDig.TryDig(digDirection);
                if (!isDigging)
                    isDigging = true;
            }

            if (isDigging && Input.GetKeyUp(KeyCode.LeftControl))
                isDigging = false;
        }

        private DigDirection GetDigDirection()
        {
            DigDirection direction;

            if (hInput > 0)
                direction = DigDirection.Right;
            else if (hInput < 0)
                direction = DigDirection.Left;
            else
            {
                if (Input.GetKey(KeyCode.DownArrow))
                    direction = DigDirection.Down;
                else if (Input.GetKey(KeyCode.UpArrow))
                    direction = DigDirection.Up;
                else
                {
                    if(isFacingRight)
                        direction = DigDirection.Right;
                    else
                        direction = DigDirection.Left;
                }
            }

            return direction;
        }

        private void Jump()
        {
            rb.AddForce(new Vector2(rb.velocity.x, jumpVelocity), ForceMode2D.Impulse);
            jumpRequest = false;
        }

        void MovePlayer()
        {
            rb.velocity = new Vector2(hInput * moveVelocity, rb.velocity.y);
        }

        void AnimatePlayer()
        {
            if (hInput != 0 && !anim.GetBool("isMoving"))
            {
                anim.SetBool("isMoving", true);
            }
            else if (hInput == 0 && anim.GetBool("isMoving"))
            {
                anim.SetBool("isMoving", false);
            }

            anim.SetBool("isGrounded", IsGrounded);
           
            anim.SetFloat("digDirection", (int)digDirection);
            anim.SetBool("isDigging", isDigging);
        }

        void Flip()
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(isFacingRight?-1:1, 1, 1);
        }

        private void OnDrawGizmos()
        {
            if (isGrounded)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position - Vector3.down * groundCheckOffset, groundCheckRadius);
        }
    }
}
