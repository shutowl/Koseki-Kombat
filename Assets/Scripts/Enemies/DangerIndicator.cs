using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerIndicator : MonoBehaviour
{
    public float flashRate = 0.5f;
    private float flashTimer = 0f;
    public float offDuration = 0.1f;
    private float offTimer = 0f;
    private SpriteRenderer sprite;
    public float lifeTime = 3f;
    private float lifeTimeTimer = 0f;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        lifeTimeTimer = lifeTime;

        flashRate = lifeTime / 5;
        offDuration = lifeTime / 30;
    }

    void Update()
    {
        //Sprite will flash every [flashRate] seconds for [offDuration] seconds
        flashTimer -= Time.deltaTime;
        if(flashTimer <= 0)
        {
            sprite.enabled = false;

            offTimer -= Time.deltaTime;
            if(offTimer <= 0)
            {
                sprite.enabled = true;
                offTimer = offDuration;
                flashTimer = flashRate;

                //flashRate decreases as lifeTime decreases
                flashRate = Mathf.Clamp(flashRate - lifeTime / 30, 0.1f, 5f);

                AudioManager.Instance.Play("Beep");
            }
        }

        lifeTimeTimer -= Time.deltaTime;
        if(lifeTimeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
