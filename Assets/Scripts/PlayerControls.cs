using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public enum playerState
    {
        moving,
        blocking,
        hitstun,
        inCutscene,
        dead
    }
    public playerState curState;

    [Header("Movement")]
    private int direction = 1;
    public float speed = 50f;
    public float maxSpeed = 3f;
    public float jumpPower = 100f;
    private bool bufferJump = false;
    bool doubleJump = true;

    public float gravityScale = 1.5f;               //Player Gravity
    public float fallGravityMultiplier = 1.5f;      //Player fall gravity

    [Header("Coyote Time And Jump Buffers")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Hitstun")]
    public float knockbackLimit = 0.5f;     //Max duration of knockback
    public float knockbackStrength = 5f;
    private float hitstun = 0.5f;              //hitstun value
    private float hitstunCounter;
    public float damagediFrames = 1f;
    private float damagediFramesCounter;
    public float hitstunFlashRate = 0.2f;
    float hitstunFlashTimer = 0f;
    bool flashOn = false;
    int timesHit = 0;

    [Header("Hitbox")]
    public float hitboxSize = 0.35f;
    public SpriteRenderer hitbox;
    public Transform hitboxTransform;

    [Header("Block")]
    public float blockDuration = 2f;        //Duration player in invulnerable for
    public float blockIdleDuration = 0.5f;  //Duration player stays still for
    float blockDurationTimer;
    float blockIdleTimer;

    [Header("Other")]
    public ParticleSystem dust;
    bool paused = false;

    //Store Input Values
    private InputActions inputActions;
    private Vector2 moveVal;

    private Vector2 respawnPoint; //Respawn point
    public bool grounded;
    private Rigidbody2D rb;

    void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();

        rb = GetComponent<Rigidbody2D>();
        if (rb is null)
            Debug.LogError("Rigidbody is NULL");

        respawnPoint = transform.position;
        paused = false;
    }

    void Update()
    {
        float lastDir = moveVal.x;

        //------MOVING-------
        if (curState == playerState.moving && !paused)
        {
            moveVal = inputActions.Player.Move.ReadValue<Vector2>();

            //Jump
            if (coyoteTimeCounter >= 0f && jumpBufferCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpPower);

                if (bufferJump)
                {
                    rb.AddForce(Vector2.down * jumpPower / 3);
                    bufferJump = false;
                }

                jumpBufferCounter = 0f;

                //AudioManager.Instance.Play("JumpPlayer");
            }
            //Jump Buffer
            if (inputActions.Player.Jump.WasPressedThisFrame())
            {
                jumpBufferCounter = jumpBufferTime;
                if (!grounded) bufferJump = false;
                
                if(doubleJump)
                {
                    coyoteTimeCounter = coyoteTime;
                    jumpBufferCounter = jumpBufferTime;
                    if (!grounded) bufferJump = false;
                    doubleJump = false;
                }
            }
            //Control Jump Height
            if (inputActions.Player.Jump.WasReleasedThisFrame())
            {
                if (jumpBufferCounter > 0)
                    bufferJump = true;

                if (rb.velocity.y >= 0.1)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2); //Slows down y-axis momentum
                    coyoteTimeCounter = 0f;
                }

                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            //Jump Gravity
            if (rb.velocity.y <= 0 && !grounded)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.gravityScale = gravityScale * fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }

            //Block
            if (inputActions.Player.Block.WasPressedThisFrame())
            {
                curState = playerState.blocking;
                blockDurationTimer = blockDuration;
                blockIdleTimer = blockIdleDuration;
            }
        }

        //-----BLOCKING-------
        else if (curState == playerState.blocking)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            if(blockDurationTimer >= blockDuration - 0.05f) //Prevent jumping and blocking
            {
                rb.velocity = Vector2.zero;
            }

            blockDurationTimer -= Time.deltaTime;
            blockIdleTimer -= Time.deltaTime;

            if(blockIdleTimer <= 0)
            {
                curState = playerState.moving;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        //-----HITSTUN-------
        else if (curState == playerState.hitstun)
        {
            //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later

            //knockback            
            if (hitstunCounter < (hitstun - knockbackLimit) && grounded) //prevents player from constantly sliding backwards
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = new Vector2(Mathf.Lerp(0, knockbackStrength, 1 - Mathf.Pow(1 - (hitstunCounter / hitstun), 3)) * -direction, rb.velocity.y);
            }
        }

        //------DEAD STATE-----
        else if (curState == playerState.dead)
        {
            rb.velocity = Vector2.zero;
            //Play death animation

            //DEBUG: reset position to recent respawn point
            transform.position = respawnPoint;
            //GetComponent<PlayerHealth>().FullHeal();
            curState = playerState.moving;
        }

        //------CUTSCENE STATE-------
        else if(curState == playerState.inCutscene)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }

        //-----TIMERS-----

        //Coyote Time
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
            doubleJump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Jump Buffer
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;
        else
            bufferJump = false;
    }

    void FixedUpdate()
    {
        if (curState == playerState.moving)
        {
            Vector3 easeVelocity = rb.velocity;
            easeVelocity.y = rb.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x *= 0.75f;

            float h = moveVal.x; // Direction (Left/Right)

            if (grounded)
                rb.velocity = easeVelocity;

            if (h > 0) direction = 1;   //1 = right
            if (h < 0) direction = -1;  //-1 = left

            rb.AddForce((Vector2.right * speed) * h); //Increases speed
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y); //Limits the player's speed
        }
    }


    
}
