using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Progress;


public class DatabaseConnectionTwo : MonoBehaviour
{


    public static List<string> tabelsNames;
    public static Dictionary<string, string> data;
    public static bool connection = false;
    public GameObject[] TrafficLightController;

    public void Start()
    {

        // inizilize the variable
        if (DatabaseConnectionTwo.tabelsNames == null)
        {
            DatabaseConnectionTwo.data = new Dictionary<string, string>();
            DatabaseConnectionTwo.tabelsNames = new List<string>();


            // check database connection
            Response res = new Response();
            IEnumerator e = CheckConnection(res);
            while (e.MoveNext()) ;

            if (res.result == "Yes")
            {

                // Database is up
                DatabaseConnectionTwo.connection = true;

                // Get tables name
                res = new Response();
                e = getTables(res);
                while (e.MoveNext()) ;

                if (res.result != "0")
                {
                    // Save table names
                    DatabaseConnectionTwo.tabelsNames = res.result.Split(" ").ToList();


                    //Debug
                    // string file1 = Application.streamingAssetsPath + "/TableNames.txt";
                    // File.WriteAllText(file1, res2.result);

                    // Get the data in each table
                    foreach (string table in tabelsNames)
                    {
                        if (table == "" || table == " " )
                        {
                            continue;
                        }

                        res = new Response();
                        e = getData(res, table);
                        while (e.MoveNext()) ;

                        // save data

                        DatabaseConnectionTwo.data.Add(table, res.result);
                    }


                    // Debug  Code
                    // string dataToWrite = "";
                    // dataToWrite += res3.result + "\n";
                    // string file2 = Application.streamingAssetsPath + "/Data.txt";
                    //  File.WriteAllText(file2, dataToWrite);
                    //List<string> list = File.ReadAllLines(file1).ToList();
                }
            }
        }




    }





    /// <summary>
    /// Method <c>CheckConnection</c> Check if the database is up and running.
    /// </summary>
    /// <param name="res">A container for the result</param>
    public IEnumerator CheckConnection(Response res)
    {

        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/TwoIntersection/GetData.php"))
        {

            //www.SendWebRequest();
            yield return www.SendWebRequest();

            while (!www.isDone)
                yield return true;

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Debug.Log(www.error);
                res.result = "Yes";
            }
            else
            {
                res.result = "No";
            }

        }

    }


    /// <summary>
    /// Method <c>getTables</c> Get the names of all tables in the data base.
    /// </summary>
    /// <param name="res">A container for the result</param>
    public IEnumerator getTables(Response res)
    {
        res.result = "";
        if (DatabaseConnectionTwo.connection)
        {

            using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/TwoIntersection/GetData.php"))
            {

                //www.SendWebRequest();
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return true;

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    res.result = www.downloadHandler.text;

                }

            }
        }

    }


    /// <summary>
    /// Method <c>getData</c> Get the data in the provided table.
    /// </summary>
    /// <param name="res">A container for the result</param>
    /// /// <param name="tableName">The name of the table</param>
    public IEnumerator getData(Response res, string tableName)
    {
        res.result = "";
        if (DatabaseConnectionTwo.connection)
        {

            WWWForm form = new WWWForm();
            form.AddField("name", tableName);

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/TwoIntersection/GetData.php", form))
            {

                //www.SendWebRequest();
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return true;

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    res.result = www.downloadHandler.text;
                }

            }

        }
    }


    /// <summary>
    /// Method <c>SaveWatingTime</c> Saves the data in the database.
    /// </summary>
    /// <param name="watingTime">All the cars in the simulation with thier corresponidng info</param>
    /// /// <param name="TrafficLightController">Object of the traffic Light controller</param>
    public IEnumerator SaveWatingTime(Dictionary<string, dataTwoIntersection> watingTime)
    {
        string table = "two_";
        if (TrafficLightController[0].GetComponent<Traditional_traffic_Controller>().enabled == true)
        {

            string name = Scence_Manger.startingNumberOfCars + "_traditional";

            // Intersection 0
            float[] temp = TrafficLightController[0].GetComponent<Traditional_traffic_Controller>().time;
            name += "_n" + temp[0].ToString();
            name += "_w" + temp[1].ToString();
            name += "_s" + temp[2].ToString();
            name += "_e" + temp[3].ToString();
            if (Scence_Manger.dir == 0) name += "_north";
            else if (Scence_Manger.dir == 1) name += "_west";
            else if (Scence_Manger.dir == 2) name += "_south";
            else if (Scence_Manger.dir == 3) name += "_east";

            //Intersection 1
            temp = TrafficLightController[1].GetComponent<Traditional_traffic_Controller>().time;
            name += "_n" + temp[0].ToString();
            name += "_w" + temp[1].ToString();
            name += "_s" + temp[2].ToString();
            name += "_e" + temp[3].ToString();
            if (Scence_Manger.dir == 0) name += "_north";
            else if (Scence_Manger.dir == 1) name += "_west";
            else if (Scence_Manger.dir == 2) name += "_south";
            else if (Scence_Manger.dir == 3) name += "_east";

            table = name;


        }
        else if (TrafficLightController[0].GetComponent<Basic_algo>().enabled == true)
        {
            string name = Scence_Manger.startingNumberOfCars + "_carload#based_dynamic";
            table = name;

        }
        else if (TrafficLightController[0].GetComponent<AI_two_single>().enabled == true)
        {
            string name = Scence_Manger.startingNumberOfCars + "_ai#based_dynamic";
            table = name;

        }


        // check if a table with the same name is already exist
        int index = DatabaseConnectionTwo.tabelsNames.IndexOf(table);
        if (index != -1)
        {
            DatabaseConnectionTwo.tabelsNames.RemoveAt(index);

        }
        DatabaseConnectionTwo.tabelsNames.Add(table);


        if (DatabaseConnectionTwo.connection)
        {
            WWWForm form = new WWWForm();
            form.AddField("table", table);
            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/TwoIntersection/DeleteExistingData.php", form))
            {
                yield return www.SendWebRequest();

            }            
        }


        string carsData = "";
        float sum0 = 0, sum1 = 0, max0 = 0, max1= 0 ;
        int count0 = 0, count1 = 0;
        foreach (var item in watingTime)
        {
            carsData += item.Key.Replace(" ", "#") + "_" + item.Value.waiting_time[0].ToString() + "_" + item.Value.waiting_time[1].ToString() + "_" + item.Value.Turn1 + "_" +item.Value.Turn2 + "_" + item.Value.streat + " ";

            if (item.Value.waiting_time[0] != -1)
            {
                sum0 += item.Value.waiting_time[0];
                count0++;
                if (item.Value.waiting_time[0] > max0)
                {
                    max0 = item.Value.waiting_time[0];
                }
            }

            if (item.Value.waiting_time[1] != -1)
            {
                sum1 += item.Value.waiting_time[1];
                count1++;
                if (item.Value.waiting_time[1] > max1)
                {
                    max1 = item.Value.waiting_time[1];
                }
            }


        }

        // Max waiting time
        carsData += "Max#Waiting#time_" + max0.ToString() + "_" + max1.ToString() + " ";

        // Average waiting time
        carsData += "AVG#Waiting#time_" + (1.0f *sum0/count0).ToString() + "_"+ (1.0f * sum1 / count1).ToString() + " ";
        carsData += "Overall#AVG#Waiting#time_" + Avg_wating_time_two.Avg_wating.ToString() + " ";


        // Traffic Flow rate
        carsData += "Flow#rate_" + Avg_wating_time_two.FlowRate[0].ToString() + "_" + Avg_wating_time_two.FlowRate[1].ToString() + " ";


        string[] streets = { "north", "west", "south", "east"};

        // Avrage congestion for each street Congestion
        for (int i = 0; i < streets.Length; i++)
        {
            
            carsData += "Congestion#"+streets[i]+"_" + Avg_wating_time_two.congestion[0][i].ToString() + "_"+ Avg_wating_time_two.congestion[1][i].ToString() + " ";

        }


        StartCoroutine(saveToDatabase(table, carsData));


        if (DatabaseConnectionTwo.data.ContainsKey(table))
        {
            DatabaseConnectionTwo.data[table] = carsData;

        }
        else
        {
            DatabaseConnectionTwo.data.Add(table, carsData);
        }


    }


    /// <summary>
    /// Method <c>saveToDatabase</c> Saves the data in the database.
    /// </summary>
    /// <param name="table">Table name</param>
    /// /// <param name="item">A pair of the car name with the crossponding car's data</param>
    private IEnumerator saveToDatabase(string table, string data)
    {

        if (DatabaseConnectionTwo.connection)
        {
            WWWForm form = new WWWForm();
            form.AddField("table", table);
            form.AddField("data", data);


            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/TwoIntersection/SaveWaitingTime.php", form))
            {

                //www.SendWebRequest();
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Form upload complete!");
                }

            }


            using (UnityWebRequest newwww = UnityWebRequest.Get("http://localhost/sqlconnect/TwoIntersection/SaveTablesInfo.php"))
            {
                yield return newwww.SendWebRequest();
            }

        }

    }



}