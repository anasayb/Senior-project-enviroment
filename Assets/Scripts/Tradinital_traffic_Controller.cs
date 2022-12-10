using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class Tradinital_traffic_Controller : MonoBehaviour
{
    public Component[] trafficLights;
    public float time;
    public float delay = 2;

    [Header("Cameras")]
    public GameObject[] cameras;

    private float timeVariable = 0;
    private int direction = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false); 
        }
        ChangeLightGreen(direction);
        direction++;
    }

    // Update is called once per frame
    void Update()
    {
 
        timeVariable += Time.deltaTime;
        if (timeVariable >= time)
        {
            ChangeLightRed(direction);
        }
        if (timeVariable >= time+delay)
        {
            ChangeLightGreen(direction);
            direction++;
            direction %= 4;
            timeVariable= 0;
        }

    }

    private void FixedUpdate()
    {
       


    }

    /// <summary>
    /// Method <c>ChangeLightRed</c> make the next traffic light red.
    /// </summary>
    /// <param name="to">the next traffic light to be red</param>
    private void ChangeLightRed(int to)
    {

        if (to == 0)
        {
            trafficLights[3].GetComponent<Light_Conteroler>().chagneToRed();
        }
        else
        {
            trafficLights[to - 1].GetComponent<Light_Conteroler>().chagneToRed();
        }

    }

    /// <summary>
    /// Method <c>ChangeLightGreen</c> make the next traffic light green.
    /// </summary>
    /// <param name="to">the next traffic light to be green</param>
    private void ChangeLightGreen(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToGreen();

        if (to == 0)
        {
            cameras[3].SetActive(false); ;
        }
        else
        {
            cameras[to - 1].SetActive(false); ;
        }

        cameras[to].SetActive(true);
        
    }
}
