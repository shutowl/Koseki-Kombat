using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;
    public float lifeTime = 3f;
    public int damage = 10;
    public float hitstun = 0.5f;
    Rigidbody2D rb;

    [Header("Physics Variables")]
    bool physicsOn = false;
    private float force = 100f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * speed;

        if (physicsOn)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(direction * force);
        }

        Destroy(this.gameObject, lifeTime);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetDirection(float x, float y)
    {
        direction = new Vector2(x, y);
    }

    public void SetForce(float f)
    {
        force = f;
    }

    public void SetGravity(float gravity)
    {
        rb.gravityScale = gravity;
    }

    public void EnablePhysics()
    {
        physicsOn = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hitbox"))
        {
            col.GetComponentInParent<PlayerHealth>().Damage(damage, hitstun);
        }
    }
}
