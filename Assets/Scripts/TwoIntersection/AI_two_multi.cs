using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AI_two_multi : Agent
{
    public static bool startCouting = false;

    [Header("Cameras")]
    public GameObject cameraController;

    [Header("GUI")]
    public GameObject timer;


    // Enviroment Varaibles
    public int episodeNumber = 0;


    // Traffic light controlling variable
    private GameObject[,] trafficLights;
    private CarCounter[,] CarCount;

    private float delay = 2f;
    private float yellowLightDuration = 3f;
    private float[] timeVariable = { 0, 0 };


    //private bool finish = true;
    private int[] direct = { 0, 0 };
    private int[] nextDirect = { 0, 0 };
    private int[] time = { 0, 0 };
    private int[] nextTime = { 0, 0 };
    private int request = -1;
    private bool[] once = { true, true };

    // Emregency car
    private  int[] currentEmergencyDirection = { -1, -1 };
    private float[] EmegencyTimeVariable = { 0, 0};

    private bool start = true;


    public void Start()
    {
        // Initialize variables
        AI_two_multi.startCouting = false;

        // If the selected sysetm is not this system, then disable this script
        if (Scence_Manger.algorthim != "AI(multi) based Traffic Light System")
        {
            GetComponent<AI_two_multi>().enabled = false;
            return;
        }

        start = true;

        // traffic lights
        trafficLights = new GameObject[2,4];
        for (int j = 0; j < 4; j++)
        {
            trafficLights[0, j] = transform.GetComponent<AI_TLC_two_multi>().trafficLightsIntersection0[j];
            trafficLights[1, j] = transform.GetComponent<AI_TLC_two_multi>().trafficLightsIntersection1[j];
        }


        // Initialize Car Counter
        CarCount = new CarCounter[2, 4];
        for (int j = 0; j < 4; j++)
        {
            CarCount[0, j] = transform.GetComponent<AI_TLC_two_multi>().CarCountIntersetion0[j];
            CarCount[1, j] = transform.GetComponent<AI_TLC_two_multi>().CarCountIntersetion1[j];
        }

        // Reset Lights
        for (int i = 0;i < 2; i++)
        {
            ChangeLightRed(i, 0);
            ChangeLightRed(i, 1);
            ChangeLightRed(i, 2);
            ChangeLightRed(i, 3);
        }
        

    }

    public void FixedUpdate()
    {   

        // Start when cars are generated
        if (CarCount[0,0].carsCounter + CarCount[0, 1].carsCounter + CarCount[0, 2].carsCounter + CarCount[0, 3].carsCounter != 0 || CarCount[1, 0].carsCounter + CarCount[1, 1].carsCounter + CarCount[1, 2].carsCounter + CarCount[1, 3].carsCounter != 0)
        {
            // check if the episode finish
            if (Avg_wating_time_two.numberOfCars == 0  || Avg_wating_time_two.Avg_wating >= 110)
            {

                // Episode finish, Set reward according to result
                SetReward(1 - (Avg_wating_time_two.Avg_wating / 110));

                // End Episode
                EndEpisode();

            }
            else
            {
                // Episode still running
                TrafficLightControlling(0);
                TrafficLightControlling(1);

            }
        }
    }


    // This funcitonis called on the beginning of the episode
    public override void OnEpisodeBegin()
    {
        
      
    }

    // Observatison (number of cars)
    public override void CollectObservations(VectorSensor sensor)
    {
        if (request != -1) {
            sensor.AddObservation(CarCount[0, 0].carsCounter);
            sensor.AddObservation(CarCount[0, 1].carsCounter);
            sensor.AddObservation(CarCount[0, 2].carsCounter);
            sensor.AddObservation(CarCount[0, 3].carsCounter);

            sensor.AddObservation(CarCount[1, 0].carsCounter);
            sensor.AddObservation(CarCount[1, 1].carsCounter);
            sensor.AddObservation(CarCount[1, 2].carsCounter);
            sensor.AddObservation(CarCount[1, 3].carsCounter);

            sensor.AddObservation(direct[(request+1)%2]);
            sensor.AddObservation(time[(request + 1) % 2]);
        }

    }

    // Called when action is requested
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Direction
        nextDirect[request] = actions.DiscreteActions[(request* 2)];

        // Time
        int temp = actions.DiscreteActions[(request * 2)+1];
        // If the time is less that 4 make it zero
        if (temp < 4)
        {
           nextTime[request] = 0;
        }
        else
        {
            nextTime[request] = temp;
        }

        request = -1;

    }

    private void TrafficLightControlling(int intersection)
    {
        checkEmeregency(intersection);
        if (currentEmergencyDirection[intersection] != -1)
        {
            return;
        }

        // Start when cars are generated
        if (!start || CarCount[0, 0].carsCounter + CarCount[0,1].carsCounter  + CarCount[0, 2].carsCounter + CarCount[0, 3].carsCounter + CarCount[1, 0].carsCounter + CarCount[1, 1].carsCounter + CarCount[1, 2].carsCounter + CarCount[1, 3].carsCounter != 0) {

            start = false;
            if (time[intersection] == 0 || time[intersection] == -1)
            {
                if (nextTime[intersection] != 0 && nextTime[intersection] != -1)
                {
                    time[intersection] = nextTime[intersection];
                    direct[intersection] = nextDirect[intersection];
                    nextTime[intersection] = -1;
                    nextDirect[intersection] = -1;
                }
                else
                {
                    if (request == -1)
                    {
                        request = intersection;
                        RequestDecision();
                    }

                }
                return;

            }

            if (AI_two_multi.startCouting == false)
            {
                AI_two_multi.startCouting = true;
            }
            timeVariable[intersection] += Time.deltaTime;
            if (timeVariable[intersection] < time[intersection] - yellowLightDuration )
            {

                ChangeLightGreen(intersection, direct[intersection]);
                if (User_Controll.Intersection == intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                    timer.GetComponentInChildren<TMP_Text>().text = "AI";
                }

            }
            else if (timeVariable[intersection] >= time[intersection] - yellowLightDuration - 1 && timeVariable[intersection] < time[intersection] - yellowLightDuration)
            {   

                // Request a descion in the last second of green time
               if (once[intersection] && request == -1)
               {
                   once[intersection] = false;
                   request = intersection;
                   RequestDecision();
               }

            }
            else if (timeVariable[intersection] >= time[intersection] - yellowLightDuration && timeVariable[intersection] < time[intersection])
            {
                if (User_Controll.Intersection == intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
                }

                // If the next direction is the same as the current one
                if (direct[intersection] == nextDirect[intersection])
                {
                   time[intersection] = nextTime[intersection];
                   direct[intersection] = nextDirect[intersection];
                   nextTime[intersection] = -1;
                    nextDirect[intersection] = -1;
                    timeVariable[intersection] = 0;
                    once[intersection] = true;
                }
                else
               {
                    ChangeLightYellow(intersection, direct[intersection]);
                }
                

            }
            else if (timeVariable[intersection] >= time[intersection] && timeVariable[intersection] < time[intersection] + delay)
            {

                ChangeLightRed(intersection, direct[intersection]);

            }
            else
            {
                //Rest Variables
                time[intersection] = nextTime[intersection];
                direct[intersection] = nextDirect[intersection];
                nextTime[intersection] = -1;
                nextDirect[intersection] = -1;
                timeVariable[intersection] = 0;
                once[intersection] = true;
                if (request == -1)
                {
                    request = intersection;
                    RequestDecision();
                }
                
            }
        }
    }


    /// <summary>
    /// Method <c>checkEmeregency</c> Check if there is an emergency car and handle it.
    /// </summary>
    /// <param name="intersection">the intersection number</param>
    public void checkEmeregency(int intersection)
    {

        // This piece of code job is to check if there is emergency car if true it wil give it priority without specific time 
        if (currentEmergencyDirection[intersection] != -1)
        {
            if (AI_two_single.startCouting == false)
            {
                AI_two_single.startCouting = true;
            }
            EmegencyTimeVariable[intersection] += Time.deltaTime;
            if (User_Controll.Intersection == intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().text = "EM";
            }
            if (EmegencyTimeVariable[intersection] <= yellowLightDuration)
            {
                ChangeLightYellow(intersection, direct[intersection]);
                if (User_Controll.Intersection == intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
                }
            }

            if (EmegencyTimeVariable[intersection] >= yellowLightDuration && EmegencyTimeVariable[intersection] < yellowLightDuration + delay)
            {
                ChangeLightRed(intersection,direct[intersection]);
            }

            if (EmegencyTimeVariable[intersection] >= yellowLightDuration + delay)
            {
                if (CarCount[intersection,currentEmergencyDirection[intersection]].emergencyExist)
                {
                    ChangeLightGreen(intersection,currentEmergencyDirection[intersection]);
                    if (User_Controll.Intersection == intersection)
                    {
                        timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                    }
                }
                else
                {
                    timeVariable[intersection] = 0f;
                    EmegencyTimeVariable[intersection] = 0;
                    time[intersection] = 0;
                    direct[intersection] = currentEmergencyDirection[intersection];
                    time[intersection] = (int)yellowLightDuration + 1;
                    currentEmergencyDirection[intersection] = -1;

                }
            }
            return;
        }
        else
        {   

            // Check for emegency cars
            for (int i = 0; i < 4; i++)
            {
                if (CarCount[intersection,i].emergencyExist)
                {
                    currentEmergencyDirection[intersection] = i;
                    if (currentEmergencyDirection[intersection] == direct[intersection])
                    {
                        EmegencyTimeVariable[intersection] = yellowLightDuration + delay;
                    }
                    return;
                }
            }
        }

    }

    /// <summary>
    /// Method <c>ChangeLightRed</c> make the current traffic light red.
    /// </summary>
    /// <param name="to">the current traffic light to be red</param>
    public void ChangeLightRed(int intersection, int light)
    {

        trafficLights[intersection,light].GetComponent<Light_Conteroler>().chagneToRed();

    }

    /// <summary>
    /// Method <c>ChangeLightYellow</c> make the current traffic light yellow.
    /// </summary>
    /// <param name="to">the current traffic light to be yellow</param>
    private void ChangeLightYellow(int intersection, int light)
    {

        trafficLights[intersection, light].GetComponent<Light_Conteroler>().chagneToYellow();

    }

    /// <summary>
    /// Method <c>ChangeLightGreen</c> make the next traffic light green.
    /// </summary>
    /// <param name="to">the next traffic light to be green</param>
    private void ChangeLightGreen(int intersection, int light)
    {
        for (int i = 0; i < 4; i++)
        {
            ChangeLightRed(intersection, i);
        }
        trafficLights[intersection, light].GetComponent<Light_Conteroler>().chagneToGreen();
        cameraController.GetComponent<User_Controll>().updateCameras(light, intersection);

    }

}
