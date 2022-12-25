using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCounter : MonoBehaviour
{
    public int counterTemp;
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
        counterTemp++;
        Debug.Log(other.gameObject.tag);
        Debug.Log(counterTemp);

    }
    private void OnTriggerExit(Collider other)
    {
        counterTemp--;
        Debug.Log(other.gameObject.tag);
        Debug.Log("Car Count = " + counterTemp);


    }

}
