using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public float size = 2.5f;
    float attackTimer;
    float rngCounter = 0f;  // Time between each attack
    int attackNum = 0;      // Used for attack timings
    int attackStep = 1;     // Current step of current attack
    int lastAttack = 0;     // Last attack performed (prevents repeat attacks
    int attacksTillDice = 0; // attacks left till dice roll move
    int direction = 1;
    int difficulty = 1;     // Difficulty is determined by dice roll

    float fireRateTimer;
    float angleOffset;
    int attack2Counter = 0; //Increments. determines number of bullets

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject[] bullets;
    private BossHealthBar healthBar;
    public GameObject dice;
    GameObject storedDie;

    public bool grounded;

    public GameObject dangerIndicator;
    GameObject laserIndicator;
    GameObject danger;
    Vector3 delayedPos = Vector2.zero;


    void Start()
    {
        healthBar = GetComponent<BossHealthBar>();
        rb = GetComponent<Rigidbody2D>();
        curState = enemyState.idle;
        player = GameObject.FindGameObjectWithTag("Player");

        attacksTillDice = 0;
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
                //Dice roll to determine attack strength
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
                            attackTimer = 0.5f;
                            attackStep = 3;
                        }
                    }
                    //Summon Dice
                    if (attackStep == 3)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            int dieDirection = (Random.Range(-1f, 1f) >= 0) ? 1 : -1; 
                            storedDie = Instantiate(dice, transform.position + new Vector3(dieDirection * 3, 5), Quaternion.identity);
                            storedDie.transform.DOMoveY(transform.position.y, 0.5f).SetEase(Ease.OutCubic);
                            GetComponent<BoxCollider2D>().enabled = false;

                            attackTimer = 5f;
                            attackStep = 4;
                        }
                    }
                    //Time limit for determining dice
                    if(attackStep == 4)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0 && !storedDie.GetComponent<Dice>().IsRolling())
                        {
                            difficulty = storedDie.GetComponent<Dice>().GetFace();
                            Debug.Log("Next few attacks have a power of: " + difficulty);
                            storedDie.transform.DOMoveY(transform.position.y + 5, 0.5f).SetEase(Ease.InCubic);
                            storedDie.GetComponent<Dice>().SetRollable(false);

                            attackTimer = 0.5f;
                            attackStep = 5;
                        }
                    }
                    //Dice are lifted back out of view, then destroyed
                    if(attackStep == 5)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            Destroy(storedDie);

                            GetComponent<BoxCollider2D>().enabled = true;
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

                            attackTimer = 3f;
                            attackStep = 3;
                        }
                    }
                    //Shoot bullets in random directions
                    if (attackStep == 3)
                    {
                        attackTimer -= Time.deltaTime;

                        //Shoot bullets
                        float fireRate = 0.2f - (Mathf.Clamp(difficulty * 0.015f, 0f, 0.1f));
                        int density = 2 + (Mathf.Clamp(difficulty - 2, 0, 6));  //2 to 6 bullets
                        if (fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
                        else
                        {
                            //float angle = Random.Range(0, 2 * Mathf.PI);
                            float angle = 0 + angleOffset;
                            angleOffset += 1f;
                            for(int i = 0; i < density; i++)
                            {
                                angle += 0.4f - (Mathf.Clamp(difficulty * 0.03f, 0f, 0.2f));
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                            }
                            fireRateTimer = fireRate;
                            AudioManager.Instance.PlayOneShot("Bullet2");
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
                    //Jump
                    if(attackStep == 1)
                    {
                        attack2Counter = 0;
                        rb.AddForce(new Vector2(0, jumpForce));
                        AudioManager.Instance.Play("Jump");

                        attackTimer = 0.5f;
                        attackStep = 2;
                    }
                    //Flip gravity
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0 && rb.velocity.y <= 0.1f)
                        {
                            transform.localEulerAngles = new Vector3(0, 0, 180);
                            rb.gravityScale = -1;
                            GetComponent<SpriteRenderer>().flipX = true;

                            attackStep = 3;
                        }
                    }
                    //Start moving to opposite side of camera
                    if(attackStep == 3)
                    {
                        if (grounded)
                        {
                            direction = (Camera.main.transform.position.x - transform.position.x > 0) ? 1 : -1;
                            transform.DOMoveX(Camera.main.transform.position.x + (9 * direction), 3).SetEase(Ease.OutSine);

                            attackTimer = 2f;
                            attackStep = 4;
                        }
                    }
                    //Shoot bullets downwards (alternate between 1, 2, 3 bullets, or maybe more if higher difficulty)
                    if(attackStep == 4)
                    {
                        attackTimer -= Time.deltaTime;

                        float fireRate = 0.2f - (Mathf.Clamp(difficulty * 0.02f, 0f, 0.12f));
                        if (fireRateTimer >= 0) { fireRateTimer -= Time.deltaTime; }
                        else
                        {
                            if(attack2Counter % 4 == 3) //3
                            {
                                float angle = 3 * Mathf.PI / 2; //downwards
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);

                                angle = 7 * Mathf.PI / 6; //down-left
                                x = Mathf.Cos(angle);
                                y = Mathf.Sin(angle);
                                pos = (Vector2)transform.position + new Vector2(x, y);
                                angleDegrees = angle * Mathf.Rad2Deg - 90;
                                rot = Quaternion.Euler(0, 0, angleDegrees);

                                bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);

                                angle = 11 * Mathf.PI / 6; //down-right
                                x = Mathf.Cos(angle);
                                y = Mathf.Sin(angle);
                                pos = (Vector2)transform.position + new Vector2(x, y);
                                angleDegrees = angle * Mathf.Rad2Deg - 90;
                                rot = Quaternion.Euler(0, 0, angleDegrees);

                                bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                                AudioManager.Instance.PlayOneShot("Bullet2");
                            }
                            else if((difficulty >= 3) && (attack2Counter % 4 == 2 || attack2Counter % 4 == 0)) //2
                            {
                                float angle = 4 * Mathf.PI / 3; //down-left
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);

                                angle = 5 * Mathf.PI / 3; //down-right
                                x = Mathf.Cos(angle);
                                y = Mathf.Sin(angle);
                                pos = (Vector2)transform.position + new Vector2(x, y);
                                angleDegrees = angle * Mathf.Rad2Deg - 90;
                                rot = Quaternion.Euler(0, 0, angleDegrees);

                                bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                                AudioManager.Instance.PlayOneShot("Bullet2");
                            }
                            else if (attack2Counter % 4 == 1) // 3
                            {
                                float angle = 3 * Mathf.PI / 2; //downwards
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                                AudioManager.Instance.PlayOneShot("Bullet2");
                            }

                            fireRateTimer = fireRate;
                            attack2Counter++;
                        }

                        //Move other direction
                        if(attackTimer <= 0 && Mathf.Abs(Camera.main.transform.position.x - transform.position.x) >= 8.9f)
                        {
                            direction = (Camera.main.transform.position.x - transform.position.x > 0) ? 1 : -1;
                            transform.DOMoveX(Camera.main.transform.position.x + (9 * direction), 3).SetEase(Ease.InOutSine);

                            attackTimer = 2f;
                            attackStep = 5;
                        }
                    }
                    if(attackStep == 5)
                    {
                        attackTimer -= Time.deltaTime;

                        float fireRate = 0.2f - (Mathf.Clamp(difficulty * 0.015f, 0f, 0.1f));
                        if (fireRateTimer >= 0) { fireRateTimer -= Time.deltaTime; }
                        else
                        {
                            if (attack2Counter % 4 == 3) //3
                            {
                                float angle = 3 * Mathf.PI / 2; //downwards
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);

                                angle = 7 * Mathf.PI / 6; //down-left
                                x = Mathf.Cos(angle);
                                y = Mathf.Sin(angle);
                                pos = (Vector2)transform.position + new Vector2(x, y);
                                angleDegrees = angle * Mathf.Rad2Deg - 90;
                                rot = Quaternion.Euler(0, 0, angleDegrees);

                                bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);

                                angle = 11 * Mathf.PI / 6; //down-right
                                x = Mathf.Cos(angle);
                                y = Mathf.Sin(angle);
                                pos = (Vector2)transform.position + new Vector2(x, y);
                                angleDegrees = angle * Mathf.Rad2Deg - 90;
                                rot = Quaternion.Euler(0, 0, angleDegrees);

                                bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                                AudioManager.Instance.PlayOneShot("Bullet2");
                            }
                            else if ((difficulty >= 3) && (attack2Counter % 4 == 2 || attack2Counter % 4 == 0)) //2
                            {
                                float angle = 4 * Mathf.PI / 3; //down-left
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);

                                angle = 5 * Mathf.PI / 3; //down-right
                                x = Mathf.Cos(angle);
                                y = Mathf.Sin(angle);
                                pos = (Vector2)transform.position + new Vector2(x, y);
                                angleDegrees = angle * Mathf.Rad2Deg - 90;
                                rot = Quaternion.Euler(0, 0, angleDegrees);

                                bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                                AudioManager.Instance.PlayOneShot("Bullet2");
                            }
                            else if(attack2Counter % 4 == 1) // 3
                            {
                                float angle = 3 * Mathf.PI / 2; //downwards
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = angle * Mathf.Rad2Deg - 90;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[0], pos, rot);
                                bullet.GetComponent<EnemyBullet>().SetDirection(x, y);
                                AudioManager.Instance.PlayOneShot("Bullet2");
                            }

                            fireRateTimer = fireRate;
                            attack2Counter++;
                        }

                        //Finish attack
                        if (attackTimer <= 0 && Mathf.Abs(Camera.main.transform.position.x - transform.position.x) >= 8.9f)
                        {
                            transform.localEulerAngles = new Vector3(0, 0, 0);
                            rb.gravityScale = 1;
                            GetComponent<SpriteRenderer>().flipX = false;

                            curState = enemyState.idle;
                        }
                    }
                    break;

                // Attack 3: Jump high offscreen, then slam down at the player's position. Lasers shoot from the ground after landing.
                case 3:
                    //Jump
                    if(attackStep == 1)
                    {
                        rb.AddForce(new Vector2(0, jumpForce*3));
                        AudioManager.Instance.Play("Jump");
                        GetComponent<BoxCollider2D>().enabled = false;

                        attackTimer = 2f;

                        danger = Instantiate(dangerIndicator, new Vector2(-100, -100), Quaternion.identity);
                        danger.GetComponent<DangerIndicator>().lifeTime = attackTimer;
                        delayedPos = player.transform.position;

                        attackStep = 2;
                    }
                    //indicator
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        //indicator
                        if(transform.position.y >= 7f)
                        {
                            Vector3 smoothedPos = Vector3.Lerp(delayedPos, player.transform.position, 0.5f * Time.deltaTime);
                            delayedPos = smoothedPos;
                            if (danger != null) danger.transform.position = new Vector3(delayedPos.x, -4.5f);
                        }

                        if(attackTimer <= 0)
                        {
                            transform.position = new Vector2(delayedPos.x, 7f);
                            rb.velocity = Vector2.zero;
                            rb.AddForce(Vector2.down * 5000f);

                            attackStep = 3;
                        }
                    }
                    //re-enable collider
                    if(attackStep == 3)
                    {
                        if(transform.position.y <= 2f)
                        {
                            GetComponent<BoxCollider2D>().enabled = true;
                            attackStep = 4;
                        }
                    }
                    if(attackStep == 4)
                    {
                        if (grounded)
                        {
                            for(int i = 0; i < (5 + Mathf.Pow(difficulty, 2)); i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
                                bullet.GetComponent<EnemyBullet>().EnablePhysics();
                                bullet.GetComponent<EnemyBullet>().SetDirection(Random.Range(-1.5f, 1.5f), Random.Range(2f, 3f));
                                bullet.GetComponent<EnemyBullet>().SetForce(Random.Range(200f, 300f));
                            }

                            CameraShake(1, 0.5f, 200, 10);

                            attackTimer = 0f;
                            attackStep = 5;
                        }
                    }
                    if(attackStep == 5)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            float laserTime = 1f;
                            attackTimer = 0.6f - (difficulty * 0.05f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty/6)), -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)), -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            GameObject laser = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)), -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.red);
                            laser.GetComponent<Laser>().delay = laserTime;

                            laser = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)), -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.cyan);
                            laser.GetComponent<Laser>().delay = laserTime;

                            attackStep = 6;
                        }
                    }
                    if(attackStep == 6)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            float laserTime = 1f;
                            attackTimer = 0.6f - (difficulty * 0.05f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)) * 2, -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)) * 2, -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            GameObject laser = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)) * 2, -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.cyan);
                            laser.GetComponent<Laser>().delay = laserTime;

                            laser = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)) * 2, -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.red);
                            laser.GetComponent<Laser>().delay = laserTime;

                            attackStep = 7;
                        }
                    }
                    if(attackStep == 7)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0)
                        {
                            float laserTime = 1f;
                            attackTimer = 0.6f - (difficulty * 0.05f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)) * 3, -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)) * 3, -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            GameObject laser = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)) * 3, -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.red);
                            laser.GetComponent<Laser>().delay = laserTime;

                            laser = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)) * 3, -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.cyan);
                            laser.GetComponent<Laser>().delay = laserTime;

                            attackStep = 8;
                        }
                    }
                    if(attackStep == 8)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0)
                        {
                            float laserTime = 1f;
                            attackTimer = 1f;

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)) * 4, -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            laserIndicator = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)) * 4, -3), Quaternion.identity);
                            laserIndicator.GetComponent<Laser>().indicator = true;
                            laserIndicator.GetComponent<Laser>().lifeTime = laserTime;
                            laserIndicator.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);

                            GameObject laser = Instantiate(bullets[1], transform.position + new Vector3((4 - (difficulty / 6)) * 4, -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.cyan);
                            laser.GetComponent<Laser>().delay = laserTime;

                            laser = Instantiate(bullets[1], transform.position + new Vector3(-(4 - (difficulty / 6)) * 4, -3), Quaternion.identity);
                            laser.GetComponent<Laser>().SetPositions(Vector2.zero, Vector2.up * 20f);
                            laser.GetComponent<Laser>().SetLifeTime(0.5f);
                            laser.GetComponent<Laser>().SetColor(Color.red);
                            laser.GetComponent<Laser>().delay = laserTime;

                            attackStep = 9;
                        }
                    }
                    if(attackStep == 9)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            curState = enemyState.idle;
                        }
                    }
                    break;

            }//end switch
        }

        //-----DYING STATE------
        else if (curState == enemyState.dying)
        {
            DOTween.Kill(transform);
            rb.gravityScale = 1;
            transform.eulerAngles = Vector2.zero;

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
            rngCounter = Random.Range(0.5f, 1f);
            curState = enemyState.attacking;
            attackTimer = 1f;

            int RNG;
            //weighted RNG for attacks
            if (attacksTillDice > 0)
            {
                RNG = Random.Range(1, 4);               //1-3

                /*
                attackNum = RNG;
                while(attackNum == lastAttack)
                {
                    attackNum = Random.Range(1, 2);
                }
                lastAttack = attackNum;
                */

                attackNum = RNG;
                //attackNum = 3;    //Debug for testing specific attacks
                lastAttack = attackNum;

                attacksTillDice--;
            }
            else
            {
                attackNum = 0;
                attacksTillDice = Random.Range(2, 4);   //2-3
            }


            //difficulty = 1; //Debug for specific difficulties

            attackStep = 1; //Reset attack step to 1 after each attack
            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;  //enemy faces towards player upon landing
        }

        transform.localScale = new Vector3(size * direction, size, size);   //flips sprite of this object and its children (like hurtbox)
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (attackNum != 0 && col.CompareTag("PlayerBullet"))
        {
            healthBar.TakeDamage(col.GetComponent<PlayerBullet>().damage);
            Destroy(col.gameObject);
        }
    }

    //some good values: (1, 0.5, 200, 10)
    void CameraShake(float duration, float strength, int vibrato, float randomness)
    {
        Camera.main.DOShakePosition(duration, strength, vibrato, randomness, true, ShakeRandomnessMode.Harmonic);
    }
}
