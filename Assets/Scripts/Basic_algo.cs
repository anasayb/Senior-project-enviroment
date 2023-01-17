using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Basic_algo : MonoBehaviour
{
    public Component[] trafficLights;

    private float[] time = { 0, 0, 0, 0 }; // i assumed we will show him without the menu because its not fixed as u know 
    public float delay = 1; // im not sure if we need this 
    public float yellowLightDuration = 2f;
    public static int carNumberNorth;
    private float timeVariable = 0f;
    private int direction = 0;
    private bool oneTimeRun = true;

    [Header("GUI")]
    public GameObject timer;

    [Header("Green Time Calculation")]
    // public int[] carNumber;
    public CarCounter[] CarCount;


    [Header("Cameras")]
    public GameObject Maincamera;
    public GameObject CameraController;




    // Start is called before the first frame update
    void Start()
    {
        direction = Scence_Manger.dir;
        if (direction == 0)
        {
            Maincamera.GetComponent<User_Camera_Controll>().currentPostionOfCamera = "North";

        }
        else if (direction == 1)
        {
            Maincamera.GetComponent<User_Camera_Controll>().currentPostionOfCamera = "West";

        }
        else if (direction == 2)
        {
            Maincamera.GetComponent<User_Camera_Controll>().currentPostionOfCamera = "South";

        }
        else if (direction == 3)
        {
            Maincamera.GetComponent<User_Camera_Controll>().currentPostionOfCamera = "East";

        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (oneTimeRun) // instead of writing it on the start method ive done it here . 
        {
            // The first direction manually setted to few sec just to show the skipping feature
            time[direction] = 8f;// GreenTimeCalc(CarCount[direction].carsCounter);
            ChangeLightGreen(direction);
            oneTimeRun = false;
        }

        timeVariable += Time.deltaTime;
        Debug.Log(" Direction : " + direction +
                  " Time assigned : " + time[direction] +
                  " Car count : " + CarCount[direction].carsCounter);
        timer.GetComponentInChildren<TMP_Text>().text = Math.Max((Math.Ceiling(time[direction] - timeVariable)), 0).ToString();
        timer.transform.GetChild(1).GetComponent<Image>().fillAmount = (1 - (timeVariable / time[direction]));
        if (timeVariable >= time[direction] - yellowLightDuration && timeVariable <= time[direction])
        {
            ChangeLightYellow(direction);
            timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
        }
        if (timeVariable >= time[direction])
        {
            ChangeLightRed(direction);
        }
        if (timeVariable >= time[direction] + delay)
        {
            direction++;
            direction %= 4;
            time[direction] = GreenTimeCalc(CarCount[direction].carsCounter);
            // this loop does the job of skipping the direction i think it needs to be optimized but for now it does the job 
            for (int i = 0; i < 4; i++) 
                 
            {
                if (time[direction] == 0)
                {
                    direction++;
                    direction %= 4;
                    time[direction] = GreenTimeCalc(CarCount[direction].carsCounter);
                    continue;

                }
                else
                {
                    break;
                }

            }
            timeVariable = 0f;

            if (time[direction] != 0)
            {
                ChangeLightGreen(direction);
                timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            }
            
        }


    }


    /// <summary>
    /// Method <c>ChangeLightRed</c> make the current traffic light red.
    /// </summary>
    /// <param name="to">the current traffic light to be red</param>
    private void ChangeLightRed(int to)
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
        Maincamera.GetComponent<User_Camera_Controll>().updateCameras(to);

    }

    private int GreenTimeCalc(int carNo)
    {
        // Just temp simple formula 
        int greenTime = (carNo * 2) + (int)delay; // here is our simple formula so far i need more time to dig and get the proper and suitable one , i substracted the yellow time so it does not count up there
        if (greenTime >= 30)
        {
            return 30+1;
        }
        else if (carNo == 0) // if there is no cars 
        {
            return 0;
        }
        else
        {
            return greenTime+1;
        }

    }

    /* private void Turns() will use it on choosing lanes .
     {
         if(carCount.counterTemp[direction+/]> carCount.counterTemp[direction + 2])
         {

         }else
         {

         }
     }
    */
}