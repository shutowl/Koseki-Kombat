using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    public GameObject player;
    PlayerControls playerScript;
    public GameObject playerHealthBar;
    Sequence DOTmovePlayerUI;

    public GameObject baelz;
    BaelzControls baelzScript;
    GameObject baelzPrefab;
    public GameObject bossHealthBar;
    Sequence DOTmoveBossUI;
    Sequence DOThover;

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
    public Image portrait;
    public Sprite[] portraits;

    private InputActions inputActions;

    void Start()
    {
        inputActions = new InputActions();
        inputActions.UI.Enable();

        HideUI();

        playerScript = player.GetComponent<PlayerControls>();
        playerScript.curState = PlayerControls.playerState.inCutscene;
        baelzScript = baelz.GetComponent<BaelzControls>();
        baelzScript.curState = BaelzControls.enemyState.inCutscene;

        //Player starts offscreen and runs in
        player.transform.position = playerStartPos;
        DOTmovePlayer = DOTween.Sequence();
        DOTmovePlayer.Append(player.transform.DOMove(playerEndPos, scene1Duration).SetEase(Ease.OutSine));

        DOThover = DOTween.Sequence();

        player.GetComponent<Animator>().Play("Run");
    }

    void Update()
    {
        //Bijou runs in from the left side
        if(scene == 0)
        {
            if(player.transform.position.x >= playerEndPos.x)
            {
                scene = 1;
                sceneTimer = 1.25f;

                dialogueText.text = "";
                speakerText.text = "Koseki Bijou";
                portrait.sprite = portraits[0];
                ShowDialogue();

                player.GetComponent<Animator>().Play("Idle");
            }
        }
        //Wait for dialogue to show and setup next dialogue line
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
        //Bijou says "“HAKOS BAELZ, I know you’re here! Show yourself!”
        else if (scene == 2)
        {
            textTimer -= Time.deltaTime;
            if(textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BijouText");
                }
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
                    portrait.sprite = portraits[3];
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Baelz says “...Heh, so you’ve finally arrived, Koseki Bijou.”
        else if (scene == 3)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BaelzText");
                }

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
                baelzPrefab = Instantiate(baelz, baeSpawnPoint.transform.position, Quaternion.identity);
                baelzPrefab.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePosition;
                DOThover = DOTween.Sequence();
                DOThover.Append(baelzPrefab.transform.DOMoveY(baeSpawnPoint.transform.position.y - 1, 2).SetEase(Ease.InOutSine));
                DOThover.Append(baelzPrefab.transform.DOMoveY(baeSpawnPoint.transform.position.y, 2).SetEase(Ease.InOutSine));
                DOThover.SetLoops(-1);

                sceneTimer = 2f;
                scene = 5;
            }
        }
        else if(scene == 5)
        {
            sceneTimer -= Time.deltaTime;

            if(sceneTimer <= 0)
            {
                sceneTimer = 1.25f;
                scene = 6;
                ShowDialogue();
                dialogueText.text = "";
                portrait.sprite = portraits[4];
            }
        }
        //Short delay before dialogue again
        else if(scene == 6)
        {
            sceneTimer -= Time.deltaTime;

            if(sceneTimer <= 0)
            {
                nextLine = "This steel rose has been awaiting you,****** my sparkling gem.";
                textSpeed = 0.05f;
                textTimer = 0;
                dialogueText.text = "";

                speakerText.text = "Hakos Baelz";

                scene = 7;
            }
        }
        //Bae says "This steel rose has been awaiting you, my sparkling gem."
        else if (scene == 7)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BaelzText");
                }

                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }

            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    nextLine = "The world will soon plummet into chaos,****** with you as its last beacon of hope.";
                    textSpeed = 0.05f;
                    textTimer = 0;
                    dialogueText.text = "";

                    speakerText.text = "Hakos Baelz";

                    scene = 8;
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Bae says "The world will soon plummet into chaos, with you as its last beacon of hope"
        else if (scene == 8)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BaelzText");
                }

                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }

            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    nextLine = "Heh,**** just try it!******* I could care less what happens to the world.";
                    textSpeed = 0.05f;
                    textTimer = 0;
                    dialogueText.text = "";

                    speakerText.text = "Koseki Bijou";
                    portrait.sprite = portraits[1];

                    scene = 9;
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Bijou says "Heh, just try it! I could care less what happens to the world."
        else if (scene == 9)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BijouText");
                }

                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }

            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    nextLine = "I’m just here to settle our score!";
                    textSpeed = 0.05f;
                    textTimer = 0;
                    dialogueText.text = "";

                    speakerText.text = "Koseki Bijou";
                    portrait.sprite = portraits[2];

                    scene = 10;
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Bijou says "I'm just here to settle our score"
        else if(scene == 10)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BijouText");
                }

                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }


            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    nextLine = "HAH!******** THEN WITNESS ME!!";
                    textSpeed = 0.04f;
                    textTimer = 0;
                    dialogueText.text = "";

                    speakerText.text = "Hakos Baelz";
                    portrait.sprite = portraits[4];

                    scene = 11;

                    AudioManager.Instance.PlayMusic("PSYCHO");
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Baelz says "HAH, THEN WITNESS ME!!!"
        else if(scene == 11)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0 && !nextLine.Equals(""))
            {
                if (nextLine[0] != '*')
                {
                    dialogueText.text += nextLine[0];
                    AudioManager.Instance.Play("BaelzText");
                }

                nextLine = nextLine.Remove(0, 1);
                textTimer = textSpeed;
            }

            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                if (nextLine.Equals(""))
                {
                    HideDialogue();
                    ShowUI();

                    sceneTimer = 1f;
                    scene = 12;
                }
                else
                {
                    dialogueText.text += nextLine.Replace("*", "");
                    nextLine = "";
                }
            }
        }
        //Hide dialogue, show UI, then short delay
        else if(scene == 12)
        {
            sceneTimer -= Time.deltaTime;

            if (sceneTimer <= 0)
            {
                scene = 13;
            }
        }
        //Fight begins
        else if(scene == 13)
        {
            playerScript.curState = PlayerControls.playerState.moving;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            baelzScript.curState = BaelzControls.enemyState.idle;
            baelzPrefab.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            DOThover.Kill();

            scene = 14;
        }
        //Fight end
        else if(scene == 14)
        {
            /* if(baelz ded)
             * {
             *      scene = 15;
             * }
             */
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
