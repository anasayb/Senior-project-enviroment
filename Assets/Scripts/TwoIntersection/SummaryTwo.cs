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

        //OVerall Avg_waiting
        float Avg_wating = 0;
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Overall#AVG#Waiting#time") Avg_wating = float.Parse(s.Split("_")[1]);
        summary.transform.Find("OverallAvg").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating * 100)) / 100f).ToString("F2") + " s";

        //Avg_waiting
        float Avg_wating0 = 0, Avg_wating1 =0;
        foreach (string s in CarsData)
        {
            if (s != "" && s.Split("_")[0] == "AVG#Waiting#time")
            {
                Avg_wating0 = float.Parse(s.Split("_")[1]);
                Avg_wating1 = float.Parse(s.Split("_")[2]);
            }
        }
        summary.transform.Find("AVG").Find("Time0").GetComponent<TMP_Text>().text = (((int)(Avg_wating0 * 100)) / 100f).ToString("F2") + " s";
        summary.transform.Find("AVG").Find("Time1").GetComponent<TMP_Text>().text = (((int)(Avg_wating1 * 100)) / 100f).ToString("F2") + " s";

        // Flow Rate
        float trafficFlow0 = 0, trafficFlow1 = 0;
        foreach (string s in CarsData)
        {
            if (s != "" && s.Split("_")[0] == "Flow#rate")
            {
                trafficFlow0 = float.Parse(s.Split("_")[1]);
                trafficFlow1 = float.Parse(s.Split("_")[2]);
            }
        }
        summary.transform.Find("TrafficFlow").Find("Rate0").GetComponent<TMP_Text>().text = trafficFlow0.ToString() + " Car/Minute";
        summary.transform.Find("TrafficFlow").Find("Rate1").GetComponent<TMP_Text>().text = trafficFlow1.ToString() + " Car/Minute";

        // Avrage Congestion
        float CongestionNorth0 = 0, CongestionWest0 = 0,CongestionSouth0 = 0, CongestionEast0 = 0;
        float CongestionNorth1 = 0, CongestionWest1 = 0, CongestionSouth1 = 0, CongestionEast1 = 0;
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#north") { CongestionNorth0 = float.Parse(s.Split("_")[1]); CongestionNorth1 = float.Parse(s.Split("_")[2]); }
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#west") { CongestionWest0 = float.Parse(s.Split("_")[1]); CongestionWest1 = float.Parse(s.Split("_")[2]); }
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#south") { CongestionSouth0 = float.Parse(s.Split("_")[1]); CongestionSouth1 = float.Parse(s.Split("_")[2]); }
        foreach (string s in CarsData) if (s != "" && s.Split("_")[0] == "Congestion#east") { CongestionEast0 = float.Parse(s.Split("_")[1]); CongestionEast1 = float.Parse(s.Split("_")[2]); }
        float[,] congestionData = { { CongestionNorth0, CongestionWest0, CongestionSouth0, CongestionEast0 }, { CongestionNorth1, CongestionWest1, CongestionSouth1, CongestionEast1 } };
        
        Transform congestion = summary.transform.Find("Congestion");
        for (int i = 1; i < congestion.childCount; i++)
        {
            for (int j = 0; j < congestion.GetChild(i).childCount; j++)
            {
                congestion.GetChild(i).GetChild(j).GetChild(1).GetComponent<TMP_Text>().text = congestionData[i-1,j].ToString("F2");
            }
        }


        // Cars Number
        summary.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = (int.Parse(tableInfo[0]) * 2).ToString() +" Cars";

        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        row.SetActive(true);
        float mx = 0;
        foreach (var item in CarsData)
        {
            string[] record = item.Split('_');
            if (record[0] == "AVG#Waiting#time" || record[0] == "Flow#rate" || record[0].Split("#")[0] == "Congestion" || record[0] == "" || record[0] == "Overall#AVG#Waiting#time" || record[0] == "Max#Waiting#time")
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
            // Start Direction
            newRow.transform.GetChild(1).GetComponent<TMP_Text>().text = record[5];
            // Waiting Time0
            if (((int)(float.Parse(record[1])) != -1))
            {
                newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = (((int)(float.Parse(record[1]) * 100)) / 100f).ToString("F2");
            }
            else
            {
                newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = "N/A";
            }
            // Waiting Time1
            if (((int)(float.Parse(record[2])) != -1))
            {
                newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = (((int)(float.Parse(record[2]) * 100)) / 100f).ToString("F2");
            }
            else
            {
                newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = "N/A";
            }

            newRow.transform.SetParent(cont.transform);
        }
        row.SetActive(false);

        // Max waiting
        float mx0 = 0, mx1 = 0;
        foreach (string s in CarsData)
        {
            if (s != "" && s.Split("_")[0] == "Max#Waiting#time")
            {
                mx0 = float.Parse(s.Split("_")[1]);
                mx1 = float.Parse(s.Split("_")[2]);
            }
        }
        summary.transform.Find("Max Waiting Time").Find("Time0").GetComponent<TMP_Text>().text = (((int)(mx0 * 100)) / 100f).ToString("F2") + " s";
        summary.transform.Find("Max Waiting Time").Find("Time1").GetComponent<TMP_Text>().text = (((int)(mx1 * 100)) / 100f).ToString("F2") + " s";

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

        // Overall Avg_waiting
        summary.transform.Find("OverallAvg").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating_time_two.Avg_wating * 100)) / 100f).ToString("F2") + " s";

        // Avg_waiting
        summary.transform.Find("AVG").Find("Time0").GetComponent<TMP_Text>().text = (((int)(Avg_wating_time_two.Intersection_Avg_wating[0] * 100)) / 100f).ToString("F2") + " s";
        summary.transform.Find("AVG").Find("Time1").GetComponent<TMP_Text>().text = (((int)(Avg_wating_time_two.Intersection_Avg_wating[1] * 100)) / 100f).ToString("F2") + " s";

        // Flow Rate
        summary.transform.Find("TrafficFlow").Find("Rate0").GetComponent<TMP_Text>().text = Avg_wating_time_two.FlowRate[0] +" Car/Minute";
        summary.transform.Find("TrafficFlow").Find("Rate1").GetComponent<TMP_Text>().text = Avg_wating_time_two.FlowRate[1] + " Car/Minute";

        // Avrage Congestion
        Transform congestion = summary.transform.Find("Congestion");
        for (int i = 1; i < congestion.childCount; i++)
        {
            for (int j = 0; j < congestion.GetChild(i).childCount; j++) {
                congestion.GetChild(i).GetChild(j).GetChild(1).GetComponent<TMP_Text>().text = Avg_wating_time_two.congestion[i-1][j].ToString("F2");
            }
        }


        // Cars Number
        summary.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = (Scence_Manger.startingNumberOfCars*2).ToString() + " Cars";


        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        row.SetActive(true);
        float mx0 = 0, mx1 = 0;
        Dictionary<string, dataTwoIntersection> Data = Avg_wating_time_two.waitingTimes;
        foreach (var item in Data)
        {

            if (item.Value.waiting_time[0] > mx0)
            {
                mx0 = item.Value.waiting_time[0];
            }
            if (item.Value.waiting_time[1] > mx1)
            {
                mx1 = item.Value.waiting_time[1];
            }

            GameObject newRow = GameObject.Instantiate(row);
            // Car name
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = item.Key;
            // Start Direction
            newRow.transform.GetChild(1).GetComponent<TMP_Text>().text = item.Value.streat;
            // Waiting Time0
            if (((int)item.Value.waiting_time[0]) != -1) {
                newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = (((int)(item.Value.waiting_time[0] * 100)) / 100f).ToString("F2");
            }
            else
            {
                newRow.transform.GetChild(2).GetComponent<TMP_Text>().text = "N/A";
            }

            // Waiting Time1
            if (((int)item.Value.waiting_time[1]) != -1)
            {
                newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = (((int)(item.Value.waiting_time[1] * 100)) / 100f).ToString("F2");
            }
            else
            {
                newRow.transform.GetChild(3).GetComponent<TMP_Text>().text = "N/A";
            }
            
            newRow.transform.SetParent(cont.transform);
        }
        row.SetActive(false);


        // Max waiting
        summary.transform.Find("Max Waiting Time").Find("Time0").GetComponent<TMP_Text>().text = (((int)(mx0 * 100)) / 100f).ToString("F2") + " s";
        summary.transform.Find("Max Waiting Time").Find("Time1").GetComponent<TMP_Text>().text = (((int)(mx1 * 100)) / 100f).ToString("F2") + " s";

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