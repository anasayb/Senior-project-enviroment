using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditorInternal;
using UnityEngine;

public class CarControllerTwo : MonoBehaviour
{
    
    public WheelCollider[] wheels;
    public float MAX_SPEED = 20f;

    [Header("Turning")]
    public bool[] left = { false, false };
    public bool[] right = { false, false };
    public Transform pathGourpLeft, pathGourpRight;
    public float distanceFromPath = 2f;
    public Vector3 posForwardCenter;
    public int CurrentIntersection = 0;
    public int CurrentDirection = 0;

    private bool doneTurning = false;
    private float MAX_TURNING_ANGLE = 20f;
    private float MAX_TURNING_ANGLE_Right = 75f;
    private List<Transform> LeftPath = new List<Transform>();
    private List<Transform> RightPath = new List<Transform>();
    private float FORCE_STOP = 1600f;
    private bool turning = false;
    private int currentIndexLeft = 0, currentIndexRight = 0;
    private bool deaccelrate = false;

    [Header("Sensors")]
    public float sensorLength = 10f;

    private float tempSensorLength;
    private float colide = -1;
    private float yellowLight = 1f;

    [Header("Wating Time")]
    public LayerMask CarLay;
    public float waitngTime = 0;

    [Header("GUI")]
    public GameObject carInfo;
    public Material tent;
    public GameObject sel;

    private Material orignal;

    // Start is called before the first frame update
    void Start()
    {
        getPaths();
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

        // movement of the car
        move();

        // Calculate the wating time of the car
        calculateWatingTime();

        //updating the car info in car info
        updateCarInfo();

    }


  
    /// <summary>
    /// Method <c>checkColide</c> This function check weather there is a colide infront of the car.
    /// </summary>
    private void checkColide()
    {
        // Store hit inforamtion
        RaycastHit hit;

        // The Ray start postion
        posForwardCenter = transform.position;
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
        if (   Physics.Raycast(posForwardCenter, dir, out hit, (sensorLength + speed.velocity.magnitude)) 
            || Physics.Raycast(posForwardRight,  dir, out hit, (sensorLength + speed.velocity.magnitude)) 
            || Physics.Raycast(posForwardleft,  dir, out hit, (sensorLength + speed.velocity.magnitude))  
            // || Physics.Raycast(posRight, rightDir, out hit, (6.5f / 2.0f)) 
            // || Physics.Raycast(posLeft, leftDir, out hit, (6.5f / 2.0f))
            )
        {

            // If the colider is yellow light
            if (hit.collider.isTrigger == true)
            {
                
                // If the car needs more then 2 seconds to pass the intersection, the car will stop.
                if (hit.distance/speed.velocity.magnitude > 1)
                {
                    findHit = true;
                    colide = hit.distance;

                }
            }

            // If its not the same object 
            else if (hit.collider.name != transform.name)
            {
                //Debug Code
                //print("hello its me the freaking " + transform.gameObject.name);

                //Debug.DrawRay(posForwardCenter, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                //Debug.DrawRay(posForwardRight, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                //Debug.DrawRay(posForwardleft, dir * (sensorLength + speed.velocity.magnitude), Color.red);
                //Debug.DrawRay(posRight, rightDir * (6.5f / 2.0f), Color.red);
                //Debug.DrawRay(posLeft, leftDir * (6.5f / 2.0f), Color.red);
                
                findHit = true;
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

            // Reset colide
            colide = -1;
        }



        // Turning Code
        if (Physics.Raycast(posForwardCenter, -(transform.up), out hit, sensorLength))
        {

            // If the car enter the intersection sqaure 
            if (hit.collider.tag == "IntersectionArea0" || hit.collider.tag == "IntersectionArea1")
            {
                turning = true;
            }
            else
            {
                turning = false;
                if (hit.collider.name != transform.name && CurrentIntersection != (hit.collider.tag[hit.collider.tag.Length - 1] - '0'))
                {
                    fixPostion();
                    waitngTime = 0;
                    doneTurning = false;
                    currentIndexLeft = 0;
                    currentIndexRight = 0;
                    CurrentIntersection = hit.collider.tag[hit.collider.tag.Length - 1] - '0';
                    string[] names = { "North", "West", "South", "East" };
                    string temp = hit.collider.transform.parent.name.Split(' ')[0];
                    for (int i = 0; i < names.Length; i++) if (names[i] == temp) CurrentDirection = i;
                    getPaths();
                }
            }

        }
        else
        {
            // End of the map, no road under you
            if (carInfo.transform.Find("Car Name").GetComponent<TMP_Text>().text == name) { 
                sel.SetActive(false);
                carInfo.SetActive(false);
            }
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
        if (AI_two_single.startCouting || Traditional_traffic_Controller.startCouting || Basic_algo.startCouting) {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit h;
            if (Physics.Raycast(ray, out h, 230, ~CarLay) && deaccelrate)
            {

                Rigidbody speed = GetComponent<Rigidbody>();
                if (speed.velocity.magnitude <= 2)
                {
                    waitngTime += Time.deltaTime;
                }


            }

            Avg_wating_time_two.updateAvg(transform.name, waitngTime, left[0], right[0], left[1], right[1], CurrentIntersection, transform.parent.name);

            // Debug code
            // Debug.DrawRay(ray.origin, transform.forward*300, Color.blue);
        }
    }

    /// <summary>
    /// Method <c>move</c> This function Controll the movment of a car form speeding up to stopping.
    /// </summary>
    private void move()
    {

        if(colide != -1)
        {

            // If a colide is  detected
            deaccelerate(0);
            return;

        }
        


        // If you are in the area and the left flag is ON
   
        if (left[CurrentIntersection])
        {

            turnLeft();

        }
        else if (right[CurrentIntersection])
        {
            turnRight();
        }




        if (colide == -1)
        {
            // If a colide is NOT detected
            if (turning)
            {
                accelerate(150);
            }
            else
            {
                accelerate(300);
            }
           

        }
 
    }


    /// <summary>
    /// Method <c>maccelerateove</c> This function Accelerate the car.
    /// </summary>
    private void accelerate(float torque)   
    {
        deaccelrate = false;
        Rigidbody speed = GetComponent<Rigidbody>();
        speed.drag = 0;

        // If the car speed reached the maximum, stop incresing the speed
        if (speed.velocity.magnitude < MAX_SPEED)
        {
            // Increase the speed
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = torque;
                wheels[i].brakeTorque = 0;
            }
        }
        else
        {
            // Calculate the force need tos top the car
            float acc = (MAX_SPEED - speed.velocity.magnitude) / colide;
            float force = -1 * speed.mass * acc;

            // Apply break torque on all wheels
            speed.drag = 1;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = 0;
                wheels[i].brakeTorque = force / wheels.Length;
            }
        }

    }


    /// <summary>
    /// Method <c>deaccelerate</c> This function stop the car.
    /// </summary>
    /// <param name="finalSpeed">the target speed</param>
    private void deaccelerate(float finalSpeed)
    {
        deaccelrate = true;
        Rigidbody speed = GetComponent<Rigidbody>();
        
        
        float force = 0;
       

        if (colide < sensorLength && sensorLength != 2)
        {
            force = FORCE_STOP;
        }
        else
        {
            // Calculate the force need tos top the car
            float acc = (finalSpeed - speed.velocity.magnitude) / colide;
            force = -1 * speed.mass * acc;
        }

        if (force < 0)
        {
            accelerate(300);
            return;
        }

        // Apply break torque on all wheels
        speed.drag = 1;
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].motorTorque = 0;
            wheels[i].brakeTorque = force/ wheels.Length;
        }
        
    }


    /// <summary>
    /// Method <c>getPath</c> This function add the path of the left turn to the "Path" list.
    /// </summary>
    private void getPaths()
    {
        if (CurrentIntersection < 2)
        {
            pathGourpLeft = Paths.LeftPaths[CurrentIntersection, CurrentDirection];
            pathGourpRight = Paths.RightPaths[CurrentIntersection, CurrentDirection];

            LeftPath.Clear();
            RightPath.Clear();

            int count = pathGourpLeft.childCount;
            for (int i = 0; i < count; i++)
            {
                LeftPath.Add(pathGourpLeft.GetChild(i).transform);
            }

            count = pathGourpRight.childCount;
            for (int i = 0; i < count; i++)
            {
                RightPath.Add(pathGourpRight.GetChild(i).transform);
            }
        }

    }


    /// <summary>
    /// Method <c>turnLeft</c> This function make the car turn left.
    /// </summary>
    private void turnLeft()
    {

        if (!turning)
        {
           
            if (transform.InverseTransformPoint(LeftPath[0].position.x, transform.position.y, LeftPath[0].position.z).magnitude <= 30)
            {   
                Rigidbody speed = GetComponent<Rigidbody>();
                if (speed.velocity.magnitude > 8)
                {
                    colide = transform.InverseTransformPoint(LeftPath[0].position.x, transform.position.y, LeftPath[0].position.z).magnitude;
                    deaccelerate(8);
                }
                

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

            

        }
        else
        {

            sensorLength = 2;

            // The postion of the car
            Vector3 currentPos = transform.position;

            // The distance between the car and the next point in the path
            Vector3 nextPoint = transform.InverseTransformPoint(LeftPath[currentIndexLeft].position.x, transform.position.y, LeftPath[currentIndexLeft].position.z);

            // Calculate the stear angle
            float stear = MAX_TURNING_ANGLE * (nextPoint.x / nextPoint.magnitude);

            // Unfreez the roation contrain
            GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezeAll;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            wheels[0].steerAngle = stear;
            wheels[1].steerAngle = stear;

            // if the object is a truk increase the stear angle
            if (transform.tag == "Truck")
            {
                wheels[0].steerAngle -= 5;
                wheels[1].steerAngle -= 5;
                wheels[2].steerAngle = stear - 5;
                wheels[3].steerAngle = stear - 5;
            }
            else if (transform.tag == "Bus")
            {
                wheels[0].steerAngle -= 5;
                wheels[1].steerAngle -= 5;

            }

            // if the distance to teh point is less than the specifed distance go to next point
            if (nextPoint.magnitude <= distanceFromPath)
            {
                currentIndexLeft++;
                if (currentIndexLeft >= LeftPath.Count)
                {
                    currentIndexLeft = 0;
                   
                }
            }

           
        }

                     
       
    }


    /// <summary>
    /// Method <c>turnLeft</c> This function make the car turn left.
    /// </summary>
    private void turnRight()
    {

        if (!turning)
        {

            if (transform.InverseTransformPoint(RightPath[0].position.x, transform.position.y, RightPath[0].position.z).magnitude <= 20 && !doneTurning)
            {
                Rigidbody speed = GetComponent<Rigidbody>();
                if (speed.velocity.magnitude > 5)
                {
                    colide = transform.InverseTransformPoint(RightPath[0].position.x, transform.position.y, RightPath[0].position.z).magnitude;
                    deaccelerate(5);
                }

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

        }
        else
        {
            doneTurning = true;
            sensorLength = 2;

            // The postion of the car
            Vector3 currentPos = transform.position;

            // The distance between the car and the next point in the path
            Vector3 nextPoint = transform.InverseTransformPoint(RightPath[currentIndexRight].position.x, transform.position.y, RightPath[currentIndexRight].position.z);

            // Calculate the stear angle
            float stear = MAX_TURNING_ANGLE_Right * (nextPoint.x / nextPoint.magnitude);

            // Unfreez the roation contrain
            GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezeAll;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            wheels[0].steerAngle = stear;
            wheels[1].steerAngle = stear;

            // if the object is a truk increase the stear angle
            if (transform.tag == "Truck")
            {
                wheels[0].steerAngle += 10;
                wheels[1].steerAngle += 10;
                wheels[2].steerAngle = stear + 10;
                wheels[3].steerAngle = stear + 10;
            }

            // if the distance to teh point is less than the specifed distance go to next point
            if (nextPoint.magnitude <= distanceFromPath)
            {
                currentIndexRight++;
                if (currentIndexRight >= RightPath.Count)
                {
                    currentIndexRight = 0;
                   
                }
            }

        }

    }


    /// <summary>
    /// Method <c>roundToFullAngle</c> This function round the angle to the nearest corner angle.
    /// </summary>
    /// <param name="angle">the angle value to which need to be round/param>
    private float roundToFullAngle(float angle)
    {
        float m = Math.Min(Math.Abs(angle - 0), Math.Min(Math.Abs(angle - 90), Math.Min(Math.Abs(angle - 180), Math.Min(Math.Abs(angle - 270), Math.Abs(angle - 360)))));
        if (angle+m == 90 || angle+m == 180 || angle+m == 270 || angle+m == 360 || angle+m == 0)
        {
            if (angle+m == 360)
            {
                return 0;
            }
            return angle + m;
        }
        else
        {
            return angle-m;
        }
    }


    private void fixPostion()
    {
        //Vector3 val = Vector3.Scale(transform.right, transform.localPosition);
        float pos = transform.localPosition.z;
        float m = Math.Min(Math.Abs(pos - 6.6f), Math.Min(Math.Abs(pos - 13.07f), Math.Min(Math.Abs(pos - -6.8f), pos - -13.16f)));
        if (pos + m == 6.6f || pos + m == 13.07f || pos + m == -6.8f || pos + m == -13.16f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, pos + m);

        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, pos - m);
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
    /// Method <c>OnMouseDown</c> This function is used to update the infromation of the car in the info box if the car is selcted.
    /// </summary>
    private void updateCarInfo()
    {
        if (carInfo.transform.Find("Car Name").GetComponent<TMP_Text>().text == name)
        {
            carInfo.transform.Find("Speed").GetComponent<TMP_Text>().text = "Speed: " + Math.Floor(GetComponent<Rigidbody>().velocity.magnitude).ToString() + " km/s";
            carInfo.transform.Find("Waiting Time").GetComponent<TMP_Text>().text = "Waiting Time: " + waitngTime.ToString("0.00") + " s";
            sel.transform.position = new Vector3(transform.position.x, 5f, transform.position.z);

            string[] direction = { "North", "West", "South", "East" };
            carInfo.transform.Find("Intersection Enter").GetComponent<TMP_Text>().text = "Intersection Enter Direction: " + direction[CurrentDirection];

            // Display the intersection which the car at
            carInfo.transform.Find("Intersection").GetComponent<TMP_Text>().text = "Intersection " + (CurrentIntersection+1);


            if (!(right[CurrentIntersection] || left[CurrentIntersection]))
            {
                carInfo.transform.Find("Intersection Exit").GetComponent<TMP_Text>().text = "Intersection Exit Direction: " + direction[(CurrentDirection + 2) % 4];
            }
            else if (right[CurrentIntersection])
            {
                carInfo.transform.Find("Intersection Exit").GetComponent<TMP_Text>().text = "Intersection Exit Direction: " + direction[(CurrentDirection + 1) % 4];
            }
            else if (left[CurrentIntersection])
            {
                carInfo.transform.Find("Intersection Exit").GetComponent<TMP_Text>().text = "Intersection Exit Direction: " + direction[(CurrentDirection + 3) % 4];
            }


            // To enbale the animation
            sel.GetComponentInChildren<Animation>().enabled = true;
        }

    }


    /// <summary>
    /// Method <c>OnMouseDown</c> This function is called when the user clicks on the car object in order to display the car info in a box.
    /// </summary>
    private void OnMouseDown()
    {
        if (Scence_Manger.inv == 1)
        {
            // Activate the selector and change its postion ot the center of the car
            sel.SetActive(true);
            sel.transform.position = new Vector3(transform.position.x, 5f, transform.position.z);


            // Activate the carInfo boc
            carInfo.SetActive(true);

            // Fill the infroamtion of the carInfo box
            carInfo.transform.Find("Car Name").GetComponent<TMP_Text>().text = name;
            carInfo.transform.Find("Speed").GetComponent<TMP_Text>().text = "Speed: " + Math.Floor(GetComponent<Rigidbody>().velocity.magnitude).ToString("F2") + " km/s";
            carInfo.transform.Find("Waiting Time").GetComponent<TMP_Text>().text = "Waiting Time: " + (((int)(waitngTime * 100)) / 100f).ToString() + " s";

            string[] direction = { "North", "West", "South", "East" };
            carInfo.transform.Find("Intersection Enter").GetComponent<TMP_Text>().text = "Intersection Enter Direction: " + direction[CurrentDirection];

            // Display the intersection which the car at
            carInfo.transform.Find("Intersection").GetComponent<TMP_Text>().text = "Intersection " + (CurrentIntersection+1);


            if (!(right[CurrentIntersection] || left[CurrentIntersection]))
            {
                carInfo.transform.Find("Intersection Exit").GetComponent<TMP_Text>().text = "Intersection Exit Direction: " + direction[(CurrentDirection + 2) % 4];
            }
            else if (right[CurrentIntersection])
            {
                carInfo.transform.Find("Intersection Exit").GetComponent<TMP_Text>().text = "Intersection Exit Direction: " + direction[(CurrentDirection + 1) % 4];
            }
            else if (left[CurrentIntersection])
            {
                carInfo.transform.Find("Intersection Exit").GetComponent<TMP_Text>().text = "Intersection Exit Direction: " + direction[(CurrentDirection + 3) % 4];
            }

            
        }

    }



    /// <summary>
    /// Method <c>OnMouseDown</c> This function is called when the user mouse enter the car object in order to highlight the car.
    /// </summary>
    private void OnMouseEnter()
    {
        if (Scence_Manger.inv == 1)
        {
            // Change the matrial used to color the car into a slightly darker one
            orignal = transform.Find("Body").GetComponentInChildren<MeshRenderer>().materials[0];
            Material[] mt = transform.Find("Body").GetComponentInChildren<MeshRenderer>().materials;
            mt[0] = tent;
            if (transform.tag == "Truck")
            {
                Material[] mt2 = transform.Find("Body").GetChild(0).GetChild(0).GetComponentInChildren<MeshRenderer>().materials;
                mt2[0] = tent;
                transform.Find("Body").GetChild(0).GetChild(0).GetComponentInChildren<MeshRenderer>().materials = mt2;
            }
            transform.Find("Body").GetComponentInChildren<MeshRenderer>().materials = mt;
        }
    }


    /// <summary>
    /// Method <c>OnMouseDown</c> This function is called when the user mouse exit the car object in order to dehighlight the car.
    /// </summary>
    private void OnMouseExit()
    {
        if (Scence_Manger.inv == 1)
        {
            // Change the matrial used to the original one
            Material[] mt = transform.Find("Body").GetComponentInChildren<MeshRenderer>().materials;
            mt[0] = orignal;
            if (transform.tag == "Truck")
            {
                Material[] mt2 = transform.Find("Body").GetChild(0).GetChild(0).GetComponentInChildren<MeshRenderer>().materials;
                mt2[0] = orignal;
                transform.Find("Body").GetChild(0).GetChild(0).GetComponentInChildren<MeshRenderer>().materials = mt2;
            }
            transform.Find("Body").GetComponentInChildren<MeshRenderer>().materials = mt;
        }
    }



    /*
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
        lr.SetPosition(1, start + length);
        GameObject.Destroy(myLine, duration);

    }
    */

    }
