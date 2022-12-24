using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Progress;



public class DatabaseConnection : MonoBehaviour
{


    public IEnumerator SaveWatingTime(Dictionary<string, data> watingTime)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/DeleteExistingData.php");
        www.SendWebRequest();
 
        foreach (var item in watingTime)
        {
            WWWForm form = new WWWForm();
            form.AddField("name", item.Key);
            form.AddField("waiting_time", item.Value.waiting_time.ToString());
            form.AddField("streat", item.Value.streat);
            form.AddField("turningDirection", item.Value.direction);


            www = UnityWebRequest.Post("http://localhost/sqlconnect/SaveWaitingTime.php", form);
            
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
