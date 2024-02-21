using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseWindow;
    bool paused;
    int menuIndex = 0;

    public GameObject[] arrows;
    public Vector2[] arrowPositions;

    InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
        inputActions.UI.Enable();
        arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[0].x, arrowPositions[0].y);
        arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[0].x, arrowPositions[0].y);

        pauseWindow.SetActive(false);
    }

    void Update()
    {
        if (inputActions.UI.Pause.WasPressedThisFrame())
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (paused)
        {
            if (inputActions.UI.Confirm.WasPressedThisFrame())
            {
                switch (menuIndex)
                {
                    case 0:
                        Resume();
                        break;
                    case 1:
                        Restart();
                        break;
                    case 2:
                        ExitToMenu();
                        break;
                }
            }

            if (inputActions.UI.Cancel.WasPressedThisFrame())
            {
                Resume();
            }

            if (inputActions.UI.Down.WasPressedThisFrame())
            {
                menuIndex++;

                if (menuIndex > 2) menuIndex = 0;

                arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[menuIndex].x, arrowPositions[menuIndex].y);
                arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[menuIndex].x, arrowPositions[menuIndex].y);
            }

            if (inputActions.UI.Up.WasPressedThisFrame())
            {
                menuIndex--;

                if (menuIndex < 0) menuIndex = 2;

                arrows[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowPositions[menuIndex].x, arrowPositions[menuIndex].y);
                arrows[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-arrowPositions[menuIndex].x, arrowPositions[menuIndex].y);
            }
        }
    }

    void Pause()
    {
        paused = true;
        pauseWindow.SetActive(true);
        Time.timeScale = 0f;
        menuIndex = 0;
    }

    void Resume()
    {
        paused = false;
        pauseWindow.SetActive(false);
        Time.timeScale = 1f;
    }

    void Restart()
    {
        Resume();
        SceneManager.LoadScene("Arena");
    }

    void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
