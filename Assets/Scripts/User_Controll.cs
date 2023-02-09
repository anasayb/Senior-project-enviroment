using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class User_Controll : MonoBehaviour
{   
    public static int Intersection;

    public GameObject[] MainCamera;
    public string[] currentPostionOfCamera = { "North", "North" };
    public GameObject streetText;
    public GameObject IntersectionText;
    public GameObject RightButton;
    public GameObject LeftButton;
    public GameObject simulationSpeed;
    

    private string[] updatePostion = { "", "" };
    private bool isKeyPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        User_Controll.Intersection = 0;

        if (Scence_Manger.inv == 1) {
            IntersectionText.GetComponent<TMP_Text>().text = "Intersection 1";
        }

        if (currentPostionOfCamera[User_Controll.Intersection] == "North")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(0,30,50);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30,180,0);
            streetText.GetComponent<TMP_Text>().text = "University Street\n("+currentPostionOfCamera[User_Controll.Intersection] + ")";

        }
        else if (currentPostionOfCamera[User_Controll.Intersection] == "West")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(-50, 30, 0);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, 90, 0);
            streetText.GetComponent<TMP_Text>().text = "Stadium Street\n("+currentPostionOfCamera[User_Controll.Intersection] + ")";

        }
        else if (currentPostionOfCamera[User_Controll.Intersection] == "South")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(0, 30, -50);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, 0, 0);
            streetText.GetComponent<TMP_Text>().text = "University Street\n("+currentPostionOfCamera[User_Controll.Intersection] + ")";

        }
        else if (currentPostionOfCamera[User_Controll.Intersection] == "East")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(50, 30, 0);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, -90, 0);
            streetText.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";

        }

        // Reset the simultaion speed
        Time.timeScale = 1;
        //simulationSpeed.SetActive(true);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveCamera(0);
        MoveCamera(1);

        // Speed of the simulation, if there are still some cars in the simulation
        if (Avg_wating_time_two.numberOfCars != 0 || Avg_wating_time.numberOfCars != 0)
        {
            // Simultaion speed
            simulationSpeed.SetActive(true);
            simulationSpeed.GetComponent<TMP_Text>().text = "x" + Time.timeScale.ToString();
            if (Input.GetKeyDown(KeyCode.Equals) && !isKeyPressed)
            {
                speedUpSimultaion();
            }
            else if (Input.GetKeyDown(KeyCode.Minus) && !isKeyPressed)
            {
                speedDownSimulation();
            }

        }
        else
        {
            simulationSpeed.SetActive(false);
        }
    }

    public void MoveCamera(int intersection)
    {
        // Camera Updating 
        if (updatePostion[intersection] != "" && updatePostion[intersection] != currentPostionOfCamera[intersection])
        {
            if (User_Controll.Intersection == intersection) {
                string animation = currentPostionOfCamera[intersection] + "_To_" + updatePostion[intersection];
                MainCamera[intersection].GetComponent<Animation>().Play(animation);
                
            }
            currentPostionOfCamera[intersection] = updatePostion[intersection];
            if (User_Controll.Intersection == intersection)
            {
                if (currentPostionOfCamera[intersection] == "North" || currentPostionOfCamera[intersection] == "South")
                {
                    if (User_Controll.Intersection == 0)
                    {
                        streetText.GetComponent<TMP_Text>().text = "University Street\n(" + currentPostionOfCamera[intersection] + ")";
                    }
                    else
                    {
                        streetText.GetComponent<TMP_Text>().text = "Park Street\n(" + currentPostionOfCamera[intersection] + ")";
                    }
                }
                else
                {
                    streetText.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera[intersection] + ")";
                }
            }

            updatePostion[intersection] = "";
        }
    }


    /// <summary>
    /// Method <c>updateCameras</c> activate the corect camera.
    /// </summary>
    public void updateCameras(int cam, int intersection)
    {
 
        if (cam == 0)
        {
               updatePostion[intersection] = "North";
        }
        else if (cam == 1)
        {

            updatePostion[intersection] = "West";

        }
        else if (cam == 2)
        {

            updatePostion[intersection] = "South";

        }
        else if (cam == 3)
        {

            updatePostion[intersection] = "East";

        }



    }

    //This function speeds up the simulation up to 8x 
    public void speedUpSimultaion()
    {
        int temp = (int)(Time.timeScale * 2);
        Time.timeScale = Mathf.Min(8, temp);
        isKeyPressed = false;
    }

    //This function slow down the simulation down to 1x 
    public void speedDownSimulation()
    {
        int temp = (int)(Time.timeScale / 2);
        Time.timeScale = Mathf.Max( 1, temp);
        isKeyPressed = false;
    }

    //?
    public void UpdatedIntersection()
    {
        // Disable current Intersection camera
        MainCamera[User_Controll.Intersection].SetActive(false);

        // Go to next intersection
        User_Controll.Intersection = (User_Controll.Intersection + 1) % 2;


        // Enable new Intersection Camera
        MainCamera[User_Controll.Intersection].SetActive(true);


        // change the postion
        if (currentPostionOfCamera[User_Controll.Intersection] == "North")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(0, 30, 50);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, 180, 0);
            if (User_Controll.Intersection == 0) {
                streetText.GetComponent<TMP_Text>().text = "University Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";
            }
            else
            {
                streetText.GetComponent<TMP_Text>().text = "Park Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";
            }

        }
        else if (currentPostionOfCamera[User_Controll.Intersection] == "West")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(-50, 30, 0);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, 90, 0);
            streetText.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";

        }
        else if (currentPostionOfCamera[User_Controll.Intersection] == "South")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(0, 30, -50);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, 0, 0);
            if (User_Controll.Intersection == 0)
            {
                streetText.GetComponent<TMP_Text>().text = "University Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";
            }
            else
            {
                streetText.GetComponent<TMP_Text>().text = "Park Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";
            }

        }
        else if (currentPostionOfCamera[User_Controll.Intersection] == "East")
        {

            MainCamera[User_Controll.Intersection].transform.localPosition = new Vector3(50, 30, 0);
            MainCamera[User_Controll.Intersection].transform.rotation = Quaternion.Euler(30, -90, 0);
            streetText.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera[User_Controll.Intersection] + ")";

        }

        // Update Name of the intersection
        IntersectionText.GetComponent<TMP_Text>().text = "Intersection " + (User_Controll.Intersection + 1);

        if (User_Controll.Intersection == 0)
        {
            RightButton.SetActive(true);
            LeftButton.SetActive(false);
        }
        else
        {
            RightButton.SetActive(false);
            LeftButton.SetActive(true);
        }

    }

}
