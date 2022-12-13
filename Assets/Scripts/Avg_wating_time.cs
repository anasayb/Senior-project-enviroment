using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Avg_wating_time : MonoBehaviour
{

    public static float Avg_wating = 0;
    public GameObject Text;

    private static Dictionary<string, float> waitingTimes;
    private static float numberOfCars = 0;

    // Start is called before the first frame update
    void Start()
    {
        waitingTimes= new Dictionary<string, float>();
    }

    // Update is called once per frame
    void Update()
    {
        Cal();

        string t = "Average Waiting Time:\n" + Avg_wating.ToString() + " Seconds";
        Text.GetComponent<TMP_Text>().text = t;
        if (numberOfCars == waitingTimes.Count)
        {
            Text.GetComponent<TMP_Text>().color = new Color(0.039f, 0.545f, 0.039f);
        }
    }

    public static void updateAvg(string name, float waitingTime)
    {
        if (waitingTimes.ContainsKey(name))
        {
            waitingTimes[name]= waitingTime;
        }
        else
        {
            waitingTimes.Add(name, waitingTime);
        }
       

    }

    public static void increaseCarNumber()
    {
        numberOfCars++;

    }

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
            sum+= item.Value;
        }
        
        Avg_wating = sum / waitingTimes.Count;
    }
}
