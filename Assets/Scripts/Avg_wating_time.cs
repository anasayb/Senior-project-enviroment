
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public struct data
{
    public float waiting_time;
    public string streat;
    public string direction;
}


public class Avg_wating_time : MonoBehaviour
{
    

    public float Avg_wating = 0;
    public float numberOfCars = 0;
	
    //public GameObject Text;
    public GameObject Summery;
    public GameObject CarInfo;
    public GameObject timer;


    private Dictionary<string, data> waitingTimes;
    private bool stored = false;


    // Start is called before the first frame update
    void Start()
    {
        calculateTheNumberOfCar();
        waitingTimes = new Dictionary<string, data>();
        Avg_wating = 0;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //
        calculateTheNumberOfCar();


        // Calculate the Average waiting time
        Cal();

        // Prepare the string to print

        //string t = "Average Waiting Time:\n" + Avg_wating.ToString() + " Seconds";
        //Text.GetComponent<TMP_Text>().color = new Color(0f, 0f, 0f);
        //Text.GetComponent<TMP_Text>().text = t;

        // If all cars are disapeared chagn the color of the text to green
        // numberOfCars == 0 && transform.GetComponent<Car_Generator>().CarsToGenerate == 0

        if (numberOfCars <= 0)
        {
            //Text.GetComponent<TMP_Text>().color = new Color(0.039f, 0.545f, 0.039f);
            if (stored)
            {
                //DatabaseConnection db = GameObject.Find("Database").GetComponent<DatabaseConnection>();
				
                //StartCoroutine(db.SaveWatingTime(new Dictionary<string, data>(waitingTimes), GameObject.Find("Traffic Lights")));
                stored = true;
                summeryPanel();

            }
        }
            
    }


    /// <summary>
    /// Method <c>updateAvg</c> update the value of the wating time of the car in the Dictionary.
    /// </summary>
    /// <param name="name">the name of the car to update its watining time</param>
    /// <param name="waitingTime">the new waiting time</param>
    public void updateAvg(string name, float waitingTime, bool left, bool right, string st)
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


    private void summeryPanel()
    {   
        CarInfo.SetActive(false);
        timer.SetActive(false);
        Summery.SetActive(true);

        // Name of the method

        Summery.transform.Find("TLC").Find("Algo Name").GetComponent<TMP_Text>().text = "Traditional Traffic Light System";

        // Starting Direction
        string[] names = { "North", "West", "South", "East"};
        Summery.transform.Find("Direction").Find("Direction").GetComponent<TMP_Text>().text = names[Scence_Manger.dir];

        // Avg_waiting
        Summery.transform.Find("AVG").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating * 100))/100f).ToString("F2") + " s";

        // Max waiting
        Summery.transform.Find("Max Waiting Time").Find("Time").GetComponent<TMP_Text>().text = (((int)(maxWaiting() * 100)) / 100f).ToString("F2") + " s";

        // Cars Number
        Summery.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = Scence_Manger.startingNumberOfCars.ToString();

        // Cars informations
        GameObject cont = Summery.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        foreach (var item in waitingTimes)
        {
            GameObject newRow = GameObject.Instantiate(row);
            // Car name
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = item.Key;
            // Waiting Time
            newRow.transform.GetChild(1).GetComponent<TMP_Text>().text = (((int)(item.Value.waiting_time * 100)) / 100f).ToString("F2");
            // Start Direction
            newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = item.Value.streat;
            // Turning
            newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = item.Value.direction;
            newRow.transform.SetParent(cont.transform);
        }


        // Destroy the template record
        row.SetActive(false);
    }

    private float maxWaiting()
    {
        float max = 0;
        foreach (var item in waitingTimes)
        {
            if (item.Value.waiting_time > max)
            {
                max = item.Value.waiting_time;
            }
        }

        return max;
    }

}
