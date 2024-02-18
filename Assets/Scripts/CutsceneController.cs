using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    public GameObject player;
    PlayerControls playerScript;
    public GameObject playerHealthBar;
    Sequence DOTmovePlayerUI;

    public GameObject baelz;
    public GameObject bossHealthBar;
    Sequence DOTmoveBossUI;

    public float scene1Duration;
    public Vector2 playerStartPos;
    public Vector2 playerEndPos;
    Sequence DOTmovePlayer;

    public GameObject dialogueWindow;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    Sequence DOTmoveDialogue;
    string nextLine = "";
    float textSpeed;
    float textTimer = 0f;

    int scene = 0;
    float sceneTimer = 0f;

    public GameObject baeSpawnPoint;
    public GameObject explosion;

    private InputActions inputActions;

    void Start()
    {
        inputActions = new InputActions();
        inputActions.UI.Enable();

        playerScript = player.GetComponent<PlayerControls>();
        playerScript.curState = PlayerControls.playerState.inCutscene;
        //also get baelz script

        //Player starts offscreen and runs in
        player.transform.position = playerStartPos;
        DOTmovePlayer = DOTween.Sequence();
        DOTmovePlayer.Append(player.transform.DOMove(playerEndPos, scene1Duration).SetEase(Ease.OutSine));
    }

    void Update()
    {
        if(scene == 0)
        {
            if(player.transform.position.x >= playerEndPos.x)
            {
                scene = 1;
                sceneTimer = 1.25f;

                dialogueText.text = "";
                speakerText.text = "Koseki Bijou";
                ShowDialogue();
            }
        }
        if(scene == 1)
        {
            sceneTimer -= Time.deltaTime;

            if(sceneTimer <= 0)
            {
                scene = 2;

                nextLine = "HAKOS BAELZ!*************** " +
                            "I* *k*n*o*w* *y*o*u*'*r*e* *h*e*r*e*!*********** *S*h*o*w* *y*o*u*r*s*e*l*f*!";
                textSpeed = 0.02f;
                textTimer = 0;
                dialogueText.text = "";
            }
        }
        else if(scene == 2)
        {
            textTimer -= Time.deltaTime;
            if(textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                    dialogueText.text += nextLine[0];
                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }

            if(inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    scene = 3;

                    nextLine = ".*****.*****.*****Heh,***** so you’ve finally arrived, *****Koseki Bijou.";
                    textSpeed = 0.06f;
                    textTimer = 0;
                    dialogueText.text = "";

                    speakerText.text = "Hakos Baelz";
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        else if(scene == 3)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                    dialogueText.text += nextLine[0];
                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }

            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    HideDialogue();

                    sceneTimer = 1f;
                    scene = 4;
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Hide dialogue box then Bae spawns in
        else if(scene == 4)
        {
            sceneTimer -= Time.deltaTime;

            if(sceneTimer <= 0)
            {
                Instantiate(explosion, baeSpawnPoint.transform.position, Quaternion.identity);
                Instantiate(baelz, baeSpawnPoint.transform.position, Quaternion.identity);

                sceneTimer = 2f;
                scene = 5;
            }
        }
        else if(scene == 5)
        {
            sceneTimer -= Time.deltaTime;

            if(sceneTimer <= 0)
            {
                HideDialogue();
                scene = 20;
            }
        }
        else if(scene == 20)
        {
            playerScript.curState = PlayerControls.playerState.moving;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            ShowUI();

            scene = 21;
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

    void ShowDialogue()
    {
        DOTmoveDialogue = DOTween.Sequence();
        RectTransform dialogueRect = dialogueWindow.GetComponent<RectTransform>();
        DOTmoveDialogue.Append(dialogueRect.DOAnchorPosY(Mathf.Abs(dialogueRect.anchoredPosition.y), 1f).SetEase(Ease.OutCubic));
    }

    void HideDialogue()
    {
        DOTmoveDialogue = DOTween.Sequence();
        RectTransform dialogueRect = dialogueWindow.GetComponent<RectTransform>();
        DOTmoveDialogue.Append(dialogueRect.DOAnchorPosY(-Mathf.Abs(dialogueRect.anchoredPosition.y), 1f).SetEase(Ease.OutCubic));
    }
}
