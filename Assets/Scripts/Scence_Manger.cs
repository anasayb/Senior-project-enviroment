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
    public static int providedTime = 10;

    public GameObject timeInput;

    public void startprogram()
    {
       string input = timeInput.GetComponent<TMP_InputField>().text;

        // Try to parse, if not take defualt value of the variable
        if (!int.TryParse(input, out providedTime))
        {
            providedTime = 10;
        }
       
        SceneManager.LoadScene("SampleScene");
     
    }

}
