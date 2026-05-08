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

    private bool disguiseTextDone = false;

    public GameObject disguiseControlsText;

    
    void Start()
    {

        float[] healthValues = playerController.GetHealthValues();
        healthSlider.maxValue = healthValues[1];

        currHealth = maxHealth;

        disguiseControlsText.SetActive(false);
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

    public void ShowDisguiseText()
    {
        if(disguiseTextDone == false)
        {
            disguiseTextDone = true;
            StartCoroutine(ShowTextRoutine());
        }
        else
        {
            Debug.Log("disguise info already shown");
        }
        
    }

    IEnumerator ShowTextRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        disguiseControlsText.SetActive(true);
        yield return new WaitForSeconds(4f);
        disguiseControlsText.SetActive(false);
    }


}
