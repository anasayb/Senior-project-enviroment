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
    //private AI_TLC controller;
    //public GameObject prefab;
    public int episodeNumber = 0;
    //public GameObject cars;


    public void Start()
    {
        
    }

    public void FixedUpdate()
    {   
        if (Avg_wating_time.numberOfCars == 0 || Avg_wating_time.Avg_wating >= 300)
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
            if (GetComponent<AI_TLC>().finish)
            {
                if (episodeNumber != 0)
                {
                    SetReward(-(Avg_wating_time.Avg_wating/300));
                }
                RequestDecision();

                GetComponent<AI_TLC>().finish= false;
            }

                
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

        GetComponent<AI_TLC>().direct = actions.DiscreteActions[0];

        int temp = actions.DiscreteActions[1];
        if (temp < 4)
        {
            GetComponent<AI_TLC>().time = 0;
        }
        else
        {
            GetComponent<AI_TLC>().time = temp;
        }

    }


}
