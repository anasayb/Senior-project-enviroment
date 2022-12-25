
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct data
{
    public float waiting_time;
    public string streat;
    public string direction;
}


public class Avg_wating_time : MonoBehaviour
{

    public static float Avg_wating = 0;
    public GameObject Text;

    private static Dictionary<string, data> waitingTimes;
    private static float numberOfCars = 0;
    private bool stored = false;

    // Start is called before the first frame update
    void Start()
    {
        waitingTimes= new Dictionary<string, data>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //
        calculateTheNumberOfCar();

        // Calculate the Average waiting time
        Cal();

        // Prepare the string to print
        string t = "Average Waiting Time:\n" + Avg_wating.ToString() + " Seconds";
        Text.GetComponent<TMP_Text>().color = new Color(0f, 0f, 0f);
        Text.GetComponent<TMP_Text>().text = t;

        // If all cars are disapeared chagn the color of the text to green
        // numberOfCars == 0 && transform.GetComponent<Car_Generator>().CarsToGenerate == 0
        if (numberOfCars == 0)
        {
            Text.GetComponent<TMP_Text>().color = new Color(0.039f, 0.545f, 0.039f);
            if (!stored)
            {
                DatabaseConnection db = GameObject.Find("Database").GetComponent<DatabaseConnection>();
                StartCoroutine(db.SaveWatingTime(new Dictionary<string, data>(waitingTimes)));
                stored = true;
            }
        }
            
    }


    /// <summary>
    /// Method <c>updateAvg</c> update the value of the wating time of the car in the Dictionary.
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
            else if(right)
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
    public  void calculateTheNumberOfCar()
    {
        numberOfCars= 0;
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
        foreach(var item in waitingTimes)
        {
            sum+= item.Value.waiting_time;
        }
        
        Avg_wating = sum / waitingTimes.Count;
    }
}
