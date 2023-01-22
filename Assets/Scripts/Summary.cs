using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Summary : MonoBehaviour
{   
    //public static GameObject summary;
    
    private static Response res;
    private static Response data;
    private static DatabaseConnection database ;

    // Start is called before the first frame update
    void Start()
    {
        database = GameObject.Find("Database").GetComponent<DatabaseConnection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void summeryPanel(string tableName)
    {
        //reset
        reset();
   
        // Get table
        res = new Response();
        IEnumerator e = database.getTables(res);
        while (e.MoveNext()) ;
        string[] tables = res.result.Split(' ');
        if (tableName == null)
        {
            tableName = res.result.Split(' ')[0];
        }
        string[] info = tableName.Split('_');

        // Get the data of the table
        res = new Response();
        e = database.getData(res, tableName);
        while (e.MoveNext()) ;
        string[] Data = res.result.Split(" ");

        GameObject summary = GameObject.Find("Canvas").transform.Find("Summary").gameObject;

        // Name of the method
        summary.transform.Find("TLC").Find("Algo Name").GetComponent<TMP_Text>().text = info[1].Replace("#"," ") + " Traffic Light System";

        // Starting Direction
        string[] names = { "North", "West", "South", "East" };
        summary.transform.Find("Direction").Find("Direction").GetComponent<TMP_Text>().text = info[info.Length - 1];

        // Avg_waiting
        float Avg_wating = 0;
        foreach (string s in Data) if (s != "" && s.Split("_")[0] == "AVG#Waiting#time") Avg_wating = float.Parse(s.Split("_")[1]);
        summary.transform.Find("AVG").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating * 100)) / 100f).ToString("F2") + " s";


        // Cars Number
        summary.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = info[0];

        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        row.SetActive(true);
        float mx = 0;
        foreach (var item in Data)
        {
            string[] record = item.Split('_');
            if (record[0] == "AVG#Waiting#time" || record[0] == "")
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
        foreach (var item in tables)
        {
            if (item == "")
            {
                continue;
            }

            GameObject newRow = GameObject.Instantiate(row);
            string name = item.Split('_')[1][0].ToString().ToUpper() + item.Split('_')[1].Substring(1) + " System-" + item.Split('_')[0] + "Cars";
            newRow.transform.GetChild(0).GetComponent<TMP_Text>().text = " " + name;
            newRow.transform.SetParent(rec.transform);
            newRow.name = item;
            UnityEngine.Events.UnityAction action1 = () => { summeryPanel(newRow.name); };
            newRow.transform.GetComponent<Button>().onClick.AddListener(action1);
            if (item == tableName)
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


    private static void reset()
    {

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


    public static void CurrentRunSummery()
    {

        // Name of the method
        GameObject summary = GameObject.Find("Canvas").transform.Find("Summary").gameObject;
        summary.transform.Find("TLC").Find("Algo Name").GetComponent<TMP_Text>().text = Scence_Manger.algorthim;

        // Starting Direction
        string[] names = { "North", "West", "South", "East" };
        summary.transform.Find("Direction").Find("Direction").GetComponent<TMP_Text>().text = names[Scence_Manger.dir];

        // Avg_waiting
        summary.transform.Find("AVG").Find("Time").GetComponent<TMP_Text>().text = (((int)(Avg_wating_time.Avg_wating * 100)) / 100f).ToString("F2") + " s";

        // Cars Number
        summary.transform.Find("Cars Number").Find("number").GetComponent<TMP_Text>().text = Scence_Manger.startingNumberOfCars.ToString();

        // Cars informations
        GameObject cont = summary.transform.Find("CarInfo").Find("Scroll View").Find("Viewport").GetChild(0).gameObject;
        GameObject row = cont.transform.GetChild(0).gameObject;
        row.SetActive(true);
        float mx = 0;
        Dictionary<string, data> Data = Avg_wating_time.waitingTimes;
        foreach (var item in Data)
        {

            if (item.Value.waiting_time > mx)
            {
                mx = item.Value.waiting_time;
            }

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
        row.SetActive(false);
        

        // Max waiting
        summary.transform.Find("Max Waiting Time").Find("Time").GetComponent<TMP_Text>().text = (((int)(mx * 100)) / 100f).ToString("F2") + " s";


    }

}
