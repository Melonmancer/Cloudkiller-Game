using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public GameObject winPanel;
    public bool win = false;
    

    void Start()
    {
        winPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit the portal!");
            //winPanel.SetActive(true);

            //Time.timeScale = 0f;

            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            win = true;
            SceneManager.LoadScene("Borderlands");
        }
    }
}
