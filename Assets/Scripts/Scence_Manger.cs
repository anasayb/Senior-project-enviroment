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
    public Toggle[] direction;

    // Shared variables with other scence
    [HideInInspector]
    public static float[] providedTime = { 10, 10, 10, 10 };
    public static int dir = 0;
    public static int startingNumberOfCars = 22;

    public GameObject[] timeInputs;
    public GameObject method; 

    /// <summary>
    /// Method <c>startprogram</c> getting the times form the user and setting the global values.
    /// </summary>
    public void startprogram()
    {

        // if(method.)

        // directiont to start the simulation
        for (int i = 0; i < direction.Length; i++)
        {
            if (direction[i].isOn)
            {
                dir = i;
            }
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


        SceneManager.LoadScene("SampleScene");
     
    }



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


}
