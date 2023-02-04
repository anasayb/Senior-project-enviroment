using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.UI;
using JetBrains.Annotations;

public class AI_TLC : MonoBehaviour
{
    public GameObject[] trafficLights;

    public float delay = 2f;
    public float yellowLightDuration = 3f;

    [Header("Cameras")]
    public GameObject cameraController;

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
        //direct = Scence_Manger.dir;
        Intersection = (transform.parent.name[transform.name.Length - 1] - '0') - 1;
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        //updateCarsNumbers();

       

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
        cameraController.GetComponent<User_Controll>().updateCameras(to, Intersection);

    }


}