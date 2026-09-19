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

        if (volume.profile.TryGet(out colorAdjustments))
        {
            float defaultExposure = colorAdjustments.postExposure.value;
            float savedExposure = PlayerPrefs.GetFloat("SavedExposureSliderValue", defaultExposure);

            colorAdjustments.postExposure.value = savedExposure;
            exposureSlider.value = savedExposure;

            exposureSlider.onValueChanged.AddListener(SetExposure);
        }

        if (programmedCamera != null)
        {
            float defaultSens = programmedCamera.sensitivity;
            float savedSens = PlayerPrefs.GetFloat("SavedSensitivitySliderValue", defaultSens);

            programmedCamera.sensitivity = savedSens;
            sensitivitySlider.value = savedSens;

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
            
            PlayerPrefs.SetFloat("SavedExposureSliderValue", value);
            PlayerPrefs.Save();
        }
    }

    private void SetSensitivity(float value)
    {
        if (programmedCamera != null)
        {
            programmedCamera.sensitivity = value;

            PlayerPrefs.SetFloat("SavedSensitivitySliderValue", value);
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
        if (exposureSlider != null) exposureSlider.onValueChanged.RemoveListener(SetExposure);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.RemoveListener(SetSensitivity);
    }


    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    public void LowQualityGraphics()
    {
        //disable bloom
    }

    public void HighQualityGraphics()
    {
        //enable bloom
    }

}
