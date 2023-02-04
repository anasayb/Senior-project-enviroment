
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;


public struct dataTwoIntersection
{
    public float[] waiting_time;
    public string streat;
    public string Turn1;
    public string Turn2;

}


public class Avg_wating_time_two : MonoBehaviour
{

    public static float Avg_wating = 0;
    public static float numberOfCars = 0;
    public static Dictionary<string, dataTwoIntersection> waitingTimes;
    public static float RunningTime;
    public static float[] FlowRate = { 0, 0 };
    public static bool FlowCalcualted;
    public static float[][] congestion;

    private CarCounter[][] streets;
    private bool stored = false;
    private float timeVariable = 0;

    [Header("GUI")]
    public GameObject summary;
    //public GameObject AvgWaitingTimeText;
    public GameObject runningTimeText;
    public GameObject CarInfo;
    public GameObject timer;

    // Start is called before the first frame update
    void Start()
    {
        Avg_wating_time_two.Avg_wating = 0;
        Avg_wating_time_two.numberOfCars = 0;
        Avg_wating_time_two.waitingTimes = new Dictionary<string, dataTwoIntersection>();
        Avg_wating_time_two.RunningTime = 0;
        Avg_wating_time_two.FlowRate[0] =  0;
        Avg_wating_time_two.FlowRate[1] = 0;
        Avg_wating_time_two.FlowCalcualted = false;
        Avg_wating_time_two.congestion = new float[2][];
        Avg_wating_time_two.congestion[0] = new float[4];
        Avg_wating_time_two.congestion[1] = new float[4];
        for (int i = 0; i < 4; i++)
        {
            Avg_wating_time_two.congestion[0][i] = 0;
            Avg_wating_time_two.congestion[1][i] = 0;
        }

        streets = new CarCounter[2][];
        streets[0] = new CarCounter[4];
        streets[1] = new CarCounter[4];
        // streats
        GameObject intersection0 = GameObject.Find("Intersection0");
        streets[0][0] = intersection0.transform.Find("Turning Paths").GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<CarCounter>();
        streets[0][1] = intersection0.transform.Find("Turning Paths").GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<CarCounter>();
        streets[0][2] = intersection0.transform.Find("Turning Paths").GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<CarCounter>();
        streets[0][3] = intersection0.transform.Find("Turning Paths").GetChild(2).GetChild(3).GetChild(0).GetChild(0).GetComponent<CarCounter>();

        GameObject intersection1 = GameObject.Find("Intersection1");
        streets[1][0] = intersection1.transform.Find("Turning Paths").GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<CarCounter>();
        streets[1][1] = intersection1.transform.Find("Turning Paths").GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<CarCounter>();
        streets[1][2] = intersection1.transform.Find("Turning Paths").GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<CarCounter>();
        streets[1][3] = intersection1.transform.Find("Turning Paths").GetChild(2).GetChild(3).GetChild(0).GetChild(0).GetComponent<CarCounter>();



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
                FlowRate[0] = (streets[0][0].leaveCarsCounter + streets[0][1].leaveCarsCounter + streets[0][2].leaveCarsCounter + streets[0][3].leaveCarsCounter);
                FlowRate[1] = (streets[1][0].leaveCarsCounter + streets[1][1].leaveCarsCounter + streets[1][2].leaveCarsCounter + streets[1][3].leaveCarsCounter);
                FlowCalcualted = true;
            }

            //Text.GetComponent<TMP_Text>().color = new Color(0.039f, 0.545f, 0.039f);
            if (!stored)
            {
                // Database
                DatabaseConnectionTwo db = GameObject.Find("Database").GetComponent<DatabaseConnectionTwo>();

                // Change the GUI
                CarInfo.SetActive(false);
                timer.SetActive(false);
                summary.SetActive(true);
                runningTimeText.SetActive(false);

                // Avreging the congestion
                for(int i = 0; i < 4; i++)
                {
                   Avg_wating_time_two.congestion[0][i] = Avg_wating_time_two.congestion[0][i] / (((int)(RunningTime * 100)) / 100f);
                   Avg_wating_time_two.congestion[1][i] = Avg_wating_time_two.congestion[1][i] / (((int)(RunningTime * 100)) / 100f);
                }

                // store the run inforamtion and show the summary
                StartCoroutine(db.SaveWatingTime(new Dictionary<string, dataTwoIntersection>(waitingTimes))); ;
                //while (e.MoveNext()) ;
                
                SummaryTwo.CurrentRunSummary();
                stored = true;

            }
        }
        else
        {

            // Increase the running time of the simulation
            Avg_wating_time_two.RunningTime += Time.deltaTime;
            runningTimeText.GetComponent<TMP_Text>().text = "Running Time: " + Avg_wating_time_two.RunningTime.ToString("F2") + " s";

            // Calculate the traffic flow
            if (Avg_wating_time_two.RunningTime >= 60 && !FlowCalcualted)
            {
                FlowRate[0] = (streets[0][0].leaveCarsCounter + streets[0][1].leaveCarsCounter + streets[0][2].leaveCarsCounter + streets[0][3].leaveCarsCounter);
                FlowRate[1] = (streets[1][0].leaveCarsCounter + streets[1][1].leaveCarsCounter + streets[1][2].leaveCarsCounter + streets[1][3].leaveCarsCounter);
                FlowCalcualted = true;
            }

            // each second record the number of cars
            timeVariable += Time.deltaTime;
            if (timeVariable >= 1)
            {
                // Calculate the congetsion
                for (int i = 0; i < 4; i++)
                {
                    congestion[0][i] += streets[0][i].carsCounter;
                    congestion[1][i] += streets[1][i].carsCounter;
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
    public static void updateAvg(string name, float waitingTime, bool left, bool right, bool left2, bool right2, int intersection, string st)
    {
        if (waitingTimes.ContainsKey(name))
        {
            dataTwoIntersection t = waitingTimes[name];
            t.waiting_time[intersection] = waitingTime;
            waitingTimes[name] = t;
        }
        else
        {
            dataTwoIntersection temp = new dataTwoIntersection();
            temp.waiting_time = new float[2];
            temp.waiting_time[0] = temp.waiting_time[1] = - 1;
            temp.waiting_time[intersection] = waitingTime;
            temp.streat = st;
            if (left)
            {
                temp.Turn1 = "Left";
            }
            else if (right)
            {
                temp.Turn1 = "Right";
            }
            else
            {
                temp.Turn1 = "None";
            }


            if (left2)
            {
                temp.Turn1 = "Left";
            }
            else if (right2)
            {
                temp.Turn2 = "Right";
            }
            else
            {
                temp.Turn2 = "None";
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
            foreach (Transform intersection in transform)
            {
                foreach (Transform childe in intersection)
                {
                    foreach (Transform car in childe)
                    {
                        numberOfCars++;
                    }

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
            if (item.Value.waiting_time[0] != -1)
            {
                sum += item.Value.waiting_time[0];
            }
            if (item.Value.waiting_time[1] != -1)
            {
                sum += item.Value.waiting_time[1];
            }
            
        }


        Avg_wating = sum / waitingTimes.Count;
    }


}
