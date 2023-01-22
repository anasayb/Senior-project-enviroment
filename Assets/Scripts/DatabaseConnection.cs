using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static UnityEditor.Progress;


public class Response
{
    public string result = "";
}


public class DatabaseConnection : MonoBehaviour
{
       public bool connection;
    // public string[] res;

    public IEnumerator SaveWatingTime(Dictionary<string, data> watingTime, GameObject TrafficLightController)
    {
        string table = "";
        if (TrafficLightController.GetComponent<Traditional_traffic_Controller>().enabled == true)
        {

            string name = Scence_Manger.startingNumberOfCars + "_traditional";
            float[] temp = TrafficLightController.GetComponent<Traditional_traffic_Controller>().time;
            name += "_N" + temp[0].ToString();
            name += "_W" + temp[1].ToString();
            name += "_S" + temp[2].ToString();
            name += "_E" + temp[3].ToString();
            if (Scence_Manger.dir == 0) name += "_North";
            else if (Scence_Manger.dir == 1) name += "_West";
            else if (Scence_Manger.dir == 2) name += "_South";
            else if (Scence_Manger.dir == 3) name += "_East";


            WWWForm form = new WWWForm();
            form.AddField("table", name);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/DeleteExistingData.php", form);
            yield return www.SendWebRequest();

            www.Dispose();
            table = name;


        }
        else if (TrafficLightController.GetComponent<Basic_algo>().enabled == true)
        {
            string name = Scence_Manger.startingNumberOfCars + "_carload#Based";
            if (Scence_Manger.dir == 0) name += "_North";
            else if (Scence_Manger.dir == 1) name += "_West";
            else if (Scence_Manger.dir == 2) name += "_South";
            else if (Scence_Manger.dir == 3) name += "_East";

            WWWForm form = new WWWForm();
            form.AddField("table", name);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/DeleteExistingData.php", form);
            yield return www.SendWebRequest();

            www.Dispose();
            table = name;

        }

        foreach (var item in watingTime)
        {

            StartCoroutine(saveObject(table, item));

        }

        WWWForm form2 = new WWWForm();
        form2.AddField("table", table);
        form2.AddField("name", "AVG#Waiting#time");
        form2.AddField("waiting_time", Avg_wating_time.Avg_wating.ToString());
        form2.AddField("streat", "");
        form2.AddField("turningDirection", "");


        UnityWebRequest www2 = UnityWebRequest.Post("http://localhost/sqlconnect/SaveWaitingTime.php", form2);

        //www.SendWebRequest();
        yield return www2.SendWebRequest();

        www2.Dispose();

    }


    private IEnumerator saveObject(string table, KeyValuePair<string, data> item)
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

    public IEnumerator getTables(Response res)
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

    public IEnumerator getData(Response res, string tableName)
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
            //foreach (string s in res)
            //{
            //    Debug.Log(s);
            //}

            //Debug.Log(res);
        }


        www.Dispose();

    }

    public IEnumerator CheckConnection(Response res)
    {

        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/GetData.php");

        //www.SendWebRequest();
        yield return www.SendWebRequest();

        while (!www.isDone)
            yield return true;

        if (www.result != UnityWebRequest.Result.Success)
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

}
