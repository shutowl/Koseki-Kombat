using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class TextHover : MonoBehaviour
{
    public float duration = 3f;
    public float distance = 1f;

    public GameObject textBox;
    Sequence DOThover;

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();

        DOThover = DOTween.Sequence();
        DOThover.Append(rect.DOAnchorPosY(rect.anchoredPosition.y + distance, duration / 4).SetEase(Ease.OutSine));
        DOThover.Append(rect.DOAnchorPosY(rect.anchoredPosition.y - distance, duration / 2).SetEase(Ease.InOutSine));
        DOThover.Append(rect.DOAnchorPosY(rect.anchoredPosition.y, duration / 4).SetEase(Ease.InSine));
        DOThover.SetLoops(-1).SetUpdate(true);
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);
    }
}
