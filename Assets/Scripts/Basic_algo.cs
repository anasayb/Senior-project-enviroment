using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_algo : MonoBehaviour
{
    public Component[] trafficLights;

    public float[] time;
    public float delay = 2;
    public float yellowLightDuration = 3f;
    public static int carNumberNorth;

    [Header("Green Time Calculation")]
    public int[] carNumber;
    public CarCounter[] CarCount;

    [Header("Cameras")]
    public GameObject[] cameras;
    public GameObject CameraController;

    private float timeVariable;
    private int direction = 0;


    // Start is called before the first frame update
    void Start()
    {
       
        // carCount = GetComponent<CarCounter>();
        // time = 0; Just leaving it 0 for now 
        // timeVariable = GreenTimeCalc(carCount.counterTemp);
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
        // Turns();
        if (timeVariable >= time[direction] - yellowLightDuration && timeVariable < time[direction])
        {
            ChangeLightYellow(direction);
        }
        if (timeVariable >= time[direction])
        {
            ChangeLightRed(direction);

        }
        if (timeVariable >= time[direction] + delay)
        {
            direction++;
            direction %= 4;
            timeVariable = CarCount[direction].counterTemp; 
            timeVariable = 5; //GreenTimeCalc(carCount.counterTemp[direction]);
            ChangeLightGreen(direction);
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

    private int GreenTimeCalc(int carNo)
    {
        // Just temp simple formula 
        int greenTime=(((carNo * 3)*2)/2); // * 2 and / 2 just incase car number is zero , i know i couldve made if statment but idk lol
        if (greenTime >= 30)
        {
            return 30;
        }
        else
        {
            return greenTime;
        }

    }

   /* private void Turns()
    {
        if(carCount.counterTemp[direction+/]> carCount.counterTemp[direction + 2])
        {

        }else
        {
            
        }
    }
   */
}