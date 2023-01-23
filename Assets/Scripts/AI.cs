using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AI : Agent
{
    // Enviroment Varaibles
    public GameObject prefab;
    public GameObject cars;
    public GameObject newEnv;
    public int episodeNumber = 0;


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


    // for making things fast
    // private float initialTimeScale = 10f;

    public void Start()
    {
        //Time.timeScale = initialTimeScale;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;


        // Set variables
        cars = transform.parent.Find("Train").Find("Cars").gameObject;
        cars.transform.name = "Cars " + transform.parent.name;
        trafficLights = transform.parent.Find("Train").GetComponent<AI_TLC>().trafficLights;
        CarCount = transform.parent.Find("Train").GetComponent<AI_TLC>().CarCount;

        // Prepare
        // Reset Lights
        for (int i = 0; i < 4; i++)
        {
            ChangeLightRed(i);
        }

        // Set the seed value
        //Random.InitState(System.DateTime.Now.Millisecond);
        //int num = Random.Range(1, 24);
        int num = 23;
        cars.GetComponent<Car_Generator>().CarsToGenerate = num;
        cars.GetComponent<Avg_wating_time>().numberOfCars = num;
        cars.GetComponent<Avg_wating_time>().reset();
        cars.GetComponent<Car_Generator>().generate();

    }

    public void FixedUpdate()
    {   

        // check if the episode finish
        if (cars.GetComponent<Avg_wating_time>().numberOfCars == 0 || cars.GetComponent<Avg_wating_time>().Avg_wating >= 100)
        {   

            // Episode finish, Set reward according to result
            if (cars.GetComponent<Avg_wating_time>().numberOfCars == 0)
            {
                SetReward(1f);
            }
            else
            {
                SetReward(-1f);
            }

            // End Episode
            EndEpisode();
 
        }
        else
        {
            // Episode still running
            TrafficLightControlling();
               
        }
    }


    // This funcitonis called on the beginning of the episode
    public override void OnEpisodeBegin()
    {
        
        
        Debug.Log("ep: " + episodeNumber);
        episodeNumber++;

        if (episodeNumber != 1)
        {
            // Reset Lights
            for (int i = 0; i < 4; i++)
            {
                ChangeLightRed(i);
            }

            //Random.InitState(System.DateTime.Now.Millisecond);
            //int num = Random.Range(1, 24);
            //Debug.Log("Number: " + num);
            int num = 23;
            cars.GetComponent<Car_Generator>().CarsToGenerate = num;
            cars.GetComponent<Avg_wating_time>().numberOfCars = num;
            cars.GetComponent<Avg_wating_time>().reset();
            cars.GetComponent<Car_Generator>().generate();
           
        }

        // Resest variabels
        time = 0;
        direct = 0;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        sensor.AddObservation(CarCount[0].carsCounter);
        sensor.AddObservation(CarCount[1].carsCounter);
        sensor.AddObservation(CarCount[2].carsCounter);
        sensor.AddObservation(CarCount[3].carsCounter);
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
        if (timeVariable < time - yellowLightDuration -1)
        {

            ChangeLightGreen(direct);

        }else if (timeVariable >= time - yellowLightDuration - 1 && timeVariable < time - yellowLightDuration)
        {
            if (once)
            {
                once = false;
                SetReward(-(cars.GetComponent<Avg_wating_time>().Avg_wating / 100));
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
