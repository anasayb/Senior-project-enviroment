using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public WheelCollider[] wheels;
    public float MAX_SPEED = 200f;
    
    


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
        
        wheels[0].motorTorque = MAX_SPEED;
        wheels[1].motorTorque = MAX_SPEED;
        wheels[2].motorTorque = MAX_SPEED;
        wheels[3].motorTorque = MAX_SPEED;

    }
}
