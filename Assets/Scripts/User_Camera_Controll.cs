using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class User_Camera_Controll : MonoBehaviour
{
    public GameObject MainCamera;
    public string currentPostionOfCamera = "North";
    public GameObject text;

    private string updatePostion = "";

    // Start is called before the first frame update
    void Start()
    {
        if (currentPostionOfCamera == "North")
        {

            MainCamera.transform.position = new Vector3(0,30,50);
            MainCamera.transform.rotation = Quaternion.Euler(30,180,0);
            text.GetComponent<TMP_Text>().text = "University Street\n("+currentPostionOfCamera+")";

        }
        else if (currentPostionOfCamera == "West")
        {

            MainCamera.transform.position = new Vector3(-50, 30, 0);
            MainCamera.transform.rotation = Quaternion.Euler(30, 90, 0);
            text.GetComponent<TMP_Text>().text = "Stadium Street\n("+currentPostionOfCamera + ")";

        }
        else if (currentPostionOfCamera == "South")
        {

            MainCamera.transform.position = new Vector3(0, 30, -50);
            MainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
            text.GetComponent<TMP_Text>().text = "University Street\n("+currentPostionOfCamera + ")";

        }
        else if (currentPostionOfCamera == "East")
        {

            MainCamera.transform.position = new Vector3(50, 30, 0);
            MainCamera.transform.rotation = Quaternion.Euler(30, -90, 0);
            text.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera + ")";

        }
    }

    // Update is called once per frame
    void Update()
    {

        if (updatePostion != "" && updatePostion != currentPostionOfCamera) {
            string animation = currentPostionOfCamera + "_To_" + updatePostion;
            GetComponent<Animation>().Play(animation);
            currentPostionOfCamera = updatePostion;
            if (currentPostionOfCamera == "North" || currentPostionOfCamera == "South")
            {
                text.GetComponent<TMP_Text>().text = "University Street\n(" + currentPostionOfCamera + ")";
            }
            else
            {
                text.GetComponent<TMP_Text>().text = "Stadium Street\n(" + currentPostionOfCamera + ")";
            }
            updatePostion = "";
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


    /// <summary>
    /// Method <c>turnCameraLeft</c> move the camera to the left.
    /// </summary>
    public void turnCameraLeft()
    {
        string[] names = { "North", "West", "South", "East" };

        for (int i = 0; i < name.Length; i++)
        {
            if (names[i] == currentPostionOfCamera)
            {
                updateCameras((i+1)%4);
                break;
            }
        }

    }

    /// <summary>
    /// Method <c>turnCameraRight</c> turn the camera to right.
    /// </summary>
    public void turnCameraRight()
    {

        string[] names = { "North", "West", "South", "East" };

        for (int i = 0; i < name.Length; i++)
        {
            if (names[i] == currentPostionOfCamera)
            {
                updateCameras((i - 1) % 4);
            }
        }

    }
}
