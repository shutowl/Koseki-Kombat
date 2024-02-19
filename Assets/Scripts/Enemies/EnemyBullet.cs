using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;
    public int damage = 10;
    public float hitstun = 0.5f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * speed;

        Destroy(this.gameObject, 4f);
    }


    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetDirection(float x, float y)
    {
        direction = new Vector2(x, y);
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hitbox"))
        {
            col.GetComponentInParent<PlayerHealth>().Damage(damage, hitstun);
        }
    }
}
