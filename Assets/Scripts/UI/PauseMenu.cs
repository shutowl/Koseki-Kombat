using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseWindow;

    void Awake()
    {
        pauseWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
