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
        leaveCarsCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {


    }

    /// <summary>
    /// Method <c>OnTriggerEnter</c> the method called when another object colide with the current object.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {   
        // Increase the number of cars in that direction
        carsCounter++;
        if (other.tag == "Emergency")
        {
            emergencyExist = true;
        }

    }

    /// <summary>
    /// Method <c>OnTriggerExit</c> the method called when another object exist the colider box of the current object.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // Decrease the number of cars in that direction
        carsCounter--;
        leaveCarsCounter++;
        if (other.tag == "Emergency")
        {
            emergencyExist = false;
        }

    }

}
