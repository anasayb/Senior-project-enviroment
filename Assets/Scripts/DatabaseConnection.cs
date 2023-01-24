using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class Response
{
    public string result = "";
}


public class DatabaseConnection : MonoBehaviour
{


    public static List<string> tabelsNames;
    public static Dictionary<string, string> data;
    public static bool connection = false;

    public void Start()
    {

        // inizilize the variable
        if (DatabaseConnection.tabelsNames == null)
        {
            DatabaseConnection.data = new Dictionary<string, string>();
            DatabaseConnection.tabelsNames = new List<string>();
        }



        // check database connection
        Response res = new Response();
        IEnumerator e = CheckConnection(res);
        while (e.MoveNext()) ;

        if (res.result == "Yes")
        {

            // Database is up

            // Get tables name
            Response res2 = new Response();
            IEnumerator e2 = getTables(res2);
            while (e2.MoveNext()) ;

            // Save table names
            DatabaseConnection.tabelsNames = res2.result.Split(" ").ToList();

            //Debug
            // string file1 = Application.streamingAssetsPath + "/TableNames.txt";
            // File.WriteAllText(file1, res2.result);

            // Get the data in each table
            foreach (string table in tabelsNames)
            {
                if (table == "" || table == " ")
                {
                    continue;
                }

                Response res3 = new Response();
                IEnumerator e3 = getData(res3, table);
                while (e3.MoveNext()) ;

                // save data

                DatabaseConnection.data.Add(table, res3.result);
            }

            DatabaseConnection.connection = true;

            // Debug  Code
            // string dataToWrite = "";
            // dataToWrite += res3.result + "\n";
            // string file2 = Application.streamingAssetsPath + "/Data.txt";
            //  File.WriteAllText(file2, dataToWrite);
            //List<string> list = File.ReadAllLines(file1).ToList();

        }

    }





    /// <summary>
    /// Method <c>CheckConnection</c> Check if the database is up and running.
    /// </summary>
    /// <param name="res">A container for the result</param>
    public IEnumerator CheckConnection(Response res)
    {

        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/GetData.php");

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


        www.Dispose();

    }


    /// <summary>
    /// Method <c>getTables</c> Get the names of all tables in the data base.
    /// </summary>
    /// <param name="res">A container for the result</param>
    public IEnumerator getTables(Response res)
    {
        res.result = "";
        if (DatabaseConnection.connection)
        {

            UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/GetData.php");

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


            www.Dispose();
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
        if (DatabaseConnection.connection)
        {
            WWWForm form = new WWWForm();
            form.AddField("name", tableName);

            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/GetData.php", form);


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


            www.Dispose();
        }
    }


    /// <summary>
    /// Method <c>SaveWatingTime</c> Saves the data in the database.
    /// </summary>
    /// <param name="watingTime">All the cars in the simulation with thier corresponidng info</param>
    /// /// <param name="TrafficLightController">Object of the traffic Light controller</param>
    public IEnumerator SaveWatingTime(Dictionary<string, data> watingTime, GameObject TrafficLightController)
    {
        string table = "";
        if (TrafficLightController.GetComponent<Traditional_traffic_Controller>().enabled == true)
        {

            string name = Scence_Manger.startingNumberOfCars + "_traditional";
            float[] temp = TrafficLightController.GetComponent<Traditional_traffic_Controller>().time;
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
        else if (TrafficLightController.GetComponent<Basic_algo>().enabled == true)
        {
            string name = Scence_Manger.startingNumberOfCars + "_carload#based";
            if (Scence_Manger.dir == 0) name += "_north";
            else if (Scence_Manger.dir == 1) name += "_west";
            else if (Scence_Manger.dir == 2) name += "_south";
            else if (Scence_Manger.dir == 3) name += "_east";

            table = name;

        }


        // check if a table with the same name is already exist
        if (DatabaseConnection.tabelsNames.Contains(table))
        {
            DatabaseConnection.tabelsNames.RemoveAt(DatabaseConnection.tabelsNames.IndexOf(table));

        }
        DatabaseConnection.tabelsNames.Add(table);


        if (DatabaseConnection.connection)
        {
            WWWForm form = new WWWForm();
            form.AddField("table", table);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/DeleteExistingData.php", form);
            yield return www.SendWebRequest();

            www.Dispose();
        }


        string carsData = "";
        foreach (var item in watingTime)
        {
            carsData += item.Key.Replace(" ", "#") + "_" + item.Value.waiting_time.ToString() + "_" + item.Value.direction + "_" + item.Value.streat + " ";
            StartCoroutine(saveToDatabase(table, item));

        }




        WWWForm form2 = new WWWForm();
        form2.AddField("table", table);
        form2.AddField("name", "AVG#Waiting#time");
        form2.AddField("waiting_time", Avg_wating_time.Avg_wating.ToString());
        carsData += "AVG#Waiting#time_" + Avg_wating_time.Avg_wating.ToString() + " ";
        form2.AddField("streat", "");
        form2.AddField("turningDirection", "");


        if (DatabaseConnection.data.ContainsKey(table))
        {
            DatabaseConnection.data[table] = carsData;

        }
        else
        {
            DatabaseConnection.data.Add(table, carsData);
        }

        
        if (DatabaseConnection.connection)
        {
            UnityWebRequest www2 = UnityWebRequest.Post("http://localhost/sqlconnect/SaveWaitingTime.php", form2);

            //www.SendWebRequest();
            yield return www2.SendWebRequest();

            www2.Dispose();
        }

    }


    /// <summary>
    /// Method <c>saveToDatabase</c> Saves the data in the database.
    /// </summary>
    /// <param name="table">Table name</param>
    /// /// <param name="item">A pair of the car name with the crossponding car's data</param>
    private IEnumerator saveToDatabase(string table, KeyValuePair<string, data> item)
    {

        if (DatabaseConnection.connection)
        {
            WWWForm form = new WWWForm();
            form.AddField("table", table);
            form.AddField("name", item.Key.Replace(" ", "#"));
            form.AddField("waiting_time", item.Value.waiting_time.ToString());
            form.AddField("streat", item.Value.streat);
            form.AddField("turningDirection", item.Value.direction);


            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/SaveWaitingTime.php", form);

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


            www.Dispose();
        }

    }



}