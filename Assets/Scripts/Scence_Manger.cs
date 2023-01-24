using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    public static bool EmergencyCar;

    public GameObject direction;
    public GameObject[] timeInputs;
    public GameObject timeobject;
    public GameObject method;
    public GameObject TotalNumberOfCars;
    public GameObject carNumberError;
    public GameObject timeError;
    public GameObject startButton;
    public GameObject Loading;
    public GameObject CheckBox;

    public void FixedUpdate()
    {
    
        if (SceneManager.GetActiveScene().name == "Menu")
        {


            if (method.GetComponent<TMP_Dropdown>().value == 0)
            {
                timeobject.SetActive(true);
                direction.SetActive(true);
            }
            else
            {
                
                timeobject.SetActive(false);
                direction.SetActive(false);
            }
        }
        

    }

    /// <summary>
    /// Method <c>startprogram</c> getting the times form the user and setting the global values.
    /// </summary>
    public void startprogram()
    {

        // resset the error messages
        carNumberError.SetActive(false);
        timeError.SetActive(false);

        // directtion to start the simulation
        dir = direction.transform.GetChild(1).GetComponent<TMP_Dropdown>().value;

        // Selected method
        int meth = method.GetComponent<TMP_Dropdown>().value;
        if (meth == 0)
        {
            algorthim = "Tradional Traffic Light System";
        }
        else if(meth == 1)
        {
            algorthim = "CarLoad Based Traffic Light System";
        }
        else
        {
            algorthim = "AI Traffic Light System";
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
            if (providedTime[i] <= 3)
            {
                timeError.SetActive(true);
            }
        }

        // Number of Cars
        string input2 = TotalNumberOfCars.GetComponent<TMP_InputField>().text;
        if (!int.TryParse(input2, out startingNumberOfCars))
        {
            startingNumberOfCars = 22;
        }

        if (startingNumberOfCars <= 0 || startingNumberOfCars > 180)
        {
            carNumberError.SetActive(true);
        }

        if (carNumberError.activeSelf == true || timeError.activeSelf == true)
        {
            return;
        }

        startButton.SetActive(false);
        Loading.SetActive(true);

        if (CheckBox.GetComponent<Toggle>().isOn)
        {
            EmergencyCar = true;
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

}
