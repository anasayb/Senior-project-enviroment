using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class Basic_algo : MonoBehaviour
{
    public Component[] trafficLights;

    private float[] time = { 0, 0, 0, 0 }; // i assumed we will show him without the menu because its not fixed as u know 
    public float delay = 1; // im not sure if we need this 
    public float yellowLightDuration = 2f;
    public static int carNumberNorth;
    private float timeVariable = 0f;
    private int direction = 0;
    private bool oneTimeRun = true;
    private bool secondoneTimeRun = false;
    private int oldDirection = 1;
    private int newDirection;
    // private bool EmergencyCarMoving = true; 
    private float EmegencyTimeVariable;
    private int currentEmergencyDirection = -1;
    private bool justFinishedEmergency = false;
    private int[] turnWaiting = { 0, 0, 0, 0 };



    private PriorityQueue<NodeClass> queue = new PriorityQueue<NodeClass>();

    [Header("GUI")]
    public GameObject timer;

    [Header("Green Time Calculation")]
    // public int[] carNumber;
    public CarCounter[] CarCount;


    [Header("Cameras")]
    public GameObject Maincamera;
    public GameObject CameraController;




    // Start is called before the first frame update
    void Start()
    {

        direction = Scence_Manger.dir;
        NodeClass North = new NodeClass(0, CarCount[0].carsCounter);
        queue.Enqueue(North);
        time[queue.Peek().direction] = GreenTimeCalc(CarCount[queue.Peek().direction].carsCounter);



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentEmergencyDirection != -1)
        {
            EmegencyTimeVariable += Time.deltaTime;
            if (EmegencyTimeVariable <= yellowLightDuration)
            {
                ChangeLightYellow(direction);
                timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
            }

            if (EmegencyTimeVariable >= yellowLightDuration && EmegencyTimeVariable < yellowLightDuration + delay)
            {
                // EmergencyCarMoving = true;
                ChangeLightRed(direction);
            }

            if (EmegencyTimeVariable >= yellowLightDuration + delay)
            {
                if (CarCount[currentEmergencyDirection].emergencyExist)
                {
                    ChangeLightGreen(currentEmergencyDirection);
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                }
                else
                {

                    timeVariable = 0f;
                    EmegencyTimeVariable = 0;
                    time[direction] = 0;
                    turnWaiting[direction] = 0;
                    direction = currentEmergencyDirection;
                    time[direction] = yellowLightDuration;
                    currentEmergencyDirection = -1;
                    justFinishedEmergency = true;

                }
            }
            return;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (CarCount[i].emergencyExist)
                {
                    currentEmergencyDirection = i;
                    if (currentEmergencyDirection == direction)
                    {
                        EmegencyTimeVariable = yellowLightDuration + delay;
                    }
                    return;
                }
            }
        }

        //  Debug.Log(queue.Peek());
        timeVariable += Time.deltaTime;
        /*Debug.Log(" Direction : " + direction +
                  " Time assigned : " + time[direction] +
                  " Car count : " + CarCount[direction].carsCounter);*/
        timer.GetComponentInChildren<TMP_Text>().text = Math.Max((Math.Ceiling(time[direction] - timeVariable)), 0).ToString();
        timer.transform.GetChild(1).GetComponent<Image>().fillAmount = (1 - (timeVariable / time[direction]));

        if (timeVariable >= time[direction] - yellowLightDuration && timeVariable <= time[direction])
        {
            ChangeLightYellow(direction);
            timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
        }

        if (timeVariable >= time[direction])
        {
            // EmergencyCarMoving = true;
            ChangeLightRed(direction);
        }

        if (timeVariable >= time[direction] + delay)
        {
            if (oneTimeRun)
            {
                oneTimeRun = false;
                queue.Clear();
                NodeClass North = new NodeClass(0, CarCount[0].carsCounter);
                NodeClass South = new NodeClass(2, CarCount[2].carsCounter);
                NodeClass West = new NodeClass(1, CarCount[1].carsCounter);
                NodeClass East = new NodeClass(3, CarCount[3].carsCounter);
                queue.Enqueue(North);
                queue.Enqueue(South);
                queue.Enqueue(West);
                queue.Enqueue(East);
                direction = queue.Peek().direction;
                time[queue.Peek().direction] = GreenTimeCalc(queue.Peek().CarCount);
                if (time[direction] != 0)
                {
                    timeVariable = 0f;
                    ChangeLightGreen(direction);
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                }

            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i == direction)
                    {
                        turnWaiting[i] = 0;
                        continue;
                    }
                    else
                    {
                        turnWaiting[i]++;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (turnWaiting[i] >= 3)
                    {
                        queue.Clear();
                        secondoneTimeRun = false;
                        justFinishedEmergency = false;
                        switch (i)
                        {
                            case 0:
                                queue.Enqueue(new NodeClass(0, CarCount[0].carsCounter + 99));
                                queue.Enqueue(new NodeClass(2, CarCount[2].carsCounter));
                                queue.Enqueue(new NodeClass(1, CarCount[1].carsCounter));
                                queue.Enqueue(new NodeClass(3, CarCount[3].carsCounter));
                                turnWaiting[i] = 0;
                                break;
                            case 1:
                                queue.Enqueue(new NodeClass(1, CarCount[1].carsCounter + 99));
                                queue.Enqueue(new NodeClass(0, CarCount[0].carsCounter));
                                queue.Enqueue(new NodeClass(2, CarCount[2].carsCounter));
                                queue.Enqueue(new NodeClass(3, CarCount[3].carsCounter));
                                turnWaiting[i] = 0;
                                break;
                            case 2:
                                queue.Enqueue(new NodeClass(2, CarCount[2].carsCounter + 99));
                                queue.Enqueue(new NodeClass(0, CarCount[0].carsCounter));
                                queue.Enqueue(new NodeClass(1, CarCount[1].carsCounter));
                                queue.Enqueue(new NodeClass(3, CarCount[3].carsCounter));
                                turnWaiting[i] = 0;
                                break;
                            case 3:
                                queue.Enqueue(new NodeClass(3, CarCount[3].carsCounter + 99));
                                queue.Enqueue(new NodeClass(0, CarCount[0].carsCounter));
                                queue.Enqueue(new NodeClass(2, CarCount[2].carsCounter));
                                queue.Enqueue(new NodeClass(1, CarCount[1].carsCounter));
                                turnWaiting[i] = 0;
                                break;
                            default:
                                Debug.Log("Weird error happened Here ");
                                queue.Enqueue(new NodeClass(oldDirection, CarCount[oldDirection].carsCounter));
                                break;
                        }

                        direction = queue.Peek().direction;
                        time[direction] = GreenTimeCalc(CarCount[direction].carsCounter);
                        if (time[direction] != 0)
                        {
                            timeVariable = 0f;
                            ChangeLightGreen(direction);
                            timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                        }
                        turnWaiting[i] = 0;
                        break;
                    }
                    secondoneTimeRun = true;
                }
                if (justFinishedEmergency)
                {
                    queue.Clear();
                    justFinishedEmergency = false;
                    NodeClass North = new NodeClass(0, CarCount[0].carsCounter);
                    NodeClass South = new NodeClass(2, CarCount[2].carsCounter);
                    NodeClass West = new NodeClass(1, CarCount[1].carsCounter);
                    NodeClass East = new NodeClass(3, CarCount[3].carsCounter);
                    queue.Enqueue(North);
                    queue.Enqueue(South);
                    queue.Enqueue(West);
                    queue.Enqueue(East);
                    direction = queue.Peek().direction;
                    time[queue.Peek().direction] = GreenTimeCalc(queue.Peek().CarCount);
                    if (time[direction] != 0)
                    {
                        timeVariable = 0f;
                        ChangeLightGreen(direction);
                        timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                    }

                }
                else if (secondoneTimeRun)
                {
                    oldDirection = queue.Peek().direction;
                    queue.Dequeue();
                    direction = queue.Peek().direction;
                    time[direction] = GreenTimeCalc(CarCount[direction].carsCounter);
                    if (time[direction] != 0)
                    {
                        timeVariable = 0f;
                        ChangeLightGreen(direction);
                        timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                    }
                    queue.Enqueue(new NodeClass(oldDirection, CarCount[oldDirection].carsCounter));
                }
            }
        }


    }




    /// <summary>
    /// Method <c>ChangeLightRed</c> make the current traffic light red.
    /// </summary>
    /// <param name="to">the current traffic light to be red</param>
    private void ChangeLightRed(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToRed();

    }

    /// <summary>
    /// Method <c>ChangeLightYellow</c> make the current traffic light yellow.
    /// </summary>
    /// <param name="to">the current traffic light to be yellow</param>
    private void ChangeLightYellow(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToYellow();

    }

    /// <summary>
    /// Method <c>ChangeLightGreen</c> make the next traffic light green.
    /// </summary>
    /// <param name="to">the next traffic light to be green</param>
    private void ChangeLightGreen(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToGreen();
        Maincamera.GetComponent<User_Camera_Controll>().updateCameras(to);

    }

    private int GreenTimeCalc(int carNo)
    {
        // Just temp simple formula 
        int greenTime = (carNo * 2) + (int)delay; // here is our simple formula so far i need more time to dig and get the proper and suitable one , i substracted the yellow time so it does not count up there
        if (greenTime >= 30)
        {
            return 30 + 1;
        }
        else if (carNo == 0) // if there is no cars 
        {
            return 0;
        }
        else
        {
            return greenTime + 1;
        }

    }

}
/*        if (EmergencyCarMoving)
        {
            for (int i = 0; i < 4; i++)
            {
                if (CarCount[i].emergencyExist)
                {
                    time[direction] = 0;
                    time[i] = GreenTimeCalc(3);
                    ChangeLightYellow(direction);
                    ChangeLightRed(direction);
                    direction = i;
                    timeVariable = 0f;
                    ChangeLightGreen(i);
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                    EmergencyCarMoving = false;
                    break;
                }

            }
 Prevents multiple emergency cars to move at one time
}*/