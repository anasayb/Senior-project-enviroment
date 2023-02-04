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
    public static bool startCouting = false;

    public Component[] trafficLights;
    
    public float[] time;
    public float delay = 2;
    public float yellowLightDuration = 3f;
    

    [Header("GUI")]
    public GameObject timer;

    [Header("Cameras")]
    public GameObject cameraContoller;

    private int Intersection = 0;
    //public GameObject CameraController;

    private float timeVariable = 0;
    private int direction = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        Traditional_traffic_Controller.startCouting = false;
        time = Scence_Manger.providedTime;
        direction = Scence_Manger.dir;
        Intersection = transform.parent.name[transform.parent.name.Length - 1] - '0' - 1;
        if (direction == 0)
        {
            cameraContoller.GetComponent<User_Controll>().currentPostionOfCamera[Intersection] = "North";

        }else if (direction == 1)
        {
            cameraContoller.GetComponent<User_Controll>().currentPostionOfCamera[Intersection] = "West";

        }
        else if (direction == 2)
        {
            cameraContoller.GetComponent<User_Controll>().currentPostionOfCamera[Intersection] = "South";

        }
        else if (direction == 3)
        {
            cameraContoller.GetComponent<User_Controll>().currentPostionOfCamera[Intersection] = "East";

        }

        if (Scence_Manger.algorthim != "Traditional Traffic Light System")
        {
            GetComponent<Traditional_traffic_Controller>().enabled = false;
            return;
        }


        ChangeLightGreen(direction);
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Traditional_traffic_Controller.startCouting = true;
        timeVariable += Time.deltaTime;
        if (User_Controll.Intersection == transform.parent.name[transform.parent.name.Length-1]-'0'-1) {
            timer.GetComponentInChildren<TMP_Text>().text = Math.Max((Math.Ceiling(time[direction] - timeVariable)), 0).ToString();
            timer.transform.GetChild(1).GetComponent<Image>().fillAmount = (1 - (timeVariable / time[direction]));
        }
        if (timeVariable >= time[direction] - yellowLightDuration && timeVariable < time[direction])
        {
            ChangeLightYellow(direction);
            if (User_Controll.Intersection == Intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
            }
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
            if (User_Controll.Intersection == Intersection)
            {
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
        cameraContoller.GetComponent<User_Controll>().updateCameras(to, Intersection);
        
    }
}
