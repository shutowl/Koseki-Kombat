using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;
    private PlayerControls player;

    public TextMeshProUGUI healthText;
    public Slider HPSlider;
    public Slider delaySlider;
    public float delayDuration = 2f;
    private float delayTimer = 0f;
    private float timer = 0f;

    void Start()
    {
        player = gameObject.GetComponent<PlayerControls>();
        currentHealth = maxHealth;
        healthText.text = currentHealth + "";
        SetMaxHealth(maxHealth);
        FullHeal();
    }

    void Update()
    {
/*
        if (Keyboard.current.digit1Key.wasPressedThisFrame)         //Debug: Press 1 to take damage
        {
            Damage(20, 0.5f);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)         //Debug: Press 2 to go back to max health
        {
            FullHeal();
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)         //Debug: Press 3 to add 20 HP
        {
            SetMaxHealth(maxHealth + 20);
        }
*/

        //Delay slider
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
            float t = timer / delayDuration;
            t = Mathf.Sin((t * Mathf.PI) / 2);                                      //https://easings.net/#easeOutSine
            delaySlider.value = Mathf.Lerp(delaySlider.value, currentHealth, t);
        }
    }

    public void Damage(int damage, float hitstun)
    {
        if(player.curState != PlayerControls.playerState.inCutscene)
        {
            if (player.curState != PlayerControls.playerState.hitstun) currentHealth -= Mathf.Clamp(damage, 0, maxHealth);
            SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                player.curState = PlayerControls.playerState.dying;
            }
            else
            {
                player.SetDamageState(hitstun);
            }

            delayTimer = delayDuration;
        }
    }

    //Returns true if healing activates, false otherwise.
    public bool Heal(int amount)
    {
        if(currentHealth != maxHealth)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            SetHealth(currentHealth);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        HPSlider.value = maxHealth;
        delaySlider.value = maxHealth;
        healthText.text = currentHealth + "";
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
        HPSlider.value = health;
        healthText.text = currentHealth + "";

        if (health > delaySlider.value)
        {
            delaySlider.value = health;
        }
    }

    //Sets a new max health
    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        HPSlider.maxValue = health;
        healthText.text = currentHealth + "";

        delaySlider.maxValue = health;
    }


}
