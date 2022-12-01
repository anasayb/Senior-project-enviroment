using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

public class CarController : MonoBehaviour
{
    
    public WheelCollider[] wheels;
    public float MAX_SPEED = 200f;

    [Header("Sensors")]
    public float sensorLength = 30f;
    private bool colide = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        // Check if the car colide with something or not
        checkColide();

        if (!colide) {
            
            // If a colide is NOT detected

            // If the car speed reached the maximum, stop incresing the speed
            Rigidbody speed = GetComponent<Rigidbody>();
            if (speed.velocity.magnitude > MAX_SPEED)
            {
                return;
            }

            // Increase the speed
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = 0;
                wheels[i].motorTorque = MAX_SPEED;
            }

        }
        else
        {   
            // A colide IS detected, stop the car
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = 500;
            }
        }

    }


    /*
     * Input: NONE
     * Output: void
     * This function check weather there is a colide infront of the car.
     * If yes the function going to chang the value of "colide" variable.
     * 
     */
    private void checkColide()
    {

        RaycastHit hit;

        // The Ray start postion
        Vector3 pos = transform.position;

        // The direction of the Ray (on the z axis)
        Vector3 dir = transform.forward;

        // If a colide is detected
        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
               
                //Debug Code
                //print("hello its me the freaking " + transform.gameObject.name);

                colide = true;
                return;
           
        }

        //Debug Code
        Debug.DrawRay(transform.position, transform.forward*10, Color.red);

        colide = false;

    }
}
