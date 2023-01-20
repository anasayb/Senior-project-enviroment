using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEditor;

public class AI_TLC : MonoBehaviour
{
    

    [Header("Cameras")]
    //public GameObject Maincameras;
    //public GameObject CameraController;

    [Header("GUI")]
    //public GameObject text;


    // used by AI
    public GameObject[] trafficLights;
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

    private void FixedUpdate()
    {
        //updateCarsNumbers();

        

    }




}