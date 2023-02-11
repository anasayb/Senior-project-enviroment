using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
using UnityEngine.UIElements;


public class Car_Generator_two : MonoBehaviour
{
    public GameObject[] CarsPrefabs;
    public GameObject[] Streats;
    public int CarsToGenerate;
    public GameObject CarsTemplate;
    public GameObject selector;
    public GameObject Intersection;
    public int seed;

    private Transform[] parents;
    private Transform[] TurningPathsLeft;
    private Transform[] TurningPathsRight;
    private static int num = 0;
    private static int NameCarNumber = 1;
    private static int NameTruckNumber = 1;
    private static int NameEmergencyNumber = 1;
    private static int NameBusNumber = 1; 
    private bool emergency = false;

    Vector3[] startPos = {  new Vector3(6.97998f, 1, -33.25642f), new Vector3(13.35f, 1, -33.25642f),
                            new Vector3(32.31f, 1, 6.62f), new Vector3(32.31f, 1, 13.07f),
                            new Vector3(-6.68f, 1, 33.73298f), new Vector3(-13.4f, 1, 33.90339f),
                            new Vector3(-33.0f, 1, -6.8f), new Vector3(-33.0f, 1, -13.16f)};

    // Start is called before the first frame update
    void Awake()    
    {

        // selector
        emergency = Scence_Manger.EmergencyCar;

        // Min the number if it exceed the max possible car generation
        CarsToGenerate = System.Math.Min(Scence_Manger.startingNumberOfCars, 45 * 4);

        // Inizilie varibels
        parents = new Transform[4];
        parents[0] = transform.Find("North").transform;
        parents[1] = transform.Find("West").transform;
        parents[2] = transform.Find("South").transform;
        parents[3] = transform.Find("East").transform;

        TurningPathsLeft = new Transform[4];
        TurningPathsLeft[0] = Intersection.transform.Find("Turning Paths").Find("Left").Find("Turnining Left Path North").transform;
        TurningPathsLeft[1] = Intersection.transform.Find("Turning Paths").Find("Left").Find("Turnining Left Path West").transform;
        TurningPathsLeft[2] = Intersection.transform.Find("Turning Paths").Find("Left").Find("Turnining Left Path South").transform;
        TurningPathsLeft[3] = Intersection.transform.Find("Turning Paths").Find("Left").Find("Turnining Left Path East").transform;

        TurningPathsRight = new Transform[4];
        TurningPathsRight[0] = Intersection.transform.Find("Turning Paths").Find("Right").Find("Turnining Right Path North").transform;
        TurningPathsRight[1] = Intersection.transform.Find("Turning Paths").Find("Right").Find("Turnining Right Path West").transform;
        TurningPathsRight[2] = Intersection.transform.Find("Turning Paths").Find("Right").Find("Turnining Right Path South").transform;
        TurningPathsRight[3] = Intersection.transform.Find("Turning Paths").Find("Right").Find("Turnining Right Path East").transform;

        // generate cars
        GenerateCar();

    }


    private void GenerateCar()
    {

        // Set the seed value
        Random.InitState(seed);

        // Decide the number of cars for each direction
        Dictionary<string, int> nums = new Dictionary<string, int>();
        nums["northCars"] =  System.Math.Min(Random.Range(0, CarsToGenerate), 45);
        nums["westCars"] = System.Math.Min(Random.Range(0, CarsToGenerate - nums["northCars"]), 45);
        nums["southCars"] = System.Math.Min(Random.Range(0, CarsToGenerate - nums["northCars"] - nums["westCars"]),45);
        nums["eastCars"] =  System.Math.Min(CarsToGenerate - nums["northCars"] - nums["westCars"] - nums["southCars"], 45);

        CarsToGenerate -= (nums["northCars"] + nums["westCars"] + nums["southCars"] + nums["eastCars"]);

        // If the sumation of cars in each direction still hasen't reached teh "CarsToGenerate" value, add cars to teh lowest direction

        if (CarsToGenerate != 0)
        {
            Dictionary<string, int> sorted = nums.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<string, int> item in sorted)
            {
                int temp = System.Math.Min(45 - nums[item.Key], CarsToGenerate);
                nums[item.Key] += temp;
                CarsToGenerate -= temp;
            }

        }


        // Generate cars for each direction
        GenerateCrsForDirection(nums["northCars"], 0, selector);
        GenerateCrsForDirection(nums["westCars"], 1, selector);
        GenerateCrsForDirection(nums["southCars"], 2, selector);
        GenerateCrsForDirection(nums["eastCars"], 3, selector);


        // Reset naming variables
        Car_Generator_two.num++;
        if (Car_Generator_two.num == 2)
        {
            Car_Generator_two.NameCarNumber = 1;
            Car_Generator_two.NameTruckNumber = 1;
            Car_Generator_two.NameEmergencyNumber = 1;
            Car_Generator_two.NameBusNumber = 1;
            Car_Generator_two.num = 0;
        }




    }

    /// <summary>
    /// Method <c>GenerateCrsForDirection</c> Generate the cars for the specified direction.
    /// </summary>
    /// <param name="numOfCars"> the number of cars to be generated</param>
    /// <param name="streat"> the direction on whcih cars are generated </param>
    /// <param name="selector"> the object which get activate when the car is selected </param>
    void GenerateCrsForDirection(int numOfCars, int streat, GameObject selector)
    {
        // starting postion, right, and left
        Vector3 frontCarLeft = startPos[streat*2];
        Vector3 frontCarRight = startPos[streat*2+1];

        // Generate two cars at a time, on lef, one right
        for (int j = 0; j < numOfCars; j+=2)
        {

            // Random values for the need variables
            int carIndex = Random.Range(0, CarsPrefabs.Length);

            // if emrgency car is check, if the selcted car is emergency make false and generate the car, else look for a car that is not emergency
            if (emergency == true)
            {
                if (CarsPrefabs[carIndex].tag == "Emergency")
                {
                    emergency = false;
                }
                else if (j+1 == numOfCars)
                {

                    carIndex = CarsPrefabs.Length-2;
                    emergency = false;

                }

                
            }
            else
            {
                while (CarsPrefabs[carIndex].tag == "Emergency")
                {
                    carIndex = Random.Range(0, CarsPrefabs.Length);
                }
            }

            // Decide if the car turn or not
            int a = Random.Range(0, 2);  // Random number from 0 to 1
            bool turn = false;
            if (a == 1) // if a is 0 make the bool false
                turn = true;
            else       // if a is not 0, make the bool true
                turn = false;

            int b = Random.Range(0, 2);  // Random number from 0 to 1
            bool turn2 = false;
            if (b == 1) // if a is 0 make the bool false
                turn2 = true;
            else       // if a is not 0, make the bool true
                turn2 = false;


            // Rotation of the car
            Vector3 rot = new Vector3(0, 0, 0);
            Vector3 st = Streats[streat].transform.right;
            if (st.x == 1f)
            {
                rot.y = 90;
            }
            else if (st.x == -1)
            {
                rot.y = -90;
            }
            else if (st.z < 0)
            {
                rot.y = 180;
            }

            // Set the postion of the car to half the prefab colide box
            frontCarLeft.y = CarsPrefabs[carIndex].GetComponent<BoxCollider>().size.y / 2 + 0.5f;

            // Generate the car, and name it
            GameObject newCar = Instantiate(CarsPrefabs[carIndex], transform.localPosition + frontCarLeft, Quaternion.Euler(rot));
            newCar.GetComponent<CarControllerTwo>().enabled = true;
            newCar.GetComponent<CarControllerTwo>().pathGourpLeft = TurningPathsLeft[streat];
            newCar.GetComponent<CarControllerTwo>().pathGourpRight = TurningPathsRight[streat];
            if (newCar.tag == "Truck")
            {
                newCar.name = "Truck " + NameTruckNumber++;

            }
            else if (newCar.tag == "Emergency")
            {
                newCar.name = "Police " + NameEmergencyNumber++;
            }
            else if (newCar.tag == "Bus")
            {
                newCar.name = "Bus " + NameBusNumber++;
            }
            else
            {
                newCar.name = "Car " + NameCarNumber++ ;
            }

            // Add the necesarry object
            newCar.transform.SetParent(parents[streat], true);
            newCar.GetComponent<CarControllerTwo>().sel = selector;
            newCar.GetComponent<CarControllerTwo>().carInfo = GameObject.Find("Canvas").transform.Find("CarInfo").gameObject;
            newCar.GetComponent<CarControllerTwo>().left[0] = turn;
            newCar.GetComponent<CarControllerTwo>().left[1] = turn;
            newCar.GetComponent<CarControllerTwo>().CurrentIntersection = Intersection.name[Intersection.name.Length-1] - '0';
            newCar.GetComponent<CarControllerTwo>().CurrentDirection = streat;

            // Increase the postion of the next car by the length of the current car
            frontCarLeft -= (newCar.transform.forward * (newCar.GetComponent<BoxCollider>().size.z + 3));

            // check if finsih
            if (j+1 == numOfCars)
            {
                break;
            }

            // Right part

            // Random values for the need variables
            carIndex = Random.Range(0, CarsPrefabs.Length-1);

            // if emrgency car is check, if the selcted car is emergency make false and generate the car, else look for a car that is not emergency

            if (emergency == true)
            {
                if (CarsPrefabs[carIndex].tag == "Emergency")
                {
                    emergency = false;

                }
                else if (j + 1 == numOfCars)
                {

                    carIndex = CarsPrefabs.Length - 2;
                    emergency = false;

                }


            }
            else
            {
                while (CarsPrefabs[carIndex].tag == "Emergency")
                {
                    carIndex = Random.Range(0, CarsPrefabs.Length-1);
                }
            }

            // Decide if the car turn or not
            a = Random.Range(0, 2);  // Random number from 0 to 1
            turn = false;
            if (a == 1) // if a is 0 make the bool false
                turn = true;
            else       // if a is not 0, make the bool true
                turn = false;

            b = Random.Range(0, 2);  // Random number from 0 to 1
            turn2 = false;
            if (b == 1) // if a is 0 make the bool false
                turn2 = true;
            else       // if a is not 0, make the bool true
                turn2 = false;

            // Rotation of the car
            rot = new Vector3(0, 0, 0);
            st = Streats[streat].transform.right;
            if (st.x == 1f)
            {
                rot.y = 90;
            }
            else if (st.x == -1)
            {
                rot.y = -90;
            }
            else if (st.z < 0)
            {
                rot.y = 180;
            }

            // Set the postion of the car to half the prefab colide box
            frontCarRight.y = CarsPrefabs[carIndex].GetComponent<BoxCollider>().size.y / 2 + 0.5f;

            // Generate the car, and name it
            newCar = Instantiate(CarsPrefabs[carIndex], transform.position + frontCarRight, Quaternion.Euler(rot));
            newCar.GetComponent<CarControllerTwo>().enabled = true;
            newCar.GetComponent<CarControllerTwo>().pathGourpLeft = TurningPathsLeft[streat];
            newCar.GetComponent<CarControllerTwo>().pathGourpRight = TurningPathsRight[streat];
            newCar.transform.SetParent(parents[streat], true);
            newCar.GetComponent<CarControllerTwo>().sel = selector;
            newCar.GetComponent<CarControllerTwo>().carInfo = GameObject.Find("Canvas").transform.Find("CarInfo").gameObject;
            newCar.GetComponent<CarControllerTwo>().right[0] = turn;
            newCar.GetComponent<CarControllerTwo>().right[1] = turn;
            newCar.GetComponent<CarControllerTwo>().CurrentIntersection = Intersection.name[Intersection.name.Length - 1] - '0';
            newCar.GetComponent<CarControllerTwo>().CurrentDirection = streat;
            if (newCar.tag == "Truck")
            {
                newCar.name = "Truck " + NameTruckNumber++;
            }
            else if (newCar.tag == "Emergency")
            {
                newCar.name = "Police " + NameEmergencyNumber++;
            }
            else if (newCar.tag == "Bus")
            {
                newCar.name = "Bus " + NameBusNumber++;
            }
            else
            {
                newCar.name = "Car " + NameCarNumber++;
            }

            // Increase the postion of the next car by the length of the current car
            frontCarRight -= (newCar.transform.forward * (newCar.GetComponent<BoxCollider>().size.z + 3));

           

        }

        
        //yield return null;
    }
}
