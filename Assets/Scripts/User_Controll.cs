using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class User_Controll : MonoBehaviour
{
    public GameObject MainCamera;
    public string currentPostionOfCamera = "North";
    public GameObject streetText;
    public GameObject simulationSpeed;
    

    private string updatePostion = "";

    // Start is called before the first frame update
    void Start()
    {
        if (currentPostionOfCamera == "North")
        {

            MainCamera.transform.position = new Vector3(0,30,50);
            MainCamera.transform.rotation = Quaternion.Euler(30,180,0);
            streetText.GetComponent<TMP_Text>().text = "University Street\n("+currentPostionOfCamera+")";

        }
        else if (currentPostionOfCamera == "West")
        {

            MainCamera.transform.position = new Vector3(-50, 30, 0);
            MainCamera.transform.rotation = Quaternion.Euler(30, 90, 0);
            streetText.GetComponent<TMP_Text>().text = "Stadium Street\n("+currentPostionOfCamera + ")";

        }
        else if (currentPostionOfCamera == "South")
        {

            MainCamera.transform.position = new Vector3(0, 30, -50);
            MainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
            streetText.GetComponent<TMP_Text>().text = "University Street\n("+currentPostionOfCamera + ")";

        }
        else if (currentPostionOfCamera == "East")
        {

            MainCamera.transform.position = new Vector3(50, 30, 0);
            MainCamera.transform.rotation = Quaternion.Euler(30, -90, 0);
            streetText.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera + ")";

        }

    }

    // Update is called once per frame
    void Update()
    {

        // Camera Updating 
        if (updatePostion != "" && updatePostion != currentPostionOfCamera) {
            string animation = currentPostionOfCamera + "_To_" + updatePostion;
            GetComponent<Animation>().Play(animation);
            currentPostionOfCamera = updatePostion;
            if (currentPostionOfCamera == "North" || currentPostionOfCamera == "South")
            {
                streetText.GetComponent<TMP_Text>().text = "University Street\n(" + currentPostionOfCamera + ")";
            }
            else
            {
                streetText.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera + ")";
            }
            updatePostion = "";
        }


        // Speed of the simulation, if there are still some cars in the simulation
        if (Avg_wating_time.numberOfCars != 0)
        {
            // Simultaion speed
            simulationSpeed.GetComponent<TMP_Text>().text = "x" + Time.timeScale.ToString();
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                speedUpSimultaion();
            }
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                speedDownSimulation();
            }

        }
        else
        {
            simulationSpeed.SetActive(false);
        }
        


    }


    /// <summary>
    /// Method <c>updateCameras</c> activate the corect camera.
    /// </summary>
    public void updateCameras(int cam)
    {
 
        if (cam == 0)
        {
               updatePostion = "North";
        }
        else if (cam == 1)
        {

            updatePostion = "West";

        }
        else if (cam == 2)
        {

            updatePostion = "South";

        }
        else if (cam == 3)
        {

            updatePostion = "East";

        }



    }


    public void speedUpSimultaion()
    {
        Time.timeScale = Mathf.Min(8, Time.timeScale * 2);
    }

    public void speedDownSimulation()
    {
        Time.timeScale = Mathf.Max( 1, Time.timeScale / 2);
    }



}
