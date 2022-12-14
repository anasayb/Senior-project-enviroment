using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User_Camera_Controll : MonoBehaviour
{
    public GameObject[] NorthCamera;
    public bool north = false;

    public GameObject[] WestCamera;
    public bool west = false;

    public GameObject[] SouthCamera;
    public bool south = false;

    public GameObject[] EastCamera;
    public bool east = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < NorthCamera.Length; i++)
        {
            NorthCamera[i].gameObject.SetActive(false);
            if (i == 0)
            {
                NorthCamera[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i< WestCamera.Length; i++)
        {  
                WestCamera[i].gameObject.SetActive(false);
        }



        for (int i = 0; i < SouthCamera.Length; i++)
        {
                SouthCamera[i].gameObject.SetActive(false);
        }


        for (int i = 0; i < EastCamera.Length; i++)
        {
                EastCamera[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        int cam = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cam = 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cam = 2;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cam = 3;
        }

        if (north && cam != 0)
        {
            for (int i = 0; i < NorthCamera.Length; i++)
            {
                if (i == cam-1)
                {
                    NorthCamera[i].gameObject.SetActive(true);
                }
                else
                {
                    NorthCamera[i].gameObject.SetActive(false);
                }
            }

        }else if (west && cam != 0)
        {
            for (int i = 0; i < WestCamera.Length; i++)
            {
                if (i == cam - 1)
                {
                    WestCamera[i].gameObject.SetActive(true);
                }
                else
                {
                    WestCamera[i].gameObject.SetActive(false);
                }
            }

        }
        else if(south && cam != 0)
        {
            for (int i = 0; i < SouthCamera.Length; i++)
            {
                if (i == cam - 1)
                {
                    SouthCamera[i].gameObject.SetActive(true);
                }
                else
                {
                    SouthCamera[i].gameObject.SetActive(false);
                }
            }

        }
        else if (east && cam != 0)
        {
            for (int i = 0; i < EastCamera.Length; i++)
            {
                if (i == cam - 1)
                {
                    EastCamera[i].gameObject.SetActive(true);
                }
                else
                {
                    EastCamera[i].gameObject.SetActive(false);
                }
            }

        }

    }


    /// <summary>
    /// Method <c>updateCameras</c> activate the corect camera.
    /// </summary>
    public void updateCameras(int cam)
    {

        if (cam == 0)
        {
            east = false;
            north = true;
            for (int i = 0; i < EastCamera.Length; i++)
            {

                EastCamera[i].gameObject.SetActive(false);
            }

        }
        else if (cam == 1)
        {
            north = false;
            west = true;
            for (int i = 0; i < NorthCamera.Length; i++)
            {
                NorthCamera[i].gameObject.SetActive(false);
            }

        }
        else if (cam == 2)
        {
            west = false;
            south = true;
            for (int i = 0; i < WestCamera.Length; i++)
            {
                WestCamera[i].gameObject.SetActive(false);
            }

        }
        else if (cam == 3)
        {
            south = false;
            east = true;
            for (int i = 0; i < SouthCamera.Length; i++)
            {
                SouthCamera[i].gameObject.SetActive(false);
            }

        }

    }
}
