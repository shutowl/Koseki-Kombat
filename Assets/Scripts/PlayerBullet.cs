using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * speed;

        Destroy(this.gameObject, 2f);
    }

    public void SetDirection(float x, float y)
    {
        direction = new Vector2(x, y);

        if(y > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
    }
}
