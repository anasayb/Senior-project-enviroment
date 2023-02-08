using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class Basic_algo : MonoBehaviour
{
    public static bool startCouting = false;

    public Component[] trafficLights;

    private float[] time = { 0, 0, 0, 0 }; // i assumed we will show him without the menu because its not fixed as u know 
    public float delay = 1; // im not sure if we need this 
    public float yellowLightDuration = 2f;
    public static int carNumberNorth;
    private float timeVariable = 0f;
    private int direction = 0;
    private bool oneTimeRun = true;
    private bool noStarvation = false;
    private int oldDirection = 1;
    private float EmegencyTimeVariable;
    int temp = -1;
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
    //public GameObject Maincamera;
    public GameObject cameraController;
    private int Intersection = 0;



    // Start is called before the first frame update
    void Start()
    {
        Basic_algo.startCouting = false;
        direction = Scence_Manger.dir;
        NodeClass North = new NodeClass(0, CarCount[0].carsCounter);
        queue.Enqueue(North);
        Debug.Log(CarCount[0].carsCounter);
        time[queue.Peek().direction] = GreenTimeCalc(CarCount[queue.Peek().direction].carsCounter) - 2;
        Intersection = transform.parent.name[transform.parent.name.Length - 1] - '0' - 1;
        if (Scence_Manger.algorthim != "Carload based Traffic Light System")
        {
            GetComponent<Basic_algo>().enabled = false;
            return;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CarCount[0].carsCounter + CarCount[1].carsCounter + CarCount[2].carsCounter + CarCount[3].carsCounter != 0)
        {
            // This piece of code job is to check if there is emergency car if true it wil give it priority without specific time 
            if (currentEmergencyDirection != -1)
            {
                Basic_algo.startCouting = true;
                EmegencyTimeVariable += Time.deltaTime;
                if (User_Controll.Intersection == Intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().text = "EM";
                    timer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;
                }
                if (EmegencyTimeVariable <= yellowLightDuration)
                {
                    ChangeLightYellow(direction);
                    if (User_Controll.Intersection == Intersection)
                    {
                        timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
                    }
                }

                if (EmegencyTimeVariable >= yellowLightDuration && EmegencyTimeVariable < yellowLightDuration + delay)
                {
                    ChangeLightRed(direction);
                }

                if (EmegencyTimeVariable >= yellowLightDuration + delay)
                {
                    if (CarCount[currentEmergencyDirection].emergencyExist)
                    {
                        ChangeLightGreen(currentEmergencyDirection);
                        if (User_Controll.Intersection == Intersection)
                        {
                            timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                        }
                    }
                    else
                    {
                        if (oneTimeRun)
                        {
                            temp = direction;
                        }
                        ResetEmergency();

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
            // End of the Emergency car control
            Basic_algo.startCouting = true;
            Timer();
            // Normal System Behaviour 
            if (timeVariable >= time[direction] - yellowLightDuration && timeVariable <= time[direction])
            {
                ChangeLightYellow(direction);
                if (User_Controll.Intersection == Intersection)
                {
                    timer.GetComponentInChildren<TMP_Text>().color = new Color(0.885f, 0.434f, 0f);
                }
            }

            if (timeVariable >= time[direction])
            {
                ChangeLightRed(direction);
            }

            if (timeVariable >= time[direction] + delay)
            {
                if (oneTimeRun)
                {
                    Debug.Log("TEST");
                    oneTimeRun = false;
                    ResetQueue();
                    if (temp == direction)
                    {
                        getNextTurn();
                        return;
                    }
                    if (time[direction] != 0)
                    {
                        timeVariable = 0f;
                        ChangeLightGreen(direction);
                        if (User_Controll.Intersection == Intersection)
                        {
                            timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                        }
                    }

                }
                else
                {
                    turnWaitingCount();
                    Aging();
                    if (justFinishedEmergency)
                    {
                        justFinishedEmergency = false;
                        ResetQueue();
                        if (time[direction] != 0)
                        {
                            timeVariable = 0f;
                            ChangeLightGreen(direction);
                                if (User_Controll.Intersection == Intersection)
                                {
                                    timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                                }
                        }

                    }
                    else if (noStarvation)
                    {
                        getNextTurn();
                    }
                }
            }
        }
    }

    // Changes the Light to Red
    private void ChangeLightRed(int to)
    {
        trafficLights[to].GetComponent<Light_Conteroler>().chagneToRed();
    }

    // Changes the Light to Yellow
    private void ChangeLightYellow(int to)
    {
        trafficLights[to].GetComponent<Light_Conteroler>().chagneToYellow();
    }

    // Changes the Light to green
    private void ChangeLightGreen(int to)
    {

        trafficLights[to].GetComponent<Light_Conteroler>().chagneToGreen();
        cameraController.GetComponent<User_Controll>().updateCameras(to, Intersection);

    }

    private float GreenTimeCalc(int carNo)
    {
        // Just temp simple formula 
        float greenTime = (carNo) + yellowLightDuration + delay; // here is our simple formula so far i need more time to dig and get the proper and suitable one , i substracted the yellow time so it does not count up there
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
    // Resetting the queue and inserting the peak value to direction and its time
    private void ResetQueue()
    {
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

    }
    //Values need to be resetted to its basic value after doing the emergency car process so we ensure everything else back to the normal behaviour 
    private void ResetEmergency()
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

    //This method applies the aging mechanism to prevent starvation 
    private void Aging()
    {
        for (int i = 0; i < 4; i++)
        {
            if (turnWaiting[i] >= 3)
            {
                queue.Clear();
                noStarvation = false;
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
                    if (User_Controll.Intersection == Intersection)
                    {
                        timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
                    }
                }
                turnWaiting[i] = 0;
                break;
            }
            noStarvation = true;
        }
    }

    //this method is supporting method to the aging to count how many turns each direction waiting 
    private void turnWaitingCount()
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
                if (CarCount[i].carsCounter != 0)
                    turnWaiting[i]++;
            }
        }
    }

    //Get the next direction and dequeue the pervious one and queue it again .
    private void getNextTurn()
    {
        oldDirection = queue.Peek().direction;
        queue.Dequeue();
        direction = queue.Peek().direction;
        time[direction] = GreenTimeCalc(CarCount[direction].carsCounter);
        if (time[direction] != 0)
        {
            timeVariable = 0f;
            ChangeLightGreen(direction);
            if (User_Controll.Intersection == Intersection)
            {
                timer.GetComponentInChildren<TMP_Text>().color = new Color(1, 1, 1);
            }
        }
        queue.Enqueue(new NodeClass(oldDirection, CarCount[oldDirection].carsCounter));
    }

    // The timer calculation its UI 
    private void Timer()
    {
        timeVariable += Time.deltaTime;
        if (User_Controll.Intersection == Intersection)
        {
            timer.GetComponentInChildren<TMP_Text>().text = Math.Max((Math.Ceiling(time[direction] - timeVariable)), 0).ToString();
            timer.transform.GetChild(1).GetComponent<Image>().fillAmount = (1 - (timeVariable / time[direction]));
        }
    }
}

