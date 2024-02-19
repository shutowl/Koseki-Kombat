using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Dice : MonoBehaviour
{
    public float spawnDuration = 1f;
    public float rollDuration = 2f;
    float rollTimer;
    float delayTimer;
    public Sprite[] dieFaces;
    [SerializeField] int face = 1;  //from 1-6
    bool rolling;

    public float faceChangeRate = 0.2f;
    float faceChangeTimer = 0;

    public TextMeshProUGUI diceText;

    void Start()
    {
        rolling = true;
        diceText.text = "";

        transform.DORotate(new Vector3(0, 0, 360), spawnDuration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);
        RollDie(spawnDuration);
        rollTimer = spawnDuration + rollDuration;
        delayTimer = spawnDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (rolling)
        {
            if(faceChangeTimer <= 0 && delayTimer <= 0 && rollTimer >= 0.5f)
            {
                ChangeFace();
                faceChangeTimer = faceChangeRate;
            }
            else
            {
                faceChangeTimer -= Time.deltaTime;
            }
        }

        //Timers
        if (rollTimer <= 0)
        {
            rolling = false;
        }
        else
        {
            rollTimer -= Time.deltaTime;
        }

        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
    }



    public void RollDie(float delay)
    {
        rolling = true;
        transform.DORotate(new Vector3(0, 0, -1080), rollDuration, RotateMode.FastBeyond360).SetEase(Ease.OutQuint).SetDelay(delay);
        rollTimer = rollDuration;

        //Play dice roll sound
    }

    public void RollDie()
    {
        RollDie(0);
    }

    public void ChangeFace()
    {
        face = Random.Range(0, 6)+1;
        GetComponent<SpriteRenderer>().sprite = dieFaces[face-1];
    }

    public int GetFace()
    {
        return face;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!rolling && (col.CompareTag("PlayerBullet") || col.CompareTag("Player")))
        {
            RollDie();
        }
    }
}
