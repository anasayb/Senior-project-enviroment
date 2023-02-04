using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryTwo : MonoBehaviour
{
    //public static GameObject summary;

    private static Response res;
    private static DatabaseConnectionTwo database;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// Method <c>summeryPanel</c> Display the summary of the provided table name.
    /// </summary>
    /// <param name="tablename">Table name</param>
    public static void summaryPanel(string tablename)
    {
       
        // Define the summary variable
        GameObject summary = GameObject.Find("Canvas").transform.Find("Summary").gameObject;

        //reset
        reset();

        string[] tableInfo = tablename.Split("_");
        string[] CarsData = DatabaseConnectionTwo.data[tablename].Split(" ");

        // Name of the method
        string nameOfAlgo = tableInfo[1].Replace("#", " ");
        summary.transform.Find("TLC").Find("Algo Name").GetComponent<TMP_Text>().text = nameOfAlgo[0].ToString().ToUpper() + nameOfAlgo.Substring(1) + " Traffic Light System";

        // Starting Direction
        summary.transform.Find("Direction").Find("Direction").GetComponent<TMP_Text>().text = tableInfo[tableInfo.Length - 1][0].ToString().ToUpper() + tableInfo[tableInfo.Length - 1].Substring(1);

        // Avg_waiting
        float Avg_wating = 0;
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "AVG#Waiting#time") Avg_wating = float.Parse(s.Split("_")[1]);
        summary.transform.Find("AVG").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating * 100)) / 100f).ToString("F2") + " s";

        // Flow Rate
        float trafficFlow = 0;
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Flow#rate") trafficFlow = float.Parse(s.Split("_")[1]);
        summary.transform.Find("TrafficFlow").Find("Rate").GetComponent<TMP_Text>().text = trafficFlow.ToString() + " Car/Minute";

        // Avrage Congestion
        float CongestionNorth = 0, CongestionWest = 0,CongestionSouth = 0, CongestionEast = 0;
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#north") CongestionNorth = float.Parse(s.Split("_")[1]);
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#west") CongestionWest = float.Parse(s.Split("_")[1]);
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#south") CongestionSouth = float.Parse(s.Split("_")[1]);
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#east") CongestionEast = float.Parse(s.Split("_")[1]);
        float[] congestionData = {CongestionNorth, CongestionWest, CongestionSouth, CongestionEast};
        Transform congestion = summary.transform.Find("Congestion");
        for (int i = 1; i < congestion.childCount; i++)
        {
            congestion.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = congestionData[i-1].ToString("F2");
        }


        // Cars Number
        summary.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = tableInfo[0] + " Cars";

        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        row.SetActive(true);
        float mx = 0;
        foreach (var item in CarsData)
        {
            string[] record = item.Split('_');
            if (record[0] == "AVG#Waiting#time" || record[0] == "Flow#rate" || record[0].Split("#")[0] == "Congestion" || record[0] == "")
            {
                continue;
            }

            if (float.Parse(record[1]) > mx)
            {
                mx = float.Parse(record[1]);
            }

            GameObject newRow = GameObject.Instantiate(row);
            // Car name
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = record[0].Replace("#", " ");
            // Waiting Time
            newRow.transform.GetChild(1).GetComponent<TMP_Text>().text = (((int)(float.Parse(record[1]) * 100)) / 100f).ToString("F2");
            // Start Direction
            newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = record[3];
            // Turning
            newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = record[2];
            newRow.transform.SetParent(cont.transform);
        }
        row.SetActive(false);

        // Max waiting
        summary.transform.Find("Max Waiting Time").Find("Time").GetComponent<TMP_Text>().text = (((int)(mx * 100)) / 100f).ToString("F2") + " s";


        // show histroy runs
        GameObject rec = summary.transform.Find("Records").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        row = rec.transform.GetChild(0).gameObject;
        row.SetActive(true);
        foreach (var item in DatabaseConnectionTwo.tabelsNames)
        {
            if (item == "" || item == "information")
            {
                continue;
            }

            GameObject newRow = GameObject.Instantiate(row);
            string name = item.Split('_')[1][0].ToString().ToUpper() + item.Split('_')[1].Substring(1).Replace("#", " ") + " system-" + item.Split('_')[0] + "Cars";
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = " " + name;
            newRow.transform.SetParent(rec.transform);
            newRow.name = item;
            UnityEngine.Events.UnityAction action1 = () => { summaryPanel(newRow.name); };
            newRow.transform.GetComponent<Button>().onClick.AddListener(action1);
            if (item == tablename)
            {
                newRow.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(0, 0, 0);
                var color = newRow.transform.GetComponent<Button>().colors;
                color.normalColor = new Color(1, 1, 1, 1);
                newRow.transform.GetComponent<Button>().colors = color;
            }
            else
            {
                var color = newRow.transform.GetComponent<Button>().colors;
                color.normalColor = new Color(0, 0, 0, 0);
                newRow.transform.GetComponent<Button>().colors = color;
            }
        }


        // Destroy the template record
        row.SetActive(false);
    }


    /// <summary>
    /// Method <c>reset</c> Remove old information from the summary.
    /// </summary>
    private static void reset()
    {

        // Define the summary variable
        GameObject summary = GameObject.Find("Canvas").transform.Find("Summary").gameObject;

        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;

        for (int i = 1; i < cont.transform.childCount; i++)
        {
            Destroy(cont.transform.GetChild(i).gameObject);
        }



        // show histroy runs
        GameObject rec = summary.transform.Find("Records").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        for (int i = 1; i < rec.transform.childCount; i++)
        {
            Destroy(rec.transform.GetChild(i).gameObject);
        }


    }


    /// <summary>
    /// Method <c>CurrentRunSummery</c> Display the current run summary.
    /// </summary>
    public static void CurrentRunSummary()
    {

        // Name of the method
        GameObject summary = GameObject.Find("Canvas").transform.Find("Summary").gameObject;
        summary.transform.Find("TLC").Find("Algo Name").GetComponent<TMP_Text>().text = Scence_Manger.algorthim;

        // Starting Direction
        if (Scence_Manger.algorthim == "Traditional Traffic Light System")
        {
            string[] names = { "North", "West", "South", "East" };
            summary.transform.Find("Direction").Find("Direction").GetComponent<TMP_Text>().text = names[Scence_Manger.dir];
        }
        else
        {
            summary.transform.Find("Direction").Find("Direction").GetComponent<TMP_Text>().text = "Dynamic";
        }
        

        // Avg_waiting
        summary.transform.Find("AVG").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating_time_two.Avg_wating * 100)) / 100f).ToString("F2") + " s";

        // Flow Rate
        summary.transform.Find("TrafficFlow").Find("Rate").GetComponent<TMP_Text>().text = Avg_wating_time_two.FlowRate +" Car/Minute";

        // Avrage Congestion
        Transform congestion = summary.transform.Find("Congestion");
        for (int i = 1; i < congestion.childCount; i++)
        {
                congestion.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = Avg_wating_time_two.congestion[0][i -1].ToString("F2");
        }


        // Cars Number
        summary.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = Scence_Manger.startingNumberOfCars.ToString() + " Cars";


        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        row.SetActive(true);
        float mx = 0;
        Dictionary<string, dataTwoIntersection> Data = Avg_wating_time_two.waitingTimes;
        foreach (var item in Data)
        {

            if (item.Value.waiting_time[0] > mx)
            {
                mx = item.Value.waiting_time[0];
            }

            GameObject newRow = GameObject.Instantiate(row);
            // Car name
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = item.Key;
            // Waiting Time
            newRow.transform.GetChild(1).GetComponent<TMP_Text>().text = (((int)(item.Value.waiting_time[0] * 100)) / 100f).ToString("F2");
            // Start Direction
            newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = item.Value.streat;
            // Turning
            newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = item.Value.Turn1;
            newRow.transform.SetParent(cont.transform);
        }
        row.SetActive(false);


        // Max waiting
        summary.transform.Find("Max Waiting Time").Find("Time").GetComponent<TMP_Text>().text = (((int)(mx * 100)) / 100f).ToString("F2") + " s";


        // show histroy runs
        GameObject rec = summary.transform.Find("Records").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        row = rec.transform.GetChild(0).gameObject;
        row.SetActive(true);
        foreach (var item in DatabaseConnectionTwo.tabelsNames)
        {
            if (item == "" || item == "information")
            {
                continue;
            }

            GameObject newRow = GameObject.Instantiate(row);
            string name = item.Split('_')[1][0].ToString().ToUpper() + item.Split('_')[1].Substring(1).Replace("#"," ") + " system-" + item.Split('_')[0] + "Cars";
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = " " + name;
            newRow.transform.SetParent(rec.transform);
            newRow.name = item;
            UnityEngine.Events.UnityAction action1 = () => { summaryPanel(newRow.name); };
            newRow.transform.GetComponent<Button>().onClick.AddListener(action1);
            if (DatabaseConnectionTwo.tabelsNames.IndexOf(item) == DatabaseConnectionTwo.tabelsNames.Count-1)
            {
                newRow.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(0, 0, 0);
                var color = newRow.transform.GetComponent<Button>().colors;
                color.normalColor = new Color(1, 1, 1, 1);
                newRow.transform.GetComponent<Button>().colors = color;
            }
            else
            {
                var color = newRow.transform.GetComponent<Button>().colors;
                color.normalColor = new Color(0, 0, 0, 0);
                newRow.transform.GetComponent<Button>().colors = color;
            }
            
        }


        // Destroy the template record
        row.SetActive(false);


    }

}