using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider disguiseSlider;

    public PlayerController playerController;

    private float maxHealth;
    private float currHealth;
    
    void Start()
    {
        //PlayerController playerController = new PlayerController();
        //float value = playerController.health;
        //float value = playerController.maxHealth;

        //playerController.GetDisguiseHealth();
        //playerController.GetHealthValues();

        float[] healthValues = playerController.GetHealthValues();
        healthSlider.maxValue = healthValues[1];

        currHealth = maxHealth;
    }

    
    void Update()
    {
        UpdateHealthUI();
        UpdateDisguiseUI();

    }

    void UpdateHealthUI()
    {
        float[] healthValues = playerController.GetHealthValues();

        float currHealth = healthValues[0];
        float maxHealth = healthValues[1];

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currHealth;

    }

    void UpdateDisguiseUI()
    {
         float disguise = playerController.GetDisguiseHealth();

        disguiseSlider.value = disguise;
    }

    
}
