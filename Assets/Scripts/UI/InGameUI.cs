using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Slider disguiseSlider;
    public Image disguiseSliderFillImage;

    [SerializeField] private Image deathScreen;

    [SerializeField] private PlayerController playerController;

    private float maxHealth;
    private float currHealth;

    private bool disguiseTextDone = false;
    private bool attackTextDone = false;

    public GameObject disguiseControlsText;
    public GameObject attackControlsText;
    public GameObject border;
    public GameObject yellowVignette;
    public GameObject purpleVignette;
    public GameObject disguiseDrainingUI;

    public Image disguiseIcon;
    public RawImage knifeIcon;
    
    private bool isDisguiseFlashing = false;
    private bool isKnifeFlashing = false;

    private bool fadingInDeathScreen = false;
    private bool fadingOutDeathScreen = false;

    private float counter = 0f;
    
    private Color deathScreenC = Color.black;
    private Color boundGreyedOutC = new Color(135, 116, 0);

    void Start()
    {
        currHealth = maxHealth;

        disguiseControlsText.SetActive(false);
        border.SetActive(true);
        
        disguiseSliderFillImage.color = new Color32(208,236,124,255);
        disguiseDrainingUI.SetActive(false);
    }

    
    void Update()
    {
        UpdateDisguiseUI();
        VignetteUI();
        DisguisSliderColourChanges();

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

             if (!isDisguiseFlashing)
            {
                disguiseIcon.color = boundGreyedOutC;
            }

            if (!isKnifeFlashing)
            {
                knifeIcon.color = boundGreyedOutC;
            }

            if (Input.GetKeyDown(KeyCode.E) && !isDisguiseFlashing)
            {
                StartCoroutine(DisguiseIconFlashRed());
            }

            if (Input.GetMouseButtonDown(0) && !isKnifeFlashing)
            {
                StartCoroutine(KnifeIconFlashRed());
            }

            //put chains on eye slider and knife UI

        }
        else
        {
            yellowVignette.SetActive(false);
            
            if (!isDisguiseFlashing)
            {
                disguiseIcon.color = Color.white;
            }
            
            knifeIcon.color = Color.white;

            
            //remove chains from eye slider and knife UI
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

    public void DisguisSliderColourChanges()
    {
        float currDisguiseHealth = playerController.GetDisguiseHealth();

        if (currDisguiseHealth == 0)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(!isDisguiseFlashing)
                {
                    StartCoroutine(DisguiseIconFlashRed());
                }
            }
        }

        bool anyAngelDraining = false;

        foreach (SmallAngel angel in SmallAngel.AllSmallAngels)
        {
            if (angel != null && angel.angelIsDrainingDisguise)
            {
                anyAngelDraining = true;
                break; 
            }
        }

        if (anyAngelDraining)
        {
            disguiseSliderFillImage.color = Color.red;
            disguiseDrainingUI.SetActive(true);
        }
        else
        {
            disguiseSliderFillImage.color = new Color32(208,236,124,255);
            disguiseDrainingUI.SetActive(false);
        }
    }


    private float flashDuration = 0.1f;

    IEnumerator DisguiseIconFlashRed ()
    {
        isDisguiseFlashing = true;

        disguiseIcon.color = new Color (1f,0f,0f);
        yield return new WaitForSeconds (flashDuration);
        disguiseIcon.color = boundGreyedOutC;
        yield return new WaitForSeconds (flashDuration);

        disguiseIcon.color = new Color (1f,0f,0f);
        yield return new WaitForSeconds (flashDuration);
        disguiseIcon.color = boundGreyedOutC;
        yield return new WaitForSeconds (flashDuration);

        disguiseIcon.color = new Color (1f,0f,0f);
        yield return new WaitForSeconds (flashDuration);
        disguiseIcon.color = boundGreyedOutC;
        yield return new WaitForSeconds (flashDuration);
        
        isDisguiseFlashing = false;

    }

    IEnumerator KnifeIconFlashRed ()
    {
        isKnifeFlashing = true;

        knifeIcon.color = new Color (1f,0f,0f);
        yield return new WaitForSeconds (flashDuration);
        knifeIcon.color = boundGreyedOutC;
        yield return new WaitForSeconds (flashDuration);
        
        knifeIcon.color = new Color (1f,0f,0f);
        yield return new WaitForSeconds (flashDuration);
        knifeIcon.color = boundGreyedOutC;
        yield return new WaitForSeconds (flashDuration);

        knifeIcon.color = new Color (1f,0f,0f);
        yield return new WaitForSeconds (flashDuration);
        knifeIcon.color = boundGreyedOutC;
        yield return new WaitForSeconds (flashDuration);

        isKnifeFlashing = false;
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
            StartCoroutine(ShowDisguiseTextRoutine());
        }
        else
        {
            Debug.Log("disguise info already shown");
        }
        
    }

    public void ShowAttackText()
    {
        if(attackTextDone == false)
        {
            attackTextDone = true;
            StartCoroutine(ShowAttackTextRoutine());
        }
        else
        {
            Debug.Log("attack info already shown");
        }
    }

    IEnumerator ShowDisguiseTextRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        disguiseControlsText.SetActive(true);
        yield return new WaitForSeconds(4f);
        disguiseControlsText.SetActive(false);
    }

    IEnumerator ShowAttackTextRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            attackControlsText.SetActive(true);
            yield return new WaitForSeconds(4f);
            attackControlsText.SetActive(false);
        }

    public void FadeDeathScreen()
    {
        fadingInDeathScreen = true;
    }

    



}
