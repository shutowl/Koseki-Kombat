using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class BossHealthBar : MonoBehaviour
{
    private GameObject boss;
    RectTransform rect;
    public float rectDuration = 2f;
    private float rectTimer = 0f;
    private float rectTimer2 = 0f;
    public float rectYShown;            //Y position of health bar when boss is on screen
    public float rectYHidden;           //Y position of health bar when boss is off screen


    public Slider HPSlider;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI bossHealthText;
    public float HPDuration = 1f;
    private float HPTimer = 0f;
    private float lastHP = 0;

    public Image borderImage;
    public Image fillImage;


    void Start()
    {
        rect = GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -100);
        rectTimer2 = 10;

        bossHealthText.enabled = false;
    }

    void Update()
    {
        if(boss != null)
        {
            if(rectTimer < rectDuration) rectTimer += Time.deltaTime;
            float t = rectTimer / rectDuration;
            t = 1 - Mathf.Pow(1 - t, 3);
            float y = Mathf.Lerp(rectYHidden, rectYShown, t);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);   //show boss bar 

            rectTimer2 = 0f;
        }
        else
        {
            if (rectTimer2 < rectDuration) rectTimer2 += Time.deltaTime;
            float t = rectTimer2 / rectDuration;
            t = 1 - Mathf.Pow(1 - t, 3);
            float y = Mathf.Lerp(rectYShown, rectYHidden, t);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y); //hide boss bar
        }
        

        if(HPSlider.value <= 0)
        {
            bossHealthText.enabled = false;
        }

        //HP Text Lerp
        if(lastHP != HPSlider.value)
        {
            if(HPTimer < HPDuration) HPTimer += Time.deltaTime;
            float t = HPTimer / HPDuration;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            int hp = Mathf.RoundToInt(Mathf.Lerp(lastHP, HPSlider.value, t));
            bossHealthText.text = "" + hp;
        }
        else
        {
            lastHP = HPSlider.value;
            HPTimer = 0f;
        }
    }

    public void SetBoss(GameObject boss, float maxHealth, string name)
    {
        this.boss = boss;

        HPSlider.maxValue = maxHealth;
        HPSlider.value = maxHealth;
        lastHP = HPSlider.value;
        bossNameText.text = name;


        Debug.Log("Boss set to: " + boss);

        rectTimer = 0;
    }

    public void SetBarColor(Color border, Color fill, Color delay)
    {
        borderImage.color = border;
        fillImage.color = fill;
    }

    public void SetHP(float health)
    {
        HPTimer = 0;
        lastHP = HPSlider.value;
        HPSlider.value = health;

        //Show text when below a certain percentage of health
        if(health < HPSlider.maxValue * 0.5f)
        {
            bossHealthText.enabled = true;
        }
    }

    //Custom Inspector button to check bar colors
    public void PrintColors()
    {
        Debug.Log("Border: " + borderImage.color);
        Debug.Log("Fill: " + fillImage.color);
    }
}
/*
[CustomEditor(typeof(BossHealthBar))]
public class BossHealthBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BossHealthBar bossHealthBar = (BossHealthBar)target;
        if (GUILayout.Button("Print Colors"))
        {
            bossHealthBar.PrintColors();
        }
    }
}
*/