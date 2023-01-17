using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEditor;

public class AI_TLC : MonoBehaviour
{
    public GameObject[] trafficLights;

    public float delay = 2f;
    public float yellowLightDuration = 3f;

    [Header("Cameras")]
    //public GameObject Maincameras;
    //public GameObject CameraController;

    [Header("GUI")]
    //public GameObject text;

    private float timeVariable = 0;


    // used by AI
    public bool finish = true;
    public int time = 0;
    public int direct = 0;
    public CarCounter[] CarCount;

    // Start is called before the first frame update
    void Start()
    {
        //updateCarsNumbers();
        /*
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].SetActive(false);
            }
        */

    }

    // Update is called once per frame

    private void Update()
    {
        //updateCarsNumbers();

        timeVariable += Time.deltaTime;
        //text.GetComponent<TMP_Text>().text = "Time:\n" + Math.Max((time - timeVariable), 0).ToString();
        if (timeVariable < time - yellowLightDuration)
        {
            //text.GetComponent<TMP_Text>().color = new Color(0.02360218f, 0.3018868f, 0.01281594f);
            ChangeLightGreen(direct);
        }
        else if (timeVariable >= time - yellowLightDuration && timeVariable < time)
        {
            ChangeLightYellow(direct);
            //text.GetComponent<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
        }
        else if (timeVariable >= time && timeVariable < time + delay)
        {

            ChangeLightRed(direct);
            //timeVariable = 0;

        }
        else
        {
            ChangeLightRed(direct);
            finish = true;
            timeVariable = 0;
        }

    }


    /// <summary>
    /// Method <c>ChangeLightRed</c> make the current traffic light red.
    /// </summary>
    /// <param name="to">the current traffic light to be red</param>
    public void ChangeLightRed(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToRed();

    }

    /// <summary>
    /// Method <c>ChangeLightYellow</c> make the current traffic light yellow.
    /// </summary>
    /// <param name="to">the current traffic light to be yellow</param>
    private void ChangeLightYellow(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToYellow();

    }

    /// <summary>
    /// Method <c>ChangeLightGreen</c> make the next traffic light green.
    /// </summary>
    /// <param name="to">the next traffic light to be green</param>
    private void ChangeLightGreen(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToGreen();

        /*for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);
        }


        CameraController.GetComponent<User_Camera_Controll>().updateCameras(to);
        cameras[to].SetActive(true);
        */

    }


}