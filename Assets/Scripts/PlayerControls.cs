using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    public enum playerState
    {
        moving,
        blocking,
        hitstun,
        inCutscene,
        dying,
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
    public float stepRate = 0.3f;                   //How fast the step audio plays
    public float stepTimer = 0;

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

    [Header("Hitbox")]
    public float hitboxSize = 0.35f;
    public SpriteRenderer hitbox;
    public Transform hitboxTransform;

    [Header("Block")]
    public float blockDuration = 2f;        //Duration player in invulnerable for
    public float blockIdleDuration = 0.5f;  //Duration player stays still for
    float blockIdleTimer;
    public Image[] blockCharges;
    public float blockRechargeDuration = 20f;
    [SerializeField] int blocksLeft = 3;

    [Header("Shoot")]
    public GameObject bullet;
    bool shooting = false;
    public float fireRate = 0.25f;
    float fireTimer = 0f;
    bool aimUp = false;

    [Header("Other")]
    public ParticleSystem dust;
    bool paused = false;
    bool feetClink;

    //Store Input Values
    private InputActions inputActions;
    private Vector2 moveVal;

    private Vector2 respawnPoint; //Respawn point
    public bool grounded;
    private Rigidbody2D rb;

    private Animator anim;
    public float size;          // Determines size of player (used for sprite direction checking)

    void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();

        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        if (rb is null)
            Debug.LogError("Rigidbody is NULL");

        respawnPoint = transform.position;
        paused = false;

        blockCharges[0] = GameObject.Find("Block1").GetComponent<Image>();
        blockCharges[1] = GameObject.Find("Block2").GetComponent<Image>();
        blockCharges[2] = GameObject.Find("Block3").GetComponent<Image>();

        feetClink = (PlayerPrefs.GetInt("option1") == 1) ? true : false;
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

                if(doubleJump)
                    AudioManager.Instance.PlayOneShot("Jump");
                else
                    AudioManager.Instance.PlayOneShot("DoubleJump");
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
                if(blocksLeft > 0)
                {
                    float lastFillAmount = 0f;
                    if (blocksLeft < 3)
                    {
                        lastFillAmount = blockCharges[blocksLeft].fillAmount;
                        blockCharges[blocksLeft].fillAmount = 0f;
                    }
                    blockCharges[blocksLeft - 1].fillAmount = lastFillAmount;
                    blocksLeft--;
                    curState = playerState.blocking;
                    blockIdleTimer = blockIdleDuration;

                    damagediFramesCounter = blockDuration;
                    hitstunFlashTimer = hitstunFlashRate;
                    flashOn = false;
                    GetComponentInChildren<Shield>().ShieldOn(true, blockDuration);
                }

                shooting = false;
            }

            //Damaged IFrames after knockback
            if (damagediFramesCounter <= 0)
            {
                hitbox.GetComponent<BoxCollider2D>().enabled = true;
            }

            //Shoot
            if (inputActions.Player.Shoot.WasPressedThisFrame())
            {
                shooting = true;
            }
            if (inputActions.Player.Shoot.WasReleasedThisFrame())
            {
                fireTimer = fireRate;
                shooting = false;
            }

        }

        //-----BLOCKING-------
        else if (curState == playerState.blocking)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

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

            if (hitstunCounter <= 0)
            {
                curState = playerState.moving;
            }
        }

        //------DYING STATE-----
        else if(curState == playerState.dead)
        {
            //should work similar to baelz's death code
        }

        //------DEAD STATE-----
        else if (curState == playerState.dead)
        {
            rb.velocity = Vector2.zero;
            //Play death animation

            //DEBUG: reset position to recent respawn point
            transform.position = respawnPoint;
            GetComponent<PlayerHealth>().FullHeal();
            curState = playerState.moving;
        }

        //------CUTSCENE STATE-------
        else if(curState == playerState.inCutscene)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            rb.velocity = Vector2.zero;
        }

        //-----SHOOTING-----
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
        if (shooting)
        {
            if(fireTimer <= 0)
            {
                //Fire Bullet
                GameObject tempBullet = Instantiate(bullet, transform.position, Quaternion.identity);

                if (aimUp)
                {
                    tempBullet.GetComponent<PlayerBullet>().SetDirection(0, 1);
                }
                else
                {
                    tempBullet.GetComponent<PlayerBullet>().SetDirection(direction, 0);
                }

                fireTimer = fireRate;
                AudioManager.Instance.PlayOneShot("Bullet3");
            }
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

        if (hitstunCounter >= 0)
        {
            hitstunCounter -= Time.deltaTime;
        }
        if (damagediFramesCounter > 0)
        {
            damagediFramesCounter -= Time.deltaTime;
            hitbox.GetComponent<BoxCollider2D>().enabled = false;

            //flash
            hitstunFlashTimer -= Time.deltaTime;
            if (hitstunFlashTimer <= 0)
            {
                if (flashOn)
                {
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                }
                flashOn = !flashOn;
                hitstunFlashTimer = hitstunFlashRate;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        //Block
        if(blocksLeft <= 2)
        {
            if(blockCharges[blocksLeft].fillAmount < 1f)
            {
                Mathf.Clamp01(blockCharges[blocksLeft].fillAmount += (Time.deltaTime) / blockRechargeDuration);
            }
            else
            {
                blocksLeft = Mathf.Clamp(++blocksLeft, 0, 3);
            }
        }

        //Aim Up
        if (inputActions.Player.Up.WasPressedThisFrame())
        {
            aimUp = true;
        }
        if (inputActions.Player.Up.WasReleasedThisFrame())
        {
            aimUp = false;
        }

        //------Animations-------
        /* STATES:
         * 0 = moving
         * 1 = blocking
         * 2 = hitstun
         * 3 = inCutscene
         * 4 = dying
         * 5 = dead
         */
        if(curState != playerState.inCutscene)
        {
            anim.SetInteger("curState", (int)curState);
            anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
            anim.SetFloat("yVelocity", rb.velocity.y);
            anim.SetBool("grounded", grounded);
        }

        transform.localScale = new Vector3(size * direction, size, size);   //flips sprite of this object and its children (like hurtbox)
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

            if(feetClink && grounded && h != 0)
            {
                stepTimer -= Time.deltaTime;

                if(stepTimer <= 0)
                {
                    int step = Random.Range(1, 11);
                    AudioManager.Instance.PlayOneShot("Step" + step);
                    stepTimer = stepRate;
                }
            }
        }
    }

    public void SetDamageState(float hitstun)   //used for variable hitstun lengths
    {
        StopAllCoroutines();

        rb.velocity = Vector2.zero;
        curState = playerState.hitstun;
        hitstunCounter = hitstun;
        this.hitstun = hitstun;
        damagediFramesCounter = damagediFrames + hitstun;
        hitstunFlashTimer = hitstunFlashRate;
        flashOn = false;

        shooting = false;
        AudioManager.Instance.Play("Hurt");
    }

}
