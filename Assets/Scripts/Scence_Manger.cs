using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scence_Manger : MonoBehaviour
{
    // Shared variables with other scence
    [HideInInspector]
    public static float[] providedTime;

    public GameObject[] timeInputs;


    /// <summary>
    /// Method <c>startprogram</c> getting the times form the user and setting the global values.
    /// </summary>
    public void startprogram()
    {
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

}
