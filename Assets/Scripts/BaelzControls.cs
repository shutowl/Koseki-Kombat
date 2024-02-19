using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaelzControls : MonoBehaviour
{
    public enum enemyState
    {
        idle,
        attacking,
        inCutscene,
        dying,
        dead
    }
    public enemyState curState;

    public float speed = 10f;
    public float jumpForce = 600f;
    float attackTimer;
    float rngCounter = 0f;  // Time between each attack
    int attackNum = 0;      // Used for attack timings
    int attackStep = 1;     // Current step of current attack
    int lastAttack = 0;     // Last attack performed (prevents repeat attacks
    int direction = 1;
    int difficulty = 1;     // Difficulty is determined by dice roll

    float fireRateTimer;

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject[] bullets;
    private BossHealthBar healthBar;
    public GameObject dice;

    public bool grounded;


    void Start()
    {
        healthBar = GetComponent<BossHealthBar>();
        rb = GetComponent<Rigidbody2D>();
        curState = enemyState.idle;
        player = GameObject.FindGameObjectWithTag("Player");

        rngCounter = 1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //-----IDLE STATE-----
        if (curState == enemyState.idle)
        {
            if (grounded)
            {
                rb.velocity = Vector2.zero;

                if (rngCounter > 0)
                {
                    rngCounter -= Time.deltaTime;
                }
            }
        }
        else if (curState == enemyState.attacking)
        {
            switch (attackNum)
            {
                //Dice roll move
                case 0:
                    //Jump towards center of screen
                    if(attackStep == 1)
                    {
                        direction = (Camera.main.transform.position.x - transform.position.x > 0) ? 1 : -1;
                        float jumpForceX = Mathf.Abs(transform.position.x - Camera.main.transform.position.x) * direction * 50f;
                        rb.AddForce(new Vector2(jumpForceX, jumpForce));

                        attackTimer = 0.5f;
                        attackStep = 2;
                    }
                    //Freeze position
                    if (attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0 && rb.velocity.y <= 0.1f && (transform.position.x >= -0.1f && transform.position.x <= 0.1f))
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezeAll;
                            attackTimer = 2f;
                            attackStep = 3;
                        }
                    }
                    //Summon Dice
                    if (attackStep == 3)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                            curState = enemyState.idle;
                        }
                    }
                    break;

                //Attack 1: Jumps up and shoots cheese projectiles at random angles
                case 1:
                    //Jump slightly towards player
                    if(attackStep == 1)
                    {
                        direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;
                        rb.AddForce(new Vector2(60 * direction, jumpForce));
                        //AudioManager.Instance.Play("Jump");

                        attackTimer = 0.5f;
                        attackStep = 2;
                    }
                    //Lock boss in place
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(rb.velocity.y <= 0.1f && attackTimer <= 0)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                            attackTimer = 2f;
                            attackStep = 3;
                        }
                    }
                    //Shoot bullets in random directions
                    if (attackStep == 3)
                    {
                        attackTimer -= Time.deltaTime;

                        //Shoot bullets
                        float fireRate = 0.1f;
                        int density = 3;
                        if (fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
                        else
                        {
                            float angle = Random.Range(0, 2 * Mathf.PI);
                            for(int i = 0; i < density; i++)
                            {
                                angle += 0.3f;
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                            }
                            fireRateTimer = fireRate;
                        }

                        if (attackTimer <= 0)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                            curState = enemyState.idle;
                        }
                    }
                    break;

                // Attack 2: Flip Upside down, walk around, and shoot bullets downwards
                case 2:
                    if(attackStep == 1)
                    {

                    }
                    break;

            }//end switch
        }

        //-----DYING STATE------
        else if (curState == enemyState.dying)
        {
            rb.velocity = Vector2.zero;
            direction = (FindObjectOfType<PlayerControls>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(200f * -direction, 400f));
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            //GetComponent<BoxCollider2D>().enabled = false;
            curState = enemyState.dead;
            GetComponent<Hazard>().SetActive(false);
            attackTimer = 1f;

            Debug.Log("Baelz defeated");
        }

        //-----DEAD STATE-----
        else if (curState == enemyState.dead)
        {
            Debug.Log(rb.velocity);
            if (grounded)
            {
                if (Mathf.Abs(rb.velocity.x) > 0.2f)
                {
                    rb.velocity = new Vector2(rb.velocity.x - (0.1f * -direction), rb.velocity.y);
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    curState = enemyState.inCutscene;
                }
            }
        }

        //-----CUTSCENE STATE-----
        else if(curState == enemyState.inCutscene)
        {
            //idk
        }

        //Counters and Timers
        if (grounded && rngCounter <= 0 && curState != enemyState.inCutscene)
        {
            rngCounter = Mathf.Clamp(Random.Range(1f, 2f), 0.2f, 10f);  //from 1 to [actionTimer] seconds
            curState = enemyState.attacking;
            attackTimer = 1f;

            //weighted RNG for attacks
            int RNG = Random.Range(1, 2);

            /*
            attackNum = RNG;
            while(attackNum == lastAttack)
            {
                attackNum = Random.Range(1, 2);
            }
            lastAttack = attackNum;
            */

            attackNum = 0;  //Debug for testing specific attacks

            attackStep = 1; //Reset attack step to 1 after each attack
            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;  //enemy faces towards player upon landing
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerBullet"))
        {
            healthBar.TakeDamage(col.GetComponent<PlayerBullet>().damage);
            Destroy(col.gameObject);
        }
    }
}
