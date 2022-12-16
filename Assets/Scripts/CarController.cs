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
    public float MAX_SPEED = 30f;

    [Header("Turning")]
    public bool left = false, right = false;
    public Transform pathGourp;
    public float distanceFromPath = 2f;

    private float MAX_TURNING_ANGLE = 20f;
    private List<Transform> Path = new List<Transform>();
    private float FORCE_STOP = 1500f;
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
        // Store hit inforamtion
        RaycastHit hit;

        // The Ray start postion
        Vector3 posForwardCenter = transform.position;
        posForwardCenter.y = 1.5f;

        Vector3 posForwardRight = transform.position;
        posForwardRight += (GetComponent<BoxCollider>().size.x / 2 * transform.right);
        posForwardRight.y = 1.5f;

        Vector3 posForwardleft = transform.position;
        posForwardleft += (GetComponent<BoxCollider>().size.x / 2 * (-1 * transform.right));
        posForwardleft.y = 1.5f;

        Vector3 posRight = transform.position;
        posRight += (GetComponent<BoxCollider>().size.z/2 * transform.forward);
        posRight += (GetComponent<BoxCollider>().size.x/2 * transform.right);


        Vector3 posLeft = transform.position;
        posLeft += (GetComponent<BoxCollider>().size.z / 2 * transform.forward);
        posLeft += (GetComponent<BoxCollider>().size.x / 2 * (-1 * transform.right));


        // The direction of the Ray (on the z axis)
        Vector3 dir = transform.forward;
        Vector3 rightDir = (transform.forward + transform.right);
        Vector3 leftDir =  (transform.forward + -1*transform.right);

        // Speed
        Rigidbody speed = GetComponent<Rigidbody>();

        // If a colide is detected
        bool findHit = false;
        if (   Physics.Raycast(posForwardCenter, dir, out hit, (sensorLength + speed.velocity.magnitude) * 1.25f) 
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

                //Debug.DrawRay(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                //Debug.DrawRay(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                //Debug.DrawRay(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                //Debug.DrawRay(posRight, rightDir * (6.5f / 2.0f), Color.red);
                //Debug.DrawRay(posLeft, leftDir * (6.5f / 2.0f), Color.red);
                
                findHit = true;
                DrawLine(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                DrawLine(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                DrawLine(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.red);

                colide = hit.distance;
               
            }
            

        }

        // If no hit is found
        if (!findHit) { 

            // Debug Code
            //Debug.DrawRay(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            //Debug.DrawRay(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            //Debug.DrawRay(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            //Debug.DrawRay(posRight, rightDir * (6.5f / 2.0f), Color.green);
            //Debug.DrawRay(posLeft, leftDir * (6.5f / 2.0f), Color.green);

            DrawLine(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            DrawLine(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.green);
            DrawLine(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.green);

            // Reset colide
            colide = -1;
        }

        

        // Turning Code
        if (Physics.Raycast(posForwardCenter, -(transform.up), out hit, sensorLength))
        {   

            // If the car enter the intersection sqaure 
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
            // End of the map, no road under you
            Avg_wating_time.increaseCarNumber();
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
            if (speed.velocity.magnitude < 1)
            {
                waitngTime += Time.deltaTime;
            }

            
        }

        Avg_wating_time.updateAvg(transform.name, waitngTime);

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
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
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
                for (int i =0; i < wheels.Length; i++)
                {
                    wheels[i].brakeTorque = 0;
                }
                for (int i = 2; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = 200;
                }

            }


        }
        else
        {
            // A colide IS detected, stop the car

            // Calculate the force need tos top the car
            float force = 0;

            if (colide <= sensorLength)
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
                wheels[i].motorTorque = 0;
                wheels[i].brakeTorque = force;
            }
        }
    }


    /// <summary>
    /// Method <c>getPath</c> This function add the path of the left turn to the "Path" list.
    /// </summary>
    void getPath()
    {
        int count = pathGourp.childCount;
        for (int i = 0; i < count; i++)
        {
            Path.Add(pathGourp.GetChild(i).transform);
        }
       
    }


    /// <summary>
    /// Method <c>turnLeft</c> This function make the car turn left.
    /// </summary>
    void turnLeft()
    {
        
        // The postion of the car
        Vector3 currentPos = transform.position;

        // The distance between the car and the next point in the path
        Vector3 nextPoint = transform.InverseTransformPoint(Path[currentIndex].position.x, transform.position.y, Path[currentIndex].position.z);
        
        // Calculate the stear angle
        float stear = MAX_TURNING_ANGLE *(nextPoint.x / nextPoint.magnitude);
        
        // Unfreez the roation contrain
        GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezeAll;
        wheels[0].steerAngle = stear;
        wheels[1].steerAngle = stear;

        // if the object is a truk increase the stear angle
        if (transform.tag == "Truck")
        {
            wheels[2].steerAngle = stear-5;
            wheels[3].steerAngle = stear-5;
        }

        // if the distance to teh point is less than the specifed distance go to next point
        if (nextPoint.magnitude <= distanceFromPath)
        {
            currentIndex++;
            if (currentIndex >= Path.Count)
            {
                currentIndex = 0;
            }
        }
        
       
       
    }


    /// <summary>
    /// Method <c>roundToFullAngle</c> This function round the angle to the nearest corner angle.
    /// </summary>
    /// <param name="angle">the angle value to which need to be round/param>
    private float roundToFullAngle(float angle)
    {
        float m = Math.Min(Math.Abs(angle - 90), Math.Min(Math.Abs(angle - 180), Math.Min(Math.Abs(angle - 270), Math.Abs(angle))));
        if (angle+m == 90 || angle+m == 180 || angle+m == 270 || angle+m == 0)
        {
            return angle + m;
        }
        else
        {
            return angle-m;
        }
    }


    /// <summary>
    /// Method <c>roundToFullAngle</c> This function reset the stear angle to zeros.
    /// </summary>
    private void resetAllWheelsAngles()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].steerAngle= 0;
        }
    }


    /// <summary>
    /// Method <c>DrawLine</c> This function Draw ray lines in the game mode.
    /// </summary>
    /// <param name="start">the vector where the length start</param>
    /// <param name="length">the lien length</param>
    /// <param name="start">the clor of the line</param>
    /// <param name="duration">the duration on which the line apears in the screen</param>
    private void DrawLine(Vector3 start, Vector3 length, Color color, float duration = 0.1f)
    {
       
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lr.startColor = color; lr.endColor = color;
        lr.startWidth = 0.1f; lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, start+length);
        GameObject.Destroy(myLine, duration);
        
    }


}
