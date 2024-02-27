using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuController : MonoBehaviour
{
    public enum menuState
    {
        main,
        options,
        starting
    }
    public menuState state;

    private int menuIndex = 0;
    public GameObject[] arrows; //Left = 0, Right = 1
    public Vector2[] arrowPositions;
    public GameObject mainWindow;
    public GameObject optionsWindow;
    public GameObject startButton;

    public TextMeshProUGUI[] rightOptions;
    bool option1 = true;
    int bgmVol;
    int sfxVol;

    bool gameStarting = false;
    public Image screenFade;
    public float screenFadeDuration = 3f;
    float screenFadeTimer;


    private InputActions input;

    private void Awake()
    {
        input = new InputActions();
        input.UI.Enable();
        state = menuState.main;
        Time.timeScale = 1;

        arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[0].x, arrowPositions[0].y);
        arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[0].x, arrowPositions[0].y);
        menuIndex = 0;

        mainWindow.SetActive(true);
        optionsWindow.SetActive(false);

        gameStarting = false;
        screenFadeTimer = screenFadeDuration;

        option1 = (PlayerPrefs.GetInt("option1", 1) == 1);
        bgmVol = PlayerPrefs.GetInt("bgmVol", 4);
        sfxVol = PlayerPrefs.GetInt("sfxVol", 6);


        rightOptions[0].text = (option1) ? "On" : "Off";
        rightOptions[1].text = "";
        rightOptions[2].text = "";
        for (int i = 0; i < bgmVol; i++)
        {
            rightOptions[1].text += "I";
        }
        AudioManager.Instance.ChangeBGMVolume(bgmVol);
        for (int i = 0; i < sfxVol; i++)
        {
            rightOptions[2].text += "I";
        }
        AudioManager.Instance.ChangeSFXVolume(sfxVol);

        AudioManager.Instance.PlayMusic("MenuBGM");
    }

    private void Update()
    {
        if (input.UI.Confirm.WasPressedThisFrame())
        {
            if(state == menuState.options)
            {
                if(menuIndex == 0)
                {
                    option1 = !option1;
                    rightOptions[0].text = (option1) ? "On" : "Off";
                    AudioManager.Instance.PlayOneShot("Step" + Random.Range(1, 11));
                }
                else if(menuIndex == 3)
                {
                    AudioManager.Instance.Play("MenuSelect");
                    CloseOptions();
                }
            }
            else if(state == menuState.main)
            {
                if(menuIndex == 0)
                {
                    AudioManager.Instance.Play("MenuStart");
                    arrows[0].GetComponent<TextFlash>().on = true;
                    arrows[1].GetComponent<TextFlash>().on = true;
                    startButton.GetComponent<TextFlash>().on = true;

                    state = menuState.starting;
                    gameStarting = true;
                }
                else if(menuIndex == 1)
                {
                    AudioManager.Instance.Play("MenuSelect");
                    OpenOptions();
                }
            }
        }
        if (input.UI.Cancel.WasPressedThisFrame())
        {
            if(state == menuState.options)
            {
                AudioManager.Instance.Play("MenuCancel");
                CloseOptions();
            }
        }

        if (input.UI.Up.WasPressedThisFrame())
        {
            if (state == menuState.main)
            {
                if (menuIndex == 0)
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[1].x, arrowPositions[1].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[1].x, arrowPositions[1].y);
                    menuIndex = 1;
                }
                else if (menuIndex == 1)
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[0].x, arrowPositions[0].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[0].x, arrowPositions[0].y);
                    menuIndex = 0;
                }
            }
            if (state == menuState.options)
            {
                if (menuIndex == 0)  //Option 1
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[5].x, arrowPositions[5].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[5].x, arrowPositions[5].y);
                    menuIndex = 3;
                }
                else if (menuIndex == 1)     //BGM Vol
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[2].x, arrowPositions[2].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[2].x, arrowPositions[2].y);
                    menuIndex = 0;
                }
                else if (menuIndex == 2)    //SFX Vol
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[3].x, arrowPositions[3].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[3].x, arrowPositions[3].y);
                    menuIndex = 1;
                }
                else if (menuIndex == 3)    //Back
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[4].x, arrowPositions[4].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[4].x, arrowPositions[4].y);
                    menuIndex = 2;
                }
            }
            AudioManager.Instance.Play("MenuMove");
        }

        if (input.UI.Down.WasPressedThisFrame())
        {
            if (state == menuState.main)
            {
                if(menuIndex == 0)
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[1].x, arrowPositions[1].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[1].x, arrowPositions[1].y);
                    menuIndex = 1;
                }
                else if (menuIndex == 1)
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[0].x, arrowPositions[0].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[0].x, arrowPositions[0].y);
                    menuIndex = 0;
                }
            }
            if(state == menuState.options)
            {
                if(menuIndex == 0)  //Option 1
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[3].x, arrowPositions[3].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[3].x, arrowPositions[3].y);
                    menuIndex = 1;
                }
                else if(menuIndex == 1)     //BGM Vol
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[4].x, arrowPositions[4].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[4].x, arrowPositions[4].y);
                    menuIndex = 2;
                }
                else if (menuIndex == 2)    //SFX Vol
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[5].x, arrowPositions[5].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[5].x, arrowPositions[5].y);
                    menuIndex = 3;
                }
                else if (menuIndex == 3)    //Back
                {
                    arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[2].x, arrowPositions[2].y);
                    arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[2].x, arrowPositions[2].y);
                    menuIndex = 0;
                }
            }
            AudioManager.Instance.Play("MenuMove");
        }

        if (input.UI.Left.WasPressedThisFrame())
        {
            if(state == menuState.options)
            {
                if(menuIndex == 0)
                {
                    option1 = !option1;
                    rightOptions[0].text = (option1) ? "On" : "Off";
                    AudioManager.Instance.PlayOneShot("Step" + Random.Range(1, 11));
                }
                if(menuIndex == 1)  //BGM Vol
                {
                    bgmVol = Mathf.Clamp(--bgmVol, 0, 10);
                    rightOptions[1].text = "";
                    for (int i = 0; i < bgmVol; i++)
                    {
                        rightOptions[1].text += "I";
                    }
                    AudioManager.Instance.ChangeBGMVolume(bgmVol);
                }
                if(menuIndex == 2)  //SFX Vol
                {
                    sfxVol = Mathf.Clamp(--sfxVol, 0, 10);
                    rightOptions[2].text = "";
                    for (int i = 0; i < sfxVol; i++)
                    {
                        rightOptions[2].text += "I";
                    }
                    AudioManager.Instance.ChangeSFXVolume(sfxVol);
                    AudioManager.Instance.Play("MenuMove");
                }
            }
        }

        if (input.UI.Right.WasPressedThisFrame())
        {
            if (state == menuState.options)
            {
                if (menuIndex == 0)
                {
                    option1 = !option1;
                    rightOptions[0].text = (option1) ? "On" : "Off";
                    AudioManager.Instance.PlayOneShot("Step" + Random.Range(1, 11));
                }
                if (menuIndex == 1)  //BGM Vol
                {
                    bgmVol = Mathf.Clamp(++bgmVol, 0, 10);
                    rightOptions[1].text = "";
                    for (int i = 0; i < bgmVol; i++)
                    {
                        rightOptions[1].text += "I";
                    }
                    AudioManager.Instance.ChangeBGMVolume(bgmVol);
                }
                if (menuIndex == 2)  //SFX Vol
                {
                    sfxVol = Mathf.Clamp(++sfxVol, 0, 10);
                    rightOptions[2].text = "";
                    for (int i = 0; i < sfxVol; i++)
                    {
                        rightOptions[2].text += "I";
                    }
                    AudioManager.Instance.ChangeSFXVolume(sfxVol);
                    AudioManager.Instance.Play("MenuMove");
                }
            }
        }

        if (gameStarting && screenFadeTimer > 0)
        {
            screenFadeTimer -= Time.deltaTime;
            screenFade.color = new Color(screenFade.color.r, screenFade.color.r, screenFade.color.r, screenFade.color.a + (Time.deltaTime / screenFadeDuration));
        }
        if(screenFadeTimer <= 0)
        {
            StartGame();
        }
    }


    void StartGame()
    {
        SceneManager.LoadScene("Arena");
    }

    void OpenOptions()
    {
        state = menuState.options;
        mainWindow.SetActive(false);
        optionsWindow.SetActive(true);

        arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[2].x, arrowPositions[2].y);
        arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[2].x, arrowPositions[2].y);
        menuIndex = 0;
    }

    void CloseOptions()
    {
        state = menuState.main;
        optionsWindow.SetActive(false);
        mainWindow.SetActive(true);

        arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[0].x, arrowPositions[0].y);
        arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[0].x, arrowPositions[0].y);
        menuIndex = 0;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("option1", (option1) ? 1 : 0);
        PlayerPrefs.SetInt("bgmVol", bgmVol);
        PlayerPrefs.SetInt("sfxVol", sfxVol);
    }
}
