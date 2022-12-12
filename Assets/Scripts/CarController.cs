using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

public class CarController : MonoBehaviour
{
    
    public WheelCollider[] wheels;
    public float MAX_SPEED = 100f;

    [Header("Turning")]
    private float MAX_TURNING_ANGLE = 20f;
    public bool left = false, right = false;
    public Transform pathGourp;
    public float distanceFromPath = 2f;

    private List<Transform> Path = new List<Transform>();
    private float FORCE_STOP = 600f;
    private bool turning = false;
    private int currentIndex = 0;

    [Header("Sensors")]
    public float sensorLength = 10f;

    private float tempSensorLength;
    private float colide = -1;

    [Header("Wating Time")]
    public LayerMask CarLay;
    public float waitngTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        getPath();
        tempSensorLength = sensorLength;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        // Check if the car colide with something or not
        checkColide();

        // Calculate the wating time of the car
        calculateWatingTime();

        // movement of the car
        move();

    }


  
    /// <summary>
    /// Method <c>checkColide</c> This function check weather there is a colide infront of the car.
    /// </summary>
    private void checkColide()
    {

        RaycastHit hit;

        // The Ray start postion
        Vector3 posForwardCenter = transform.position;

        Vector3 posForwardRight = transform.position;
        posForwardRight += (gameObject.GetComponent<BoxCollider>().size.x / 2 * transform.right);

        Vector3 posForwardleft = transform.position;
        posForwardleft += (gameObject.GetComponent<BoxCollider>().size.x / 2 * (-1 * transform.right));

        Vector3 posRight = transform.position;
        posRight += (gameObject.GetComponent<BoxCollider>().size.z/2 * transform.forward);
        posRight += (gameObject.GetComponent<BoxCollider>().size.x/2 * transform.right);


        Vector3 posLeft = transform.position;
        posLeft += (gameObject.GetComponent<BoxCollider>().size.z / 2 * transform.forward);
        posLeft += (gameObject.GetComponent<BoxCollider>().size.x / 2 * (-1 * transform.right));


        // The direction of the Ray (on the z axis)
        Vector3 dir = transform.forward;
        Vector3 rightDir = (transform.forward + transform.right);
        Vector3 leftDir =  (transform.forward + -1*transform.right);

        //speed
        Rigidbody speed = GetComponent<Rigidbody>();

        // If a colide is detected
        if (Physics.Raycast(posForwardCenter, dir, out hit, (sensorLength + speed.velocity.magnitude) * 1.25f) 
            || Physics.Raycast(posForwardRight,  dir, out hit, (sensorLength + speed.velocity.magnitude) * 1.25f) 
            || Physics.Raycast(posForwardleft,  dir, out hit, (sensorLength + speed.velocity.magnitude) * 1.25f)  
            // || Physics.Raycast(posRight, rightDir, out hit, (6.5f / 2.0f)) 
            // || Physics.Raycast(posLeft, leftDir, out hit, (6.5f / 2.0f))
            )
        {   
            // If its not the same object 
            if (hit.collider.name != transform.name)
            {
                //Debug Code
                //print("hello its me the freaking " + transform.gameObject.name);

                Debug.DrawRay(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                Debug.DrawRay(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                Debug.DrawRay(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                // Debug.DrawRay(posRight, rightDir * (6.5f / 2.0f), Color.red);
                // Debug.DrawRay(posLeft, leftDir * (6.5f / 2.0f), Color.red);

                colide = hit.distance;
               
            }
            

        }
        else
        {
            //Debug Code
            Debug.DrawRay(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            Debug.DrawRay(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            Debug.DrawRay(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            // Debug.DrawRay(posRight, rightDir * (6.5f / 2.0f), Color.green);
            // Debug.DrawRay(posLeft, leftDir * (6.5f / 2.0f), Color.green);

            // Reset colide
            colide = -1;
        }

        

        // Turning Code
        if (Physics.Raycast(posForwardCenter, -(transform.up), out hit, sensorLength))
        {
            if (hit.collider.tag == "IntersectionArea")
            {
                turning = true;
            }
            else
            {
                turning = false;
            }
           
        }
        else
        {
            Destroy(gameObject);
        }

        // Debug Code
        // Debug.DrawRay(posForwardCenter, -(transform.up*1000), Color.blue);
    }

    /// <summary>
    /// Method <c>calculateWatingTime</c> This function calculate the waiting time of a car form a full stop at red light till it passes the traffic light.
    /// </summary>
    private void calculateWatingTime()
    {

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, 200, ~CarLay))
        {
            Rigidbody speed = GetComponent<Rigidbody>();
            if (speed.velocity.magnitude >= 0)
            {
                waitngTime += Time.deltaTime;
            }

        }

        // Debug code
        // Debug.DrawRay(ray.origin, transform.forward*200, Color.blue);

    }

    /// <summary>
    /// Method <c>move</c> This function Controll the movment of a car form speeding up to stopping.
    /// </summary>
    private void move()
    {
        Rigidbody speed = GetComponent<Rigidbody>();

        // If you are in the area and the left flag is ON
        if (turning && left)
        {
            sensorLength = 2;
            turnLeft();
        }
        else
        {
            sensorLength = tempSensorLength;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            gameObject.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
            var rotationVector = transform.rotation.eulerAngles;

            rotationVector.x = 0;
            rotationVector.z = 0;
            rotationVector.y = roundToFullAngle(rotationVector.y);
 
            transform.rotation = Quaternion.Euler(rotationVector);
            resetAllWheelsAngles();
        }


        if (colide == -1)
        {

            // If a colide is NOT detected

            // If the car speed reached the maximum, stop incresing the speed
            
            if (speed.velocity.magnitude > MAX_SPEED )
            {
                return;
            }
            else
            {
                // Increase the speed
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].brakeTorque = 0;
                    wheels[i].motorTorque = MAX_SPEED;
                }

            }


        }
        else
        {
            // A colide IS detected, stop the car

            // Calculate the force need tos top the car
            float force = 0;

            if (colide <= sensorLength / 2)
            {
                force = FORCE_STOP;
            }
            else
            {
              
                float engery = (1 / 2.0f) * speed.mass * (speed.velocity.magnitude) * (speed.velocity.magnitude);
                force = engery / colide;
            }


            // Apply break torque on all wheels
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = force;
            }
        }
    }

    void getPath()
    {
        int count = pathGourp.childCount;
        for (int i = 0; i < count; i++)
        {
            Path.Add(pathGourp.GetChild(i).transform);
        }
       
    }

    void turnLeft()
    {
   
        Vector3 currentPos = transform.position;
        Vector3 nextPoint = transform.InverseTransformPoint(Path[currentIndex].position.x, transform.position.y, Path[currentIndex].position.z);
        //print(currentIndex);
        //print(nextPoint.magnitude);
        float stear = MAX_TURNING_ANGLE *(nextPoint.x / nextPoint.magnitude);
        

        gameObject.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezeAll;
        wheels[0].steerAngle = stear;
        wheels[1].steerAngle = stear;
        if (transform.tag == "Truck")
        {
            wheels[2].steerAngle = stear-5;
            wheels[3].steerAngle = stear-5;
        }

        if (nextPoint.magnitude <= distanceFromPath)
        {
            currentIndex++;
            if (currentIndex >= Path.Count)
            {
                currentIndex = 0;
            }
        }
        
       
       
    }

    private float roundToFullAngle(float angle)
    {
        float m = Math.Min(Math.Abs(angle - 90), Math.Min(Math.Abs(angle - 180), Math.Min(Math.Abs(angle - 270), Math.Abs(angle - 0))));
        if (angle+m == 90 || angle+m == 180 || angle+m == 270 || angle+m == 0)
        {
            return angle + m;
        }
        else
        {
            return angle-m;
        }
    }

    private void resetAllWheelsAngles()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].steerAngle= 0;
        }
    }

}
