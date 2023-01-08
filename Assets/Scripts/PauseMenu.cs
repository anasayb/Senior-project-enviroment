using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject PauseMenuUI;
    private Object SceneMange1r;
    // Start is called before the first frame update
    void Start()
    {
        PauseMenuUI.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
    }


    public void Resume()
        {
            PauseMenuUI.SetActive(false);
            Time.timeScale= 1f;
            isPaused = false;

        }


    public void Pause()
    {
            PauseMenuUI.SetActive(true);
            Time.timeScale= 0;
            isPaused = true;
    
    }

    public void Menu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }
    public void Quit()
    {
        Application.Quit();
    }
      
            
    
}
