using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public LayerMask groundMask;
        public float moveVelocity = 3f;
        public float climbVelocity = 2f;
        public float groundCheckOffset = 0.5f;
        public float groundCheckRadius = 0.25f;
        public GameObject lampGameObject;

        [Header("Jump Settings")]
        public float jumpVelocity = 3f;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        [Header("Action Keys")]
        public KeyCode drillKey;
        public KeyCode jumpKey;
        public KeyCode ladderKey;

        bool jumpRequest = false;
        Rigidbody2D rb;
        Animator anim;
        SceneFade sceneFade;

        bool isDigging = false;
        bool isFacingRight = false;
        bool isGrounded;
        float hInput;
        float vInput;
        PlayerDig playerDig;
        PlayerResources playerResources;
        GameTiles gameTiles;
        SoundManager soundManager;
        PlayerEnergy playerEnergy;

        DigDirection digDirection;
        private Vector2 currentVelocity;
        private int depth;
        public bool canMove = false;
        private bool isOnLadder = false;

        public bool IsGrounded { get => isGrounded; private set => isGrounded = value; }
        public Vector2 CurrentVelocity { get => currentVelocity; private set => currentVelocity = value; }
        public bool IsDigging { get => isDigging; private set => isDigging = value; }
        public int Depth { get => depth; private set => depth = value; }
        public bool CanMove
        {
            get => canMove;
            set
            {
                canMove = value;
                if(!CanMove)
                    rb.velocity = Vector2.zero;
            }
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            playerDig = GetComponent<PlayerDig>();
            playerResources = FindObjectOfType<PlayerResources>();
            gameTiles = FindObjectOfType<GameTiles>();
            sceneFade = FindObjectOfType<SceneFade>();
            soundManager = FindObjectOfType<SoundManager>();
            playerEnergy = FindObjectOfType<PlayerEnergy>();
        }


        void Update()
        {
            CheckIfGrounded();
            GetPlayerInput();
            UpdateDepth();
        }

        internal void ActivateLamp()
        {
            lampGameObject.SetActive(true);
        }

        private void UpdateDepth()
        {
            depth = (int)transform.position.y;
        }

        private void FixedUpdate()
        {
            if (!canMove)
                return;

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
            if (isOnLadder)
                return;

            if (rb.velocity.y < 0)
            {
                rb.gravityScale = fallMultiplier;
                if (rb.velocity.y < -moveVelocity * 1.5f)
                    rb.velocity = new Vector2(rb.velocity.x, -moveVelocity * 1.5f);
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
                rb.gravityScale = lowJumpMultiplier;
            else
                rb.gravityScale = 1f;
        }

        void GetPlayerInput()
        {
            hInput = Input.GetAxis("Horizontal");

            if (isOnLadder)
            {
                vInput = Input.GetAxis("Vertical");
            }

            digDirection = GetDigDirection();

            if (hInput > 0 && !isFacingRight ||
                hInput < 0 && isFacingRight)
                Flip();

            if ((IsGrounded || isOnLadder) && !jumpRequest && Input.GetKeyDown(jumpKey))
            {
                jumpRequest = true;
                if(isDigging)
                {
                    isDigging = false;
                    soundManager.StopDrill();
                }
            }

            if ((isGrounded || isOnLadder) && Input.GetKey(drillKey))
            { 
                playerDig.TryDig(digDirection);
                if (!isDigging)
                {
                    isDigging = true;
                    soundManager.StartDrill();
                }
            }

            if (isDigging && Input.GetKeyUp(drillKey))
            {
                isDigging = false;
                soundManager.StopDrill();
            }

            if (Input.GetKeyDown(ladderKey) && playerResources.Ladders > 0)
            {
                if (gameTiles.TileHasLadder(transform.position))
                {
                    if(!gameTiles.TileHasLadder(transform.position + Vector3.up))
                    {
                        if(gameTiles.PlaceLadderTile(transform.position + Vector3.up))
                        {
                            playerResources.UseLadder();
                            soundManager.PlaySfx("Ladder", 1f);
                        }
                    }
                }
                else
                {
                    if (gameTiles.PlaceLadderTile(transform.position))
                    {
                        playerResources.UseLadder();
                        soundManager.PlaySfx("Ladder", 1f);
                    }
                }
            }
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
            float y = rb.velocity.y;
            if (isOnLadder)
                y = vInput * climbVelocity;

            rb.velocity = new Vector2(hInput * moveVelocity, y);
        }

        internal IEnumerator Faint()
        {
            soundManager.PlaySfx("Faint", 1f);
            canMove = false;
            float wait = sceneFade.BeginFade(1, 0.5f);
            yield return new WaitForSeconds(wait);
            transform.position = new Vector3(7f, 1f, 0f);
            playerResources.ClearAllOres();
            playerEnergy.RefillEnergy();
            wait = sceneFade.BeginFade(-1, 1f);
            yield return new WaitForSeconds(wait);
            canMove = true;
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
                isOnLadder = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
                isOnLadder = false;
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
