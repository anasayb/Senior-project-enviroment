using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scence_Manger : MonoBehaviour
{
    
    // Shared variables with other scence
    [HideInInspector]
    public static float[] providedTime = { 10, 10, 10, 10 };
    public static int dir = 0;
    public static int startingNumberOfCars = 22;
    public static string algorthim;

    public GameObject direction;
    public GameObject[] timeInputs;
    public GameObject timeobject;
    public GameObject method;
    public GameObject TotalNumberOfCars;


    public void FixedUpdate()
    {

        if (SceneManager.GetActiveScene().name == "Menu")
        {


            if (method.GetComponent<TMP_Dropdown>().value == 0)
            {
                timeobject.SetActive(true);
            }
            else
            {
                timeobject.SetActive(false);
            }
        }
        

    }

    /// <summary>
    /// Method <c>startprogram</c> getting the times form the user and setting the global values.
    /// </summary>
    public void startprogram()
    {

        // if(method.)

        // directiont to start the simulation
        dir = direction.GetComponent<TMP_Dropdown>().value;

        // Selected method
        int meth = method.GetComponent<TMP_Dropdown>().value;
        if (meth == 0)
        {
            algorthim = "Tradional Traffic Light System";
        }
        else
        {
            algorthim = "Queued Traffic Light System";
        }

        // Times of the traffic lights
        providedTime = new float[timeInputs.Length];
        for (int i = 0; i < timeInputs.Length; i++)
        {
            string input = timeInputs[i].GetComponent<TMP_InputField>().text;

            // Try to parse, if not take defualt value of the variable
            if (!float.TryParse(input, out providedTime[i]))
            {
                providedTime[i] = 10f;
            }
        }

        // Number of Cars
        string input2 = TotalNumberOfCars.GetComponent<TMP_InputField>().text;
        if (!int.TryParse(input2, out startingNumberOfCars))
        {
            startingNumberOfCars = 22;
        }

        StartCoroutine(LoadYourAsyncScene("SampleScene"));
        //SceneManager.LoadScene("SampleScene");
     
    }


    public void ReturnToMenu()
    {
        //SceneManager.LoadScene("Menu");
        StartCoroutine(LoadYourAsyncScene("Menu"));
    }

    public void Rerun()
    {
        //SceneManager.LoadScene("SampleScene");
        StartCoroutine(LoadYourAsyncScene("SampleScene"));
    }


    IEnumerator LoadYourAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    /*
    public  void updateNorthToggle(bool value)
    {

        for (int i = 0; i < direction.Length; i++)
        {
            if (i == 0)
            {
                direction[i].isOn = value;
            }
            else
            {
                direction[i].isOn = false;
            }
        }
    }

    public void updateWestToggle(bool value)
    {

        for (int i = 0; i < direction.Length; i++)
        {
            if (i == 1)
            {
                direction[i].isOn = value;
            }
            else
            {
                direction[i].isOn = false;
            }
        }
    }

    public void updateSouthToggle(bool value)
    {

        for (int i = 0; i < direction.Length; i++)
        {
            if (i == 2)
            {
                direction[i].isOn = value;
            }
            else
            {
                direction[i].isOn = false;
            }
        }
    }

    public void updateEastToggle(bool value)
    {

        for (int i = 0; i < direction.Length; i++)
        {
            if (i == 3)
            {
                direction[i].isOn = value;
            }
            else
            {
                direction[i].isOn = false;
            }
        }
    }

    */

}
