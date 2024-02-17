//Used for reference


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class GuraMovement : Enemy
{
    [Header("Enemy Specific Variables")]
    public float speed = 10f;
    public float jumpForce = 300f;
    public float minActionRate = 0.5f;
    public float maxActionRate = 2f;    // Performs an action every [1 to actionRate] seconds
    private float rngCounter = 0f;
    private Vector2 centerPos = Vector2.zero;
    private float attackTimer;          // Used for attack timings (such as delays or charges)
    private int attackNum = 0;          // Determines which attack is used
    private int attackStep = 1;         // Current step of the current attack
    private int lastAttack = 0;         // The last attack performed (prevents repeat attacks)
    private bool overdrive = false;
    public bool attackInOrder = false;

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject[] bullets;

    private bool bulletRainOn = false;
    public float bulletRainRate = 0.2f;
    private float bulletRainTimer = 0f;
    private Vector2 lastPosition;
    private float fireRateTimer = 0f;   // Used for spiral attack
    private float bulletOffset = 0f;    // Used for spiral attack
    float wFireRateTimer = 0f;          // Used for waterfall attack


    public GameObject dangerIndicator;
    GameObject laserIndicator;
    GameObject danger;
    Vector3 delayedPos = Vector2.zero;

    new void Start()
    {
        base.Start();

        lastAttack = 1;
        rb = GetComponent<Rigidbody2D>();
        currentState = enemyState.idle;
        centerPos = Camera.main.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");

        bossHealthBar.SetBoss(this.gameObject, maxHealth, enemyName);
        bossHealthBar.SetBarColor(new Color(0.286f, 0.313f, 0.812f),
                                  new Color(0.600f, 0.907f, 1.000f),
                                  new Color(0.134f, 0.204f, 0.481f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.Update();
        //-----IDLE STATE-----
        if(currentState == enemyState.idle)
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

        // I'll try applying movement within the attacks this time
        *//*//-----MOVING STATE-----
        else if(currentState == enemyState.moving)
        {

        }

        //-----JUMPING STATE-----
        else if (currentState == enemyState.jumping)
        {

        }*//*

        //-----ATTACKING STATE-----
        else if (currentState == enemyState.attacking)
        {
            switch (attackNum)
            {
                //Attack 1: Jump past the player while shooting bullets downwards
                case 1:
                    if(attackStep == 1)
                    {
                        attackTimer = 1f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2((Mathf.Abs(transform.position.x - player.transform.position.x)) * 60 * direction, jumpForce));
                            AudioManager.Instance.Play("Jump");
                            attackStep = 3;
                        }
                    }
                    if(attackStep == 3)
                    {
                        if(!grounded && rb.velocity.y < 0f)
                        {
                            rb.velocity = Vector2.zero;
                            rb.AddForce(new Vector2(100 * direction, 300));
                            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;

                            GameObject bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(-1, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(0, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(1, -2);

                            AudioManager.Instance.Play("Bullet2");

                            if (difficulty >= 10)
                            {
                                attackStep = 4;
                            }
                            else
                            {
                                currentState = enemyState.idle;
                            }
                        }
                    }
                    if(attackStep == 4) //Jump one more time if at a higher difficulty
                    {
                        if (!grounded && rb.velocity.y < -4f)
                        {
                            rb.velocity = Vector2.zero;
                            rb.AddForce(new Vector2(100 * direction, 300));
                            AudioManager.Instance.Play("Jump");
                            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;

                            GameObject bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(-1, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(0, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(1, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(-2, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(2, -2);

                            AudioManager.Instance.Play("Bullet2");

                            if (difficulty >= 75) 
                            {
                                danger = Instantiate(dangerIndicator, new Vector2(-100, -100), Quaternion.identity);
                                danger.GetComponent<DangerIndicator>().lifeTime = 0.7f;
                                attackStep = 5;
                            }
                            else
                            {
                                currentState = enemyState.idle;
                            }
                        }
                    }
                    if (attackStep == 5) //Shoot a laser downwards at even higher difficulties
                    {
                        //Show indicator
                        if (danger != null)  //Prevents MissingReferenceException
                            danger.transform.position = new Vector3(transform.position.x, -1.5f);

                        if (!grounded && rb.velocity.y < -5f)
                        {
                            rb.velocity = Vector2.zero;
                            rb.AddForce(new Vector2(100 * direction, 400));
                            AudioManager.Instance.Play("Jump");

                            GameObject laser = Instantiate(bullets[2], transform.position, Quaternion.identity);
                            laser.GetComponent<GuraLaser>().SetPositions(Vector2.zero, Vector2.down * 10f);
                            laser.GetComponent<GuraLaser>().SetLifeTime(1f);

                            currentState = enemyState.idle;
                        }
                    }
                    break;

                //Attack 2: Lob a few projectiles at the player, when projectiles land, they create a damaging geyser from the floor
                case 2:
                    if(attackStep == 1)
                    {
                        attackTimer = 0.5f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            //Fire a few bullets towards player (Replace with geyser bullets later)
                            GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                            bullet.GetComponent<PhysicsBullet>().SetForce(600f);
                            bullet.GetComponent<PhysicsBullet>().SetDirection(10 * direction, 80f);
                            bullet.GetComponent<PhysicsBullet>().geyser = true;

                            for (int i = 0; i < 4 + Mathf.Clamp(difficulty / 20, -2, 10); i++)
                            {
                                bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(400f, 1000f));
                                bullet.GetComponent<PhysicsBullet>().SetDirection(10 * direction, Random.Range(20f, 50f));
                                bullet.GetComponent<PhysicsBullet>().geyser = true;
                            }

                            AudioManager.Instance.Play("Bullet2");

                            currentState = enemyState.idle;
                        }
                    }
                    break;

                //Attack 3: Floats in the air and shoots bullets in a sprial
                case 3:
                    if(attackStep == 1)
                    {
                        attackTimer = 0f;
                        attackStep = 2;
                        //bulletOffset = 0f;
                        lastPosition = transform.position;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer += Time.deltaTime;
                        float t = attackTimer / 2f;
                        t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                        transform.position = Vector2.Lerp(lastPosition, centerPos + new Vector2(0, 3), t);

                        if(attackTimer >= 2)
                        {
                            attackStep = 3;
                            attackTimer = 4f;
                        }
                    }
                    if(attackStep == 3)
                    {
                        attackTimer -= Time.deltaTime;

                        //Shoot spiral bullets
                        float fireRate = 0.3f - Mathf.Clamp((difficulty / 200f), 0, 0.2f);   //difficulty alters fireRate
                        int density = 4 + Mathf.Clamp((int)(Mathf.Log10(Mathf.Abs(difficulty)) * 2), -1, 20);      //and density
                        if (difficulty < 0)
                        {
                            density = 4 + Mathf.Clamp((int)-(Mathf.Log10(Mathf.Abs(difficulty)) * 2), -1, 20);      //and density
                        }
                        float offsetRate = 0.5f - Mathf.Clamp((difficulty / 200), 0, 0.3f); //and offsetRate

                        if (fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
                        else
                        {
                            for (int i = 0; i < density; i++)
                            {
                                float angle = (i * Mathf.PI * 2 / density) + bulletOffset;
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = -angle * Mathf.Rad2Deg;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[1], pos, rot);
                                bullet.GetComponent<NormalBulletNoFollow>().SetDirection(x, y);
                                fireRateTimer = fireRate;
                            }
                            bulletOffset += offsetRate;

                            AudioManager.Instance.Play("Bullet2");
                        }

                        if (attackTimer <= 0)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                            currentState = enemyState.idle;
                        }
                    }
                    break;

                //Attack 4: Jump and shoot 3 lasers in a row at the player, the last laser creates bullets on impact
                case 4:
                    if(attackStep == 1) //Jump
                    {
                        rb.AddForce(new Vector2(0, jumpForce));
                        AudioManager.Instance.Play("Jump");

                        attackTimer = 0.5f;
                        attackStep = 2;
                    }
                    if(attackStep == 2) //Freeze position
                    {
                        attackTimer -= Time.deltaTime;

                        if(rb.velocity.y <= 0.1f && attackTimer <= 0)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                            attackTimer = 1f - Mathf.Clamp((difficulty / 200), -1f, 0.7f); ;   //laser charge time
                            attackStep = 3;

                            laserIndicator = Instantiate(bullets[2], transform.position, Quaternion.identity);
                            laserIndicator.GetComponent<GuraLaser>().indicator = true;
                            laserIndicator.GetComponent<GuraLaser>().lifeTime = 5f;
                            delayedPos = player.transform.position;
                        }
                    }
                    if(attackStep == 3)    
                    {
                        attackTimer -= Time.deltaTime;

                        //laser indicator follows player
                        Vector3 smoothedPos = Vector3.Lerp(delayedPos, player.transform.position, (2f + Mathf.Clamp((difficulty/50), -1f, 2f)) * Time.deltaTime);
                        delayedPos = smoothedPos;
                        laserIndicator.GetComponent<GuraLaser>().SetPositions(Vector2.zero, (delayedPos - transform.position) * 3f);

                        if (attackTimer <= 0)
                        {
                            //Fire 1st laser
                            GameObject laser = Instantiate(bullets[2], transform.position, Quaternion.identity);
                            laser.GetComponent<GuraLaser>().SetPositions(Vector2.zero, (delayedPos - transform.position) * 3f);
                            laser.GetComponent<GuraLaser>().SetLifeTime(1f);

                            attackTimer = 1f - Mathf.Clamp((difficulty / 200), -1f, 0.7f); ;   //laser charge time
                            attackStep = 4;
                        }
                    }
                    if(attackStep == 4)
                    {
                        attackTimer -= Time.deltaTime;

                        //laser indicator follows player
                        Vector3 smoothedPos = Vector3.Lerp(delayedPos, player.transform.position, (2f + Mathf.Clamp((difficulty / 50), -1f, 2f)) * Time.deltaTime);
                        delayedPos = smoothedPos;
                        laserIndicator.GetComponent<GuraLaser>().SetPositions(Vector2.zero, (delayedPos - transform.position) * 3f);

                        if (attackTimer <= 0)
                        {
                            //Fire 2nd laser
                            GameObject laser = Instantiate(bullets[2], transform.position, Quaternion.identity);
                            laser.GetComponent<GuraLaser>().SetPositions(Vector2.zero, (delayedPos - transform.position) * 3f);
                            laser.GetComponent<GuraLaser>().SetLifeTime(1f);

                            attackTimer = 2f - Mathf.Clamp((difficulty / 100), -1f, 1.5f);   //laser charge time
                            attackStep = 5;
                        }
                    }
                    if (attackStep == 5)
                    {
                        attackTimer -= Time.deltaTime;

                        //laser indicator follows player
                        Vector3 smoothedPos = Vector3.Lerp(delayedPos, player.transform.position, (2f + Mathf.Clamp((difficulty / 50), -1f, 2f)) * Time.deltaTime);
                        delayedPos = smoothedPos;
                        laserIndicator.GetComponent<GuraLaser>().SetPositions(Vector2.zero, (delayedPos - transform.position) * 3f);

                        if (attackTimer <= 0)
                        {
                            //Fire last laser
                            GameObject laser = Instantiate(bullets[2], transform.position, Quaternion.identity);
                            laser.GetComponent<GuraLaser>().SetPositions(Vector2.zero, (delayedPos - transform.position) * 3f);
                            laser.GetComponent<GuraLaser>().SetLifeTime(2f);
                            laser.GetComponent<GuraLaser>().SetWidth(4f);

                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                            currentState = enemyState.idle;
                            Destroy(laserIndicator);
                        }
                    }
                    break;

                //Attack 5: Dives underwater (under arena) then pop out after a delay with bullets spraying everywhere
                case 5:
                    if(attackStep == 1) //Short hop and dive under
                    {
                        rb.velocity = Vector2.zero;
                        rb.AddForce(new Vector2(25f * direction, 400f));
                        AudioManager.Instance.Play("Jump");
                        GetComponent<BoxCollider2D>().enabled = false;
                        comboMeter.SetStop(true);

                        attackTimer = 4f - Mathf.Clamp((difficulty / 20), -1, 1.5f);

                        danger = Instantiate(dangerIndicator, new Vector2(-100, -100), Quaternion.identity);
                        danger.GetComponent<DangerIndicator>().lifeTime = attackTimer;

                        attackStep = 2;
                    }
                    if(attackStep == 2) //Jump back up after a short delay
                    {
                        attackTimer -= Time.deltaTime;

                        //show indicator for attack
                        if(transform.position.y <= -2f)
                        {
                            Vector3 smoothedPos = Vector3.Lerp(delayedPos, player.transform.position, 1f * Time.deltaTime);
                            delayedPos = smoothedPos;
                            if(danger != null)  //Prevents MissingReferenceException
                                danger.transform.position = new Vector3(delayedPos.x, -1.5f);
                        }

                        if (attackTimer <= 0)
                        {
                            transform.position = new Vector2(delayedPos.x, -6f);
                            rb.velocity = Vector2.zero;
                            rb.AddForce(Vector2.up * 1000f);

                            attackStep = 3;
                        }
                    }
                    if(attackStep == 3) //re-enable collider
                    {
                        if(transform.position.y >= -1f)
                        {
                            //Spray bullets like a fountain
                            for (int i = 0; i < 20 + Mathf.Clamp((difficulty / 5), -10, 20); i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().SetDirection(Random.Range(-1.5f, 1.5f), Random.Range(2f, 3f));
                                bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(600f, 900f));
                            }

                            GetComponent<BoxCollider2D>().enabled = true;
                            comboMeter.SetStop(false);
                            currentState = enemyState.idle;

                            AudioManager.Instance.Play("Jump");
                        }
                    }
                    break;

                //Attack 6: Causes waterfalls (vertical bullets) to fall from the ceiling
                case 6:
                    if(attackStep == 1){    //Set up attack
                        attackTimer = 1f;
                        attackStep = 2;
                    }
                    if(attackStep == 2){    //Charge up attack
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0){
                            attackTimer = 2f;
                            attackStep = 3;
                        }
                    }
                    if(attackStep == 3){    //Fire bullets into air
                        attackTimer -= Time.deltaTime;
                        
                        float wFireRate = 0.05f;
                        if(wFireRateTimer > 0) wFireRateTimer -= Time.deltaTime;
                        else{
                            GameObject bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 20f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(Random.Range(0f, 7f) * direction, 20f);

                            wFireRateTimer = wFireRate;  //Fire Rate

                            AudioManager.Instance.Play("Bullet1");
                        }

                        if(attackTimer <= 0){   //Show Floor Indicators
                            attackTimer = 1.5f;
                            
                            float gapSize = 3f - Mathf.Clamp(difficulty/150f, -1.5f, 1.75f);
                            for(int i = -20; i < 20; i++){
                                danger = Instantiate(dangerIndicator, new Vector2(player.transform.position.x + i*gapSize, -1.5f), Quaternion.identity);
                                danger.GetComponent<DangerIndicator>().lifeTime = attackTimer;
                            }
                            delayedPos = new Vector2(player.transform.position.x, 8f);

                            attackStep = 4;
                        }
                    }
                    if(attackStep == 4){       //Wait for Ceiling Indicators
                        attackTimer -= Time.deltaTime;
                        
                        if(attackTimer <= 0){
                            attackTimer = 2f;
                            attackStep = 5;
                        }
                    }
                    if(attackStep == 5){        //Waterfall attack
                        attackTimer -= Time.deltaTime;

                        float wFireRate = 0.05f;
                        float gapSize = 3f - Mathf.Clamp(difficulty / 150f, -1f, 2f); //should be equal to the last step

                        if (wFireRateTimer > 0) wFireRateTimer -= Time.deltaTime;
                        else{
                            for(int i = -10; i < 10; i++){
                                GameObject bullet = Instantiate(bullets[1], new Vector2(delayedPos.x + i*gapSize, delayedPos.y + 2f), Quaternion.identity);
                                bullet.GetComponent<NormalBulletNoFollow>().speed = 15f;
                                bullet.GetComponent<NormalBulletNoFollow>().SetDirection(0, -1);
                                bullet.GetComponent<NormalBulletNoFollow>().lifeTime = 1.5f;
                            }
                            wFireRateTimer = wFireRate;
                        }

                        if(attackTimer <= 0){
                            currentState = enemyState.idle;
                        }
                    }
                    break;

                //[Overdrive] Attack 7: Combines waterfall attack and laser attack
                case 7:

                    break;

                //[Overdrive] Attack 10 (one time only): Causes the stage to rain bullets from the ceiling at random spots until boss is defeated
                case 10:
                    if(attackStep == 1)
                    {
                        attackTimer = 2f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            bulletRainOn = true;
                            currentState = enemyState.idle;
                        }
                    }
                    break;
            }
        }

        //-----DYING STATE------
        else if (currentState == enemyState.dying)
        {
            rb.velocity = Vector2.zero;
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(Random.Range(200f, 400f) * -direction, Random.Range(200f, 400f)));
            GetComponent<BoxCollider2D>().enabled = false;
            currentState = enemyState.dead;
        }

        //-----DEAD STATE-----
        else if (currentState == enemyState.dead)
        {
            tag = "Untagged";
            Destroy(this.gameObject, 2.5f);
        }


        //Counters and Timers
        //Random Counters and Timers
        if (grounded && rngCounter <= 0 && !stunned)
        {
            rngCounter = Mathf.Clamp(Random.Range(minActionRate - (difficulty / 100f), maxActionRate - (difficulty / 100f)), 0.2f, 10f);  //from 1 to [actionTimer] seconds
            currentState = enemyState.attacking;
            attackTimer = 1f;

            //weighted RNG for attacks
            int RNG = Random.Range(1, 7);
            if (GetCurrentHealth() > maxHealth * 0.5)    //above 50% HP
            {
                *//*switch (RNG)
                {
                    case <= 0.4f:
                        attackNum = 1;
                        break;
                    case <= 0.6f:
                        attackNum = 2;
                        break;
                    case <= 0.8f:
                        attackNum = 3;
                        break;
                    default: 
                        attackNum = 4;
                        break;
                }*//*
                if(attackInOrder){
                    attackNum = Mathf.Clamp((attackNum + 1) % 7, 1, 6);
                }
                else{
                    attackNum = RNG;
                    while (attackNum == lastAttack)
                    {
                        attackNum = Random.Range(1, 7);
                    }
                    lastAttack = attackNum;
                }
            }
            else                                        //below 50% hp (overdrive)
            {
                *//*switch (RNG)
                {
                    case <= 0.4f:
                        attackNum = 1;
                        break;
                    case <= 0.6f:
                        attackNum = 2;
                        break;
                    case <= 0.8f:
                        attackNum = 3;
                        break;
                    default:
                        attackNum = 4;
                        break;
                }*//*
                if(attackInOrder){
                    attackNum = Mathf.Clamp((attackNum + 1) % 7, 1, 6);
                }
                else{
                    attackNum = RNG;
                    while(attackNum == lastAttack)
                    {
                        attackNum = Random.Range(1, 7);
                    }
                    lastAttack = attackNum;
                }
                
                if (!overdrive)
                {
                    overdrive = true;
                    difficulty += 10;
                }
                if (!bulletRainOn)
                {
                    attackNum = 10;
                }
            }
            //attackNum = 5;  //Debug for testing specific attacks

            attackStep = 1;                     //Reset attack step to 1

            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;  //enemy faces towards player upon landing
        }

        //Bullet Rain
        if (bulletRainOn && currentState != enemyState.dead)
        {
            bulletRainTimer -= Time.deltaTime;

            if(bulletRainTimer <= 0)
            {
                GameObject bullet = Instantiate(bullets[0], new Vector2(Random.Range(centerPos.x-11, centerPos.x+11), centerPos.y+6.5f), Quaternion.identity);
                bullet.GetComponent<PhysicsBullet>().SetDirection(0, -1);
                bullet.GetComponent<PhysicsBullet>().SetGravity(0.3f);

                if (difficulty > 100)
                {
                    bullet.GetComponent<PhysicsBullet>().geyser = true;
                    bulletRainTimer = bulletRainRate + 0.1f;
                }
                else if(difficulty < 20)
                {
                    bulletRainTimer = bulletRainRate + 0.2f;
                }
                else
                {
                    bulletRainTimer = bulletRainRate;
                }
            }
        }
    }

*//*
    private void OnBecameVisible()
    {
        enabled = true;
    }
    private void OnBecameInvisible()
    {
        enabled = false;
    }
*//*
}

*/