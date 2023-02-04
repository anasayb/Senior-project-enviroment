using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class AI_two_multi : Agent
{
    public static bool startCouting = false;

    [Header("Cameras")]
    //public GameObject Maincameras;
    public GameObject cameraController;

    [Header("GUI")]
    public GameObject timer;


    // Enviroment Varaibles
    //public GameObject prefab;
    //public GameObject cars;
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

    // for making things fast
    // private float initialTimeScale = 10f;

    public void Start()
    {
        AI_two_multi.startCouting = false;

        if (Scence_Manger.algorthim != "AI Traffic Light System (multi)")
        {
            GetComponent<AI_two_multi>().enabled = false;
            return;
        }

        // traffic lights
        trafficLights = new GameObject[2,4];
        for (int j = 0; j < 4; j++)
        {
            trafficLights[0, j] = transform.GetComponent<AI_TLC_two_multi>().trafficLightsIntersection0[j];
            trafficLights[1, j] = transform.GetComponent<AI_TLC_two_multi>().trafficLightsIntersection1[j];
        }



        CarCount = new CarCounter[2, 4];
        for (int j = 0; j < 4; j++)
        {
            CarCount[0, j] = transform.GetComponent<AI_TLC_two_multi>().CarCountIntersetion0[j];
            CarCount[1, j] = transform.GetComponent<AI_TLC_two_multi>().CarCountIntersetion1[j];
        }

        // Prepare
        // Reset Lights
        for (int i = 0;i < 2; i++)
        {
            ChangeLightRed(i, 0);
            ChangeLightRed(i, 1);
            ChangeLightRed(i, 2);
            ChangeLightRed(i, 3);
        }
        


        //RequestDecision();

    }

    public void FixedUpdate()
    {
        if (CarCount[0,0].carsCounter + CarCount[0, 1].carsCounter + CarCount[0, 2].carsCounter + CarCount[0, 3].carsCounter != 0 || CarCount[1, 0].carsCounter + CarCount[1, 1].carsCounter + CarCount[1, 2].carsCounter + CarCount[1, 3].carsCounter != 0)
        {
            // check if the episode finish
            if (Avg_wating_time_two.numberOfCars == 0  || Avg_wating_time_two.Avg_wating >= 200)
            {

                // Episode finish, Set reward according to result
                SetReward(1 - (Avg_wating_time_two.Avg_wating / 200));

                // End Episode
                //EndEpisode();

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
        
        /*
        Debug.Log("ep: " + episodeNumber);
        episodeNumber++;

        if (episodeNumber != 1)
        {   
            // Destroy old cars
            foreach (Transform dir in car1.transform)
            {
                foreach (Transform obj in dir)
                {
                    Destroy(obj.gameObject);
                }
            }

            foreach (Transform dir in car2.transform)
            {
                foreach (Transform obj in dir)
                {
                    Destroy(obj.gameObject);
                }
            }


            // Reset Lights
            for (int i = 0; i < 2; i++)
            {
                ChangeLightRed(i, 0);
                ChangeLightRed(i, 1);
                ChangeLightRed(i, 2);
                ChangeLightRed(i, 3);
            }


            // Set the seed value
            Random.InitState(System.DateTime.Now.Millisecond);
            int num = Random.Range(1, 51); 
            Debug.Log("Number: " + num);
            //int num1 = Random.Range(1,num);
            //int num2 = Mathf.Max( 0,num-num1);

            car1.GetComponent<Car_Generator>().CarsToGenerate = num;
            car2.GetComponent<Car_Generator>().CarsToGenerate = num;
            car2.GetComponent<Car_Generator>().NameCarNumber = num+1;
            car2.GetComponent<Car_Generator>().NameBusNumber = num+1;
            car2.GetComponent<Car_Generator>().NameEmergencyNumber = num+1;
            car2.GetComponent<Car_Generator>().NameTruckNumber = num+1;

            cars.GetComponent<Avg_wating_time>().numberOfCars = num;
            cars.GetComponent<Avg_wating_time>().reset();

            car1.GetComponent<Car_Generator>().generate(num);
            car2.GetComponent<Car_Generator>().generate(num+4);

        }

        // Resest variabels
        time[0] = 0;
        time[1] = 0;
        direct[0] = 0;
        direct[1] = 0;
        timeVariable[0] = 0;
        timeVariable[1] = 0;
        start = true;
        //request = 0;
        //RequestDecision();
        //request = 1;
        //RequestDecision();
        */
    }

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

            sensor.AddObservation(direct[request]);
            sensor.AddObservation(time[request]);
        }

    }


    public override void OnActionReceived(ActionBuffers actions)
    {

        nextDirect[request] = actions.DiscreteActions[(request* 2)];

        int temp = actions.DiscreteActions[(request * 2)+1];
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
            if (timeVariable[intersection] < time[intersection] - yellowLightDuration - 1)
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
                if (once[intersection])
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
                if (direct == nextDirect)
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
                //ChangeLightRed(direct);
                time[intersection] = nextTime[intersection];
                direct[intersection] = nextDirect[intersection];
                nextTime[intersection] = -1;
                nextDirect[intersection] = -1;
                timeVariable[intersection] = 0;
                once[intersection] = true;
                //request = intersection;
                //RequestDecision();
            }
        }
    }


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
