using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutsceneController : MonoBehaviour
{
    public GameObject player;
    PlayerControls playerScript;
    public GameObject playerHealthBar;
    Sequence DOTmovePlayerUI;

    public GameObject baelz;
    public GameObject bossHealthBar;
    Sequence DOTmoveBossUI;

    public Vector2 playerStartPos;
    public Vector2 playerEndPos;
    Sequence DOTmovePlayer;
    int scene = 0;

    void Start()
    {
        playerScript = player.GetComponent<PlayerControls>();
        playerScript.curState = PlayerControls.playerState.inCutscene;
        //also get baelz script

        //Player starts offscreen and runs in
        player.transform.position = playerStartPos;
        DOTmovePlayer = DOTween.Sequence();
        DOTmovePlayer.Append(player.transform.DOMove(playerEndPos, 4).SetEase(Ease.OutSine));
    }

    void Update()
    {
        if(scene == 0)
        {
            if(player.transform.position.x >= playerEndPos.x)
            {
                scene = 1;
            }
        }
        if(scene == 1)
        {
            //Open Dialogue boxes

            playerScript.curState = PlayerControls.playerState.moving;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            ShowUI();

            scene = 2;
        }

    }

    void HideUI()
    {
        DOTmovePlayerUI = DOTween.Sequence();
        RectTransform playerUIRect = playerHealthBar.GetComponent<RectTransform>();
        DOTmovePlayerUI.Append(playerUIRect.DOAnchorPosY(Mathf.Abs(playerUIRect.anchoredPosition.y), 1f).SetEase(Ease.OutCubic));

        DOTmoveBossUI = DOTween.Sequence();
        RectTransform bossUIRect = bossHealthBar.GetComponent<RectTransform>();
        DOTmoveBossUI.Append(bossUIRect.DOAnchorPosY(-Mathf.Abs(bossUIRect.anchoredPosition.y), 1f).SetEase(Ease.OutCubic));
    }

    void ShowUI()
    {
        DOTmovePlayerUI = DOTween.Sequence();
        RectTransform playerUIRect = playerHealthBar.GetComponent<RectTransform>();
        DOTmovePlayerUI.Append(playerUIRect.DOAnchorPosY(-Mathf.Abs(playerUIRect.anchoredPosition.y), 1f).SetEase(Ease.OutCubic));

        DOTmoveBossUI = DOTween.Sequence();
        RectTransform bossUIRect = bossHealthBar.GetComponent<RectTransform>();
        DOTmoveBossUI.Append(bossUIRect.DOAnchorPosY(Mathf.Abs(bossUIRect.anchoredPosition.y), 1f).SetEase(Ease.OutCubic));
    }
}
