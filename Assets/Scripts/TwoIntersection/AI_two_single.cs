using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System;
using static UnityEditor.PlayerSettings;
using UnityEditor;
using TMPro;

public class AI_two_single : Agent
{

    public static bool startCouting = false;

    // Enviroment Varaibles
    //public GameObject prefab;
    public GameObject cars;
    //public GameObject newEnv;
    //public int episodeNumber = 0;

    [Header("Cameras")]
    public GameObject cameraController;

    private int Intersection;

    [Header("GUI")]
    public GameObject timer;

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


    // Emregency car
    int currentEmergencyDirection = -1;
    float EmegencyTimeVariable = 0;

    public void Start()
    {
        AI_two_single.startCouting = false;
        cars = GameObject.Find("Cars");
        Intersection = (transform.parent.name[transform.name.Length - 1] - '0') - 1;
        if (Scence_Manger.algorthim != "AI based Traffic Light System")
        {
            GetComponent<AI_two_single>().enabled = false;
            return;
        }

        // Check for Emergency car
        for (int i = 0; i < 4; i++)
        {
            if (CarCount[i].emergencyExist)
            {
                currentEmergencyDirection = i;
            }
        }

    }

    public void FixedUpdate()
    {
        if (CarCount[0].carsCounter + CarCount[1].carsCounter + CarCount[2].carsCounter + CarCount[3].carsCounter != 0)
        {
            // This piece of code job is to check if there is emergency car if true it wil give it priority without specific time 
            if (currentEmergencyDirection != -1)
            {
                if (AI_two_single.startCouting == false)
                {
                    AI_two_single.startCouting = true;
                }
                EmegencyTimeVariable += Time.deltaTime;
                if (User_Controll.Intersection == Intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().text = "EM";
                }
                if (EmegencyTimeVariable <= yellowLightDuration)
                {
                    ChangeLightYellow(direct);
                    if (User_Controll.Intersection == Intersection)
                    {
                        timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
                    }
                }

                if (EmegencyTimeVariable >= yellowLightDuration && EmegencyTimeVariable < yellowLightDuration + delay)
                {
                    ChangeLightRed(direct);
                }

                if (EmegencyTimeVariable >= yellowLightDuration + delay)
                {
                    if (CarCount[currentEmergencyDirection].emergencyExist)
                    {
                        ChangeLightGreen(currentEmergencyDirection);
                        if (User_Controll.Intersection == Intersection)
                        {
                            timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                        }
                    }
                    else
                    {
                        timeVariable = 0f;
                        EmegencyTimeVariable = 0;
                        time = 0;
                        direct = currentEmergencyDirection;
                        time = (int)yellowLightDuration + 1;
                        currentEmergencyDirection = -1;

                    }
                }
                return;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (CarCount[i].emergencyExist)
                    {
                        currentEmergencyDirection = i;
                        if (currentEmergencyDirection == direct)
                        {
                            EmegencyTimeVariable = yellowLightDuration + delay;
                        }
                        return;
                    }
                }
            }

            if (Avg_wating_time_two.numberOfCars == 0 || Avg_wating_time_two.Avg_wating >= 160)
            {

                SetReward(1 - (Avg_wating_time_two.Avg_wating / 160));

                //EndEpisode();
                //Destroy(temp);
                //cars.GetComponent<Avg_wating_time_two>().Avg_wating = 0;

            }
            else
            {
                if (User_Controll.Intersection == Intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().text = "AI";
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                }
                TrafficLightControlling();

            }
        }
    }

    public override void OnEpisodeBegin()
    {

        // Resest variabels
        //time = 0;
        //direct = 0;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GetComponent<AI_TLC_two_single>().CarCount[0].carsCounter);
        sensor.AddObservation(GetComponent<AI_TLC_two_single>().CarCount[1].carsCounter);
        sensor.AddObservation(GetComponent<AI_TLC_two_single>().CarCount[2].carsCounter);
        sensor.AddObservation(GetComponent<AI_TLC_two_single>().CarCount[3].carsCounter);
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

        if (AI_two_single.startCouting == false)
        {
            AI_two_single.startCouting = true;
        }
        timeVariable += Time.deltaTime;
        if (timeVariable < time - yellowLightDuration - 1)
        {

            ChangeLightGreen(direct);
            if (User_Controll.Intersection == Intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                timer.GetComponentInChildren<TMP_Text>().text = "AI";
            }

        }
        else if (timeVariable >= time - yellowLightDuration - 1 && timeVariable < time - yellowLightDuration)
        {
            if (once)
            {
                once = false;
                RequestDecision();
            }

        }
        else if (timeVariable >= time - yellowLightDuration && timeVariable < time)
        {
            if (User_Controll.Intersection == Intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
            }
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
        cameraController.GetComponent<User_Controll>().updateCameras(to, Intersection);

    }



}
