using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{

    public static Transform[,] LeftPaths;
    public static Transform[,] RightPaths;

    // Start is called before the first frame update
    void Start()
    {
        // Inizilize 
        LeftPaths = new Transform[2, 4];
        RightPaths = new Transform[2, 4];

        // Intersection 0
        GameObject paths = GameObject.Find("Intersection0").transform.Find("Turning Paths").gameObject;
        // Left
        for (int i = 0; i < paths.transform.GetChild(0).childCount; i++)
        {
            LeftPaths[0, i] = paths.transform.GetChild(0).GetChild(i);
        }

        // Right
        for (int i = 0; i < paths.transform.GetChild(1).childCount; i++)
        {
            RightPaths[0, i] = paths.transform.GetChild(1).GetChild(i);
        }


        // Intersection 1
        paths =  GameObject.Find("Intersection1").transform.Find("Turning Paths").gameObject; ;
        // Left
        for (int i = 0; i < paths.transform.GetChild(0).childCount; i++)
        {
            LeftPaths[1, i] = paths.transform.GetChild(0).GetChild(i);
        }

        // Right
        for (int i = 0; i < paths.transform.GetChild(1).childCount; i++)
        {
            RightPaths[1, i] = paths.transform.GetChild(1).GetChild(i);
        }
    }


}