using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCounter : MonoBehaviour
{
    public int carsCounter;
    public int leaveCarsCounter;
    public bool emergencyExist = false;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {


    }
    private void OnTriggerEnter(Collider other)
    {
        carsCounter++;
        if (other.tag == "Emergency")
        {
            emergencyExist = true;
        }
        //Debug.Log(other.gameObject.tag);
        //Debug.Log("Car after Enter Count = " + carsCounter + "At this intersection " + tag);

    }
    private void OnTriggerExit(Collider other)
    {
        carsCounter--;
        leaveCarsCounter++;
        if (other.tag == "Emergency")
        {
            emergencyExist = false;
        }
        //Debug.Log(other.gameObject.tag);
        //Debug.Log("Car after Exit Count = " + carsCounter + "At this intersection " + tag);


    }

}
