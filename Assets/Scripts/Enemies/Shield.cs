using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shield : MonoBehaviour
{
    public float maxDuration;
    float duration;
    public float flashDuration = 0.25f;
    public float endAlpha = 0.25f;
    SpriteRenderer spriteRend;
    Sequence DOTflash;

    void Start()
    {
        DOTflash = DOTween.Sequence();
        spriteRend = GetComponent<SpriteRenderer>();
        DOTflash.Append(spriteRend.DOColor(new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, endAlpha), flashDuration).SetEase(Ease.InOutSine));
        DOTflash.Append(spriteRend.DOColor(new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 0), flashDuration).SetEase(Ease.InOutSine));
        DOTflash.SetLoops(-1);
    }

    private void Update()
    {
        if(duration > -1)
        {
            if (duration > 0)
            {
                duration -= Time.deltaTime;
            }
            else
            {
                spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 0);
                DOTflash.Pause();
            }
        }
    }

    public void ShieldOn(bool on, float duration)
    {
        if (on)
        {
            DOTflash.Play();
            this.duration = duration;
        }
        else
        {
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 0);
            DOTflash.Pause();
        }
    }
}
