using Google.Protobuf.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject PauseMenuUI;
    private float originalTimeScale;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) // if the menu is paused enter
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
    }

    //Resumes the menu and continue where stopped from 
    public void Resume()
        {
            PauseMenuUI.SetActive(false);
            Time.timeScale= originalTimeScale;
            isPaused = false;

        }

    //Pause the menu and stop the simulation temporarily  
    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        originalTimeScale = Time.timeScale;
        Time.timeScale= 0;  
        isPaused = true;
    
    }

    //This function sends u to the menu
    public void Menu()
    {
        isPaused = false;
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }
    //Quits the queue 
    public void Quit()
    {
        Application.Quit();
    }
}
