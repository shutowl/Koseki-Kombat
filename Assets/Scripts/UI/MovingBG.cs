using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBG : MonoBehaviour
{
    public float speed;
    public float resetTime = 10f;
    float resetTimer;
    public Vector2 direction = new Vector2(1, -3);

    private void Start()
    {
        transform.position = Vector2.zero;
        resetTimer = resetTime;
    }

    void Update()
    {
        transform.Translate(speed * Time.unscaledDeltaTime * direction.normalized);

        resetTimer -= Time.unscaledDeltaTime;
        if(resetTimer <= 0)
        {
            transform.position = Vector2.zero;
            resetTimer = resetTime;
        }
    }
}
