using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{
    public float duration;
    public float strength;
    public int vibrato;
    public float randomness;


    void Start()
    {
        
    }

    private void Update()
    {
        //Test Shake
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Shake();
        }
    }

    void Shake()
    {
        Camera.main.DOShakePosition(duration, strength, vibrato, randomness, true, ShakeRandomnessMode.Harmonic);
    }

    void Shake(float duration, float strength, int vibrato, float randomness)
    {
        Camera.main.DOShakePosition(duration, strength, vibrato, randomness, true, ShakeRandomnessMode.Harmonic);
    }
}
