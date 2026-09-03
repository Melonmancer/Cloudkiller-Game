using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    //public Slider healthSlider;
    public Slider disguiseSlider;
    [SerializeField] private Image deathScreen;

    [SerializeField] private PlayerController playerController;

    private float maxHealth;
    private float currHealth;

    private bool disguiseTextDone = false;

    public GameObject disguiseControlsText;
    public GameObject border;
    public GameObject yellowVignette;
    public GameObject purpleVignette;
    
    

    private bool fadingInDeathScreen = false;
    private bool fadingOutDeathScreen = false;

    private float counter = 0f;
    
    private Color deathScreenC = Color.black;

    void Start()
    {

        //float[] healthValues = playerController.GetHealthValues();
        //healthSlider.maxValue = healthValues[1];

        currHealth = maxHealth;

        disguiseControlsText.SetActive(false);
        border.SetActive(true);
        

    }

    
    void Update()
    {
        UpdateDisguiseUI();
        VignetteUI();

        if(fadingOutDeathScreen)
        {
            if(fadingInDeathScreen)
            {
                fadingOutDeathScreen = false;
            }

            //Debug.Log(counter);
            counter = Mathf.Lerp(counter, 0f, Time.deltaTime * 0.5f);
            deathScreenC.a = counter;
            deathScreen.color = deathScreenC;

            if(counter <= 0.0025)
            {
                deathScreenC.a = 0f;
                deathScreen.color = deathScreenC;
                fadingOutDeathScreen = false;
            }            
        }

        if(fadingInDeathScreen)
        {
            counter = Mathf.Lerp(counter, 1f, Time.deltaTime * 10);
            deathScreenC.a = counter;
            deathScreen.color = deathScreenC;

            if(counter >= 0.95)
            {
                fadingInDeathScreen = false;
                fadingOutDeathScreen = true;
                counter = 1f;
            }
        }
    }

    public void VignetteUI () 
    {
        if (playerController.bound == true)
        {
            yellowVignette.SetActive(true);

        }
        else
        {
            yellowVignette.SetActive(false);
        }

        if (playerController.isDisguised == true)
        {
            purpleVignette.SetActive(true);
        }
        else
        {
            purpleVignette.SetActive(false);
        }

        
    }




    /*
    void UpdateHealthUI()
    {
        float[] healthValues = playerController.GetHealthValues();

        float currHealth = healthValues[0];
        float maxHealth = healthValues[1];

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currHealth;

    }

    */

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

    public void FadeDeathScreen()
    {
        fadingInDeathScreen = true;
    }

}
