using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject controlsPanel;
    public GameObject mainMenuPanel;
    
    void Start()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    
    void Update()
    {
        
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1LayoutV2");
    }

    public void OpenControls()
    {
        Time.timeScale = 1f;
        controlsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        
    }

    public void ExitControls()
    {
        Time.timeScale = 1f;
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
    }

    public void ExitGame()
    {
        //ApplicationException.Quit();
    }




}
