using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avg_wating_time : MonoBehaviour
{

    public float Avg_wating = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");

        float temp = 0;
        for (int i = 0; i < cars.Length; i++)
        {
            temp += cars[i].GetComponent<CarController>().waitngTime;
        }

        Avg_wating = temp/cars.Length;


    }
}
