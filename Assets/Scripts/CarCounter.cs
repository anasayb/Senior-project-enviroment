using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCounter : MonoBehaviour
{


    public int carsCounter = 0;
    public List<float> leavedCars = new List<float>();

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

        if (other.gameObject.layer == 6)
        {
            carsCounter++;
        }



    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == 6)
        {
            carsCounter--;
            leavedCars.Add(other.gameObject.GetComponent<CarController>().waitngTime);
        }

    }

}

