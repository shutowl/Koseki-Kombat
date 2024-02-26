using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFlash : MonoBehaviour
{
    public float flashRate = 0.2f;
    float flashTimer;
    public bool on;
    bool visible;

    private void Start()
    {
        visible = true;
    }

    void Update()
    {
        if (on)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0)
            {
                if (visible)
                {
                    visible = false;
                    gameObject.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 90, gameObject.GetComponent<RectTransform>().localEulerAngles.z);
                }
                else
                {
                    visible = true;
                    gameObject.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, gameObject.GetComponent<RectTransform>().localEulerAngles.z);
                }

                flashTimer = flashRate;
            }
        }
    }
}
