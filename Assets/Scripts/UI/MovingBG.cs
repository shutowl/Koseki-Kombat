using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBG : MonoBehaviour
{
    public float speed;
    public Vector2 direction = new Vector2(1, -3);

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction.normalized);

        if(transform.position.x >= direction.x * 10 && transform.position.y <= direction.y * 10)
        {
            transform.position = Vector2.zero;
        }
    }
}
