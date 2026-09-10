using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour


{

    public GameObject controlsPanel;
    public GameObject pauseMenu;
    
    public static bool isPaused;

    public LevelEnd levelEnd;

    [SerializeField] private Slider exposureSlider;
    [SerializeField] private Volume volume;

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private ProgrammedCamera programmedCamera;

    private ColorAdjustments colorAdjustments;
    
    void Start()
    {
        controlsPanel.SetActive(false);
        pauseMenu.SetActive(false);

        {
        if (volume.profile.TryGet(out colorAdjustments))
        {
            exposureSlider.onValueChanged.AddListener(SetExposure);

            exposureSlider.value = colorAdjustments.postExposure.value;
        }

        sensitivitySlider.value = programmedCamera.sensitivity;

        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                CloseControls();
                Resume();
            }
            else
            {
                Pause();
            }

        }

    }

    private void SetExposure(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = value;
        }
    }

    private void OnDestroy()
    {
        exposureSlider.onValueChanged.RemoveListener(SetExposure);
        sensitivitySlider.onValueChanged.RemoveListener(SetSensitivity);
    }

    private void SetSensitivity(float value)
    {
        programmedCamera.sensitivity = value;
    }


    public void Pause()
    {
        //if (levelEnd.win == false)
        //{
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        //}
        

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

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void CloseControls()
    {
            controlsPanel.SetActive(false);
            pauseMenu.SetActive(true);

    }

    public void GoToMainMenu()
    {
        Debug.Log("MainMenu clicked");

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        #if UNITY_STANDALONE
        Application.Quit();
        #endif
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
