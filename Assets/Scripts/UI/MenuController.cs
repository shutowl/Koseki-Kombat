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
        options
    }
    public menuState state;

    private int menuIndex = 0;
    public GameObject[] arrows; //Left = 0, Right = 1
    public Vector2[] arrowPositions;
    public GameObject mainWindow;
    public GameObject optionsWindow;

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
        //Play Music
    }

    private void Update()
    {
        if (input.UI.Confirm.WasPressedThisFrame())
        {
            if(state == menuState.options)
            {

            }
            else if(state == menuState.main)
            {
                if(menuIndex == 0)
                {
                    AudioManager.Instance.Play("MenuSelect");
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
                AudioManager.Instance.Play("MenuMove");
            }
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
                AudioManager.Instance.Play("MenuMove");
            }
        }

        if(gameStarting && screenFadeTimer > 0)
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
        menuIndex = 0;
    }

    void CloseOptions()
    {
        state = menuState.main;
        optionsWindow.SetActive(false);
        mainWindow.SetActive(true);
        menuIndex = 1;
    }
}
