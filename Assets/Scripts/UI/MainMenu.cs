using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject controlsPanel;
    public GameObject mainMenuPanel;

    public Slider exposureSlider;
    public Slider sensitivitySlider;
    
    void Start()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void Awake()
    {
        // Set up listeners to instantly save the data when a player drags the slider
        if (exposureSlider != null)
        {
            exposureSlider.onValueChanged.AddListener(SaveExposureValue);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(SaveSensitivityValue);
        }
    }

    void SaveExposureValue(float value)
    {
        PlayerPrefs.SetFloat("SavedExposureSliderValue", value);
        PlayerPrefs.Save();
    }

    void SaveSensitivityValue(float value)
    {
        PlayerPrefs.SetFloat("SavedSensitivitySliderValue", value);
        PlayerPrefs.Save();
    }

    
    void Update()
    {
        
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Cave");
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
        #if UNITY_STANDALONE
        Application.Quit();
        #endif
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }




}
