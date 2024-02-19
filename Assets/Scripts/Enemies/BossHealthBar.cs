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
        HPSlider = GameObject.Find("BossHPSlider").GetComponent<Slider>();
        bossNameText = GameObject.Find("BossNameText").GetComponent<TextMeshProUGUI>();
        bossHealthText = GameObject.Find("BossHPText").GetComponent<TextMeshProUGUI>();

        SetBoss(this.gameObject, 3000, "Hakos Baelz");
        bossHealthText.enabled = true;

    }

    void Update()
    {
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


        //Debug.Log("Boss set to: " + boss);
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

    public void TakeDamage(int damage)
    {
        SetHP(Mathf.Clamp(HPSlider.value - damage, 0, 9999));
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