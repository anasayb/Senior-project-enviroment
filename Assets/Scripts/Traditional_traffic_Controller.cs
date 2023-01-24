using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Traditional_traffic_Controller : MonoBehaviour
{
    public Component[] trafficLights;
    
    public float[] time;
    public float delay = 2;
    public float yellowLightDuration = 3f;
    

    [Header("GUI")]
    public GameObject timer;

    [Header("Cameras")]
    public GameObject Maincamera;
    //public GameObject CameraController;

    private float timeVariable = 0;
    private int direction = 0;

    // Start is called before the first frame update
    void Start()
    {
        

        time = Scence_Manger.providedTime;
        direction = Scence_Manger.dir;
        if (direction == 0)
        {
            Maincamera.GetComponent<User_Controll>().currentPostionOfCamera = "North";

        }else if (direction == 1)
        {
            Maincamera.GetComponent<User_Controll>().currentPostionOfCamera = "West";

        }
        else if (direction == 2)
        {
            Maincamera.GetComponent<User_Controll>().currentPostionOfCamera = "South";

        }
        else if (direction == 3)
        {
            Maincamera.GetComponent<User_Controll>().currentPostionOfCamera = "East";

        }

        if (Scence_Manger.algorthim == "CarLoad Based Traffic Light System")
        {

            GetComponent<Basic_algo>().enabled = true;
            GetComponent<Traditional_traffic_Controller>().enabled = false;
            return;

        }
        else if (Scence_Manger.algorthim == "AI Traffic Light System")
        {

            GetComponent<AI>().enabled = true;
            //GetComponent<AI_TLC>().enabled = true;
            GetComponent<Traditional_traffic_Controller>().enabled = false;
            return;
        }

        ChangeLightGreen(direction);
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
 
        timeVariable += Time.deltaTime;
        timer.GetComponentInChildren<TMP_Text>().text = Math.Max((Math.Ceiling(time[direction]-timeVariable)), 0).ToString();
        timer.transform.GetChild(1).GetComponent<Image>().fillAmount = (1-(timeVariable/time[direction]));
        if (timeVariable >= time[direction] - yellowLightDuration && timeVariable < time[direction])
        {
            ChangeLightYellow(direction);
            timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
        }
        if (timeVariable >= time[direction])
        {
            ChangeLightRed(direction);
            
        }
        if (timeVariable >= time[direction] +delay)
        {
            direction++;
            direction %= 4;
            ChangeLightGreen(direction);    
            timeVariable = 0;
            timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
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
        Maincamera.GetComponent<User_Controll>().updateCameras(to);
        
    }
}
