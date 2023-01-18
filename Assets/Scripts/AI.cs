using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEditor;

public class AI : Agent
{
    //private AI_TLC controller;
    public GameObject prefab;
    public GameObject cars;
    public GameObject trafficLightController;

    public int episodeNumber = 0;

    public GameObject newEnv;

    private bool envBuild = false;

    public void Start()
    {

    }

    public void FixedUpdate()
    {
        if (newEnv == null)
        {
            return;
        }

        if (cars.GetComponent<Avg_wating_time>().numberOfCars == 0 || cars.GetComponent<Avg_wating_time>().Avg_wating >= 300)
        {
            if (cars.GetComponent<Avg_wating_time>().numberOfCars == 0)
            {
                SetReward(1f);
            }
            else
            {
                SetReward(-1f);
            }

            envBuild = false;
            EndEpisode();
            //Destroy(temp);
            //cars.GetComponent<Avg_wating_time>().Avg_wating = 0;
 
        }
        else
        {
            if (trafficLightController.GetComponent<AI_TLC>().finish)
            {
                if (episodeNumber != 0)
                {
                    SetReward(-(cars.GetComponent<Avg_wating_time>().Avg_wating/300));
                }
                RequestDecision();

                trafficLightController.GetComponent<AI_TLC>().finish= false;
            }

                
        }
    }

    public override void OnEpisodeBegin()
    {
        
        
        Debug.Log("ep: " + episodeNumber);
        episodeNumber++;

        
        
        //GameObject temp = currentEnv;
        
        if (newEnv != null)
        {
            Destroy(newEnv);
        }

        newEnv = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero));

        //GameObject newEnv = Instantiate(TrainEnvPrefab, new Vector3(0, 0, 0), Quaternion.Euler(Vector3.zero));
        newEnv.name = "Env";
        newEnv.transform.SetParent(transform.parent);
        newEnv.transform.localPosition = new Vector3(0,0,0);
        cars = newEnv.transform.Find("Cars").gameObject;
        int num = Random.Range(1,181);
        cars.GetComponent<Car_Generator>().CarsToGenerate = 24;
        cars.GetComponent<Avg_wating_time>().numberOfCars = 24;
        trafficLightController = newEnv.transform.Find("Turning Paths").Find("Intersection 1").Find("Traffic Lights").gameObject;

        cars.GetComponent<Car_Generator>().generate();

        cars = newEnv.transform.Find("Cars").gameObject;
        trafficLightController.GetComponent<AI_TLC>().finish = true;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(trafficLightController.GetComponent<AI_TLC>().CarCount[0].carsCounter);
        sensor.AddObservation(trafficLightController.GetComponent<AI_TLC>().CarCount[1].carsCounter);
        sensor.AddObservation(trafficLightController.GetComponent<AI_TLC>().CarCount[2].carsCounter);
        sensor.AddObservation(trafficLightController.GetComponent<AI_TLC>().CarCount[3].carsCounter);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {

        trafficLightController.GetComponent<AI_TLC>().direct = actions.DiscreteActions[0];

        int temp = actions.DiscreteActions[1];
        if (temp < 4)
        {
            trafficLightController.GetComponent<AI_TLC>().time = 0;
        }
        else
        {
            trafficLightController.GetComponent<AI_TLC>().time = temp;
        }

    }


}
