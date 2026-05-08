using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour


{

    public GameObject pauseMenu;
    
    public static bool isPaused;

    public LevelEnd levelEnd;
    
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }

    }


    public void Pause()
    {
        if (levelEnd.win == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        

    }

    public void Resume()
    {
        Debug.Log("Resume clicked");

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void GoToMainMenu()
    {
        Debug.Log("MainMenu clicked");

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
