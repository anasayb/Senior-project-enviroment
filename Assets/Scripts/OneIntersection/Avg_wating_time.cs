
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

// Used to store the waiting tiem of cars
public struct data
{
    public float waiting_time;
    public string streat;
    public string direction;
}


public class Avg_wating_time : MonoBehaviour
{

    public static float Avg_wating = 0;
    public static float numberOfCars = 0;
    public static Dictionary<string, data> waitingTimes;
    public static float RunningTime;
    public static float FlowRate;
    public static bool FlowCalcualted;
    public static float[] congestion;

    public CarCounter[] streets;

    [Header("GUI")]
    public GameObject summary;
    public GameObject runningTimeText;
    public GameObject CarInfo;
    public GameObject timer;

    private bool stored = false;
    private float timeVariable = 0;

    // Start is called before the first frame update
    void Start()
    {   
        // inizlize variables
        Avg_wating_time.Avg_wating = 0;
        Avg_wating_time.numberOfCars = 0;
        Avg_wating_time.waitingTimes = new Dictionary<string, data>();
        Avg_wating_time.RunningTime = 0;
        Avg_wating_time.FlowRate = 0;
        Avg_wating_time.FlowCalcualted = false;
        Avg_wating_time.congestion = new float[4];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // calculate teh number of cars in the simulation at the moment
        calculateTheNumberOfCar();


        // Calculate the Average waiting time
        Cal();

       

        // If all cars are disapeared, then finish the simulation
        if (numberOfCars == 0)
        {
            // Calculate the traffic flow if the running time is less than 1 min
            if (!FlowCalcualted)
            {
                FlowRate = (streets[0].leaveCarsCounter + streets[1].leaveCarsCounter + streets[2].leaveCarsCounter + streets[3].leaveCarsCounter);
                FlowCalcualted = true;
            }

            //Text.GetComponent<TMP_Text>().color = new Color(0.039f, 0.545f, 0.039f);
            if (!stored)
            {
                // Database
                DatabaseConnection db = GameObject.Find("Database").GetComponent<DatabaseConnection>();

                // Change the GUI
                CarInfo.SetActive(false);
                timer.SetActive(false);
                summary.SetActive(true);
                runningTimeText.SetActive(false);

                // Avreging the congestion
                for(int i = 0; i < congestion.Length; i++)
                {
                   Avg_wating_time.congestion[i] = Avg_wating_time.congestion[i] / (((int)(RunningTime * 100)) / 100f);
                }

                // store the run inforamtion and show the summary
                StartCoroutine(db.SaveWatingTime(new Dictionary<string, data>(waitingTimes), GameObject.Find("Traffic Lights"))); ;
                //while (e.MoveNext()) ;
                
                // Dispaly the summary
                Summary.CurrentRunSummary();
                stored = true;

            }
        }
        else
        {

            // Increase the running time of the simulation
            Avg_wating_time.RunningTime += Time.deltaTime;
            runningTimeText.GetComponent<TMP_Text>().text = "Running Time: " + Avg_wating_time.RunningTime.ToString("F2") + " s";

            // Calculate the traffic flow
            if (Avg_wating_time.RunningTime >= 60 && !FlowCalcualted)
            {
                FlowRate = (streets[0].leaveCarsCounter+streets[1].leaveCarsCounter + streets[2].leaveCarsCounter + streets[3].leaveCarsCounter);
                FlowCalcualted = true;
            }

            // each second record the number of cars
            timeVariable += Time.deltaTime;
            if (timeVariable >= 1)
            {
                // Calculate the congetsion
                for (int i = 0; i < streets.Length; i++)
                {
                    congestion[i] += streets[i].carsCounter;
                }
                timeVariable = 0;
            }
            

        }

    }


    /// <summary>
    /// Method <c>updateAvg</c> update the value of the wating time of a car in the Dictionary.
    /// </summary>
    /// <param name="name">the name of the car to update its watining time</param>
    /// <param name="waitingTime">the new waiting time</param>
    public static void updateAvg(string name, float waitingTime, bool left, bool right, string st)
    {
        if (waitingTimes.ContainsKey(name))
        {
            data t = waitingTimes[name];
            t.waiting_time = waitingTime;
            waitingTimes[name] = t;
        }
        else
        {
            data temp = new data();
            temp.waiting_time = waitingTime;
            temp.streat = st;
            if (left)
            {
                temp.direction = "Left";
            }
            else if (right)
            {
                temp.direction = "Right";
            }
            else
            {
                temp.direction = "None";
            }

            waitingTimes.Add(name, temp);

        }


    }


    /// <summary>
    /// Method <c>increaseCarNumber</c> increase the number of cars that are destroyed.
    /// </summary>
    public void calculateTheNumberOfCar()
    {
        numberOfCars = 0;
        foreach (Transform childe in transform)
        {
            foreach (Transform car in childe)
            {
                numberOfCars++;
            }

        }
    }


    /// <summary>
    /// Method <c>Cal</c> calulate the avrage waiting time.
    /// </summary>
    private void Cal()
    {
        if (waitingTimes.Count == 0)
        {
            Avg_wating = 0;
            return;
        }

        float sum = 0;
        foreach (var item in waitingTimes)
        {
            sum += item.Value.waiting_time;
        }

        Avg_wating = sum / waitingTimes.Count;
    }


}
