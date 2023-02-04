using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEditor;

public class AI_TLC_two_multi : MonoBehaviour
{
    

    [Header("Cameras")]
    //public GameObject Maincameras;
    //public GameObject CameraController;

    [Header("GUI")]
    //public GameObject text;


    // used by AI
    public GameObject[] trafficLightsIntersection0;
    public GameObject[] trafficLightsIntersection1;
    public CarCounter[] CarCountIntersetion0;
    public CarCounter[] CarCountIntersetion1;

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

    private void FixedUpdate()
    {
        //updateCarsNumbers();

        

    }




}