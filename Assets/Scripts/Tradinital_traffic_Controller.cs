using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class Tradinital_traffic_Controller : MonoBehaviour
{
    public Component[] trafficLights;
    
    public float[] time;
    public float delay = 2;
    public float yellowLightDuration = 3f;

    [Header("Cameras")]
    public GameObject[] cameras;
    public GameObject CameraController;

    private float timeVariable = 0;
    private int direction = 0;


    // Start is called before the first frame update
    void Start()
    {
        time = Scence_Manger.providedTime;
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false); 
        }
        ChangeLightGreen(direction);
       
    }

    // Update is called once per frame
    void Update()
    {
 
        timeVariable += Time.deltaTime;
        if (timeVariable >= time[direction] - yellowLightDuration && timeVariable < time[direction])
        {
            ChangeLightYellow(direction);
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

        }

    }

    private void FixedUpdate()
    {
       


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

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);
        }


        CameraController.GetComponent<User_Camera_Controll>().updateCameras(to);
        cameras[to].SetActive(true);
        
    }
}
