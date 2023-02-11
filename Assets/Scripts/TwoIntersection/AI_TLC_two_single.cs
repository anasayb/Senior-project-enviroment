using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.UI;

public class AI_TLC_two_single : MonoBehaviour
{
    public GameObject[] trafficLights;

    public float delay = 2f;
    public float yellowLightDuration = 3f;

    [Header("Cameras")]
    public GameObject camerContoller;

    private int Intersection;
    //public GameObject CameraController;

    [Header("GUI")]
    public GameObject timer;

    private float timeVariable = 0;


    // used by AI
    public bool finish = true;
    public int time = 0;
    public int direct = 0;
    public CarCounter[] CarCount;

    // Start is called before the first frame update
    void Start()
    {

        Intersection = (transform.parent.name[transform.name.Length - 1] - '0') - 1;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        timeVariable += Time.deltaTime;
        if (User_Controll.Intersection == Intersection) {
            timer.GetComponentInChildren<TMP_Text>().text = Math.Max((Math.Ceiling(time - timeVariable)), 0).ToString();
            timer.transform.GetChild(1).GetComponent<Image>().fillAmount = (1 - (timeVariable / time));
        }
        if (timeVariable < time - yellowLightDuration)
        {
            if (User_Controll.Intersection == Intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            }
            ChangeLightGreen(direct);
        }
        else if (timeVariable >= time - yellowLightDuration && timeVariable < time)
        {
            ChangeLightYellow(direct);
            if (User_Controll.Intersection == Intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
            }
        }
        else if (timeVariable >= time && timeVariable < time + delay)
        {

            ChangeLightRed(direct);

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
        camerContoller.GetComponent<User_Controll>().updateCameras(to, Intersection);

    }


}