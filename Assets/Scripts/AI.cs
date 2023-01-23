using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System;
using static UnityEditor.PlayerSettings;
using UnityEditor;

public class AI : Agent
{

    // Enviroment Varaibles
    //public GameObject prefab;
    public GameObject cars;
    //public GameObject newEnv;
    //public int episodeNumber = 0;


    // Traffic light controlling variable
    public GameObject[] trafficLights;
    public CarCounter[] CarCount;

    private float delay = 2f;
    private float yellowLightDuration = 3f;
    private float timeVariable = 0;


    //private bool finish = true;
    private int direct = 0;
    private int nextDirect = 0;
    private int time = 0;
    private int nextTime = 0;
    private bool once = true;



    public void Start()
    {
        cars = GameObject.Find("Cars");
        for (int i = 0; i < 4; i++)
        {
            GetComponent<AI_TLC>().ChangeLightRed(i);
        }

    }

    public void FixedUpdate()
    {   
        if (Avg_wating_time.numberOfCars == 0 || Avg_wating_time.Avg_wating >= 100)
        {
            if (Avg_wating_time.numberOfCars == 0)
            {
                SetReward(1f);
            }
            else
            {
                SetReward(-1f);
            }

            //EndEpisode();
            //Destroy(temp);
            //cars.GetComponent<Avg_wating_time>().Avg_wating = 0;
 
        }
        else
        {
            TrafficLightControlling();

        }
    }

    public override void OnEpisodeBegin()
    {
        /*

        if (episodeNumber == 0)
        {
            episodeNumber++;
            return;
        }
        episodeNumber++;
        Debug.Log("ep: " + episodeNumber);
        //GameObject temp = cars;
        
        Destroy(cars);
        cars = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero));
        cars.transform.name = "Cars";
        cars.transform.SetParent(transform.parent.parent.parent);
        cars.transform.localPosition = new Vector3(0,0,0);
        //avg = newCar.GetComponent<Avg_wating_time>();    
        foreach (Transform dir in cars.transform)
        {
            Transform left = transform.parent.parent.GetChild(0).Find("Turnining Left Path " + dir.transform.name).transform;
            Transform right = transform.parent.parent.GetChild(1).Find("Turnining Right Path " + dir.transform.name).transform;
            foreach (Transform car in dir)
            {
                car.GetComponent<CarController>().pathGourpLeft = left;
                car.GetComponent<CarController>().pathGourpRight = right;
            }

        }
        //newCar.GetComponent<Avg_wating_time>().Text = GameObject.Find("Avg_waiting_time_text");
        //cars.GetComponent<Avg_wating_time>().Avg_wating = 0;

        */

        // Resest variabels
        time = 0;
        direct = 0;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GetComponent<AI_TLC>().CarCount[0].carsCounter);
        sensor.AddObservation(GetComponent<AI_TLC>().CarCount[1].carsCounter);
        sensor.AddObservation(GetComponent<AI_TLC>().CarCount[2].carsCounter);
        sensor.AddObservation(GetComponent<AI_TLC>().CarCount[3].carsCounter);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {

        nextDirect = actions.DiscreteActions[0];

        int temp = actions.DiscreteActions[1];
        if (temp < 4)
        {
            nextTime = 0;
        }
        else
        {
            nextTime = temp;
        }


    }

    private void TrafficLightControlling()
    {
        if (time == 0 || time == -1)
        {
            if (nextTime != 0 && nextTime != -1)
            {
                time = nextTime;
                direct = nextDirect;
                nextTime = -1;
                nextDirect = -1;
            }
            else
            {
                RequestDecision();
            }
            return;

        }

        timeVariable += Time.deltaTime;
        if (timeVariable < time - yellowLightDuration - 1)
        {

            ChangeLightGreen(direct);

        }
        else if (timeVariable >= time - yellowLightDuration - 1 && timeVariable < time - yellowLightDuration)
        {
            if (once)
            {
                once = false;
                SetReward(-(Avg_wating_time.Avg_wating / 100));
                RequestDecision();
            }

        }
        else if (timeVariable >= time - yellowLightDuration && timeVariable < time)
        {

            if (direct == nextDirect)
            {
                time = nextTime;
                direct = nextDirect;
                nextTime = -1;
                nextDirect = -1;
                timeVariable = 0;
                once = true;
            }
            else
            {
                ChangeLightYellow(direct);
            }

        }
        else if (timeVariable >= time && timeVariable < time + delay)
        {

            ChangeLightRed(direct);

        }
        else
        {
            //ChangeLightRed(direct);
            time = nextTime;
            direct = nextDirect;
            nextTime = -1;
            nextDirect = -1;
            timeVariable = 0;
            once = true;
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
        for (int i = 0; i < 4; i++)
        {
            ChangeLightRed(i);
        }
        trafficLights[to].GetComponent<Light_Conteroler>().chagneToGreen();

    }



}
