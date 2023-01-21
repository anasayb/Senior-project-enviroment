using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;


public class Car_Generator : MonoBehaviour
{
    public GameObject[] CarsPrefabs;
    public GameObject[] Streats;
    public int CarsToGenerate;
    public GameObject template;

    private Transform[] parents;
    private Transform[] TurningPathsLeft;
    private Transform[] TurningPathsRight;
    private int NameCarNumber = 1;
    private int NameTruckNumber = 1;
    private int NameEmergencyNumber = 1;
    private int NameBusNumber = 1;
    //private GameObject selector;

    Vector3[] startPos = {  new Vector3(6.44f, 1, -33.256f), new Vector3(13.21318f, 1, -33.256f),
                            new Vector3(32.31f, 1, 6.62f), new Vector3(32.31f, 1, 13.07f),
                            new Vector3(-6.68f, 1, 35.39f), new Vector3(-13.39f, 1, 35.39f),
                            new Vector3(-33.0f, 1, -6.8f), new Vector3(-33.0f, 1, -13.16f)};

    public void generate()    
    {

        parents = new Transform[4];
        parents[0] = transform.Find("North").transform;
        parents[1] = transform.Find("West").transform;
        parents[2] = transform.Find("South").transform;
        parents[3] = transform.Find("East").transform;


        TurningPathsLeft = new Transform[4];
        TurningPathsLeft[0] = transform.parent.Find("Turning Paths").Find("Left").Find("Turnining Left Path North").transform;
        TurningPathsLeft[1] = transform.parent.Find("Turning Paths").Find("Left").Find("Turnining Left Path West").transform;
        TurningPathsLeft[2] = transform.parent.Find("Turning Paths").Find("Left").Find("Turnining Left Path South").transform;
        TurningPathsLeft[3] = transform.parent.Find("Turning Paths").Find("Left").Find("Turnining Left Path East").transform;

        TurningPathsRight = new Transform[4];
        TurningPathsRight[0] = transform.parent.Find("Turning Paths").Find("Right").Find("Turnining Right Path North").transform;
        TurningPathsRight[1] = transform.parent.Find("Turning Paths").Find("Right").Find("Turnining Right Path West").transform;
        TurningPathsRight[2] = transform.parent.Find("Turning Paths").Find("Right").Find("Turnining Right Path South").transform;
        TurningPathsRight[3] = transform.parent.Find("Turning Paths").Find("Right").Find("Turnining Right Path East").transform;


        if (CarsToGenerate == 22)
        {
            
            for (int i = 0; i < template.transform.childCount; i++)
            {
                GameObject dir = template.transform.GetChild(i).gameObject;
                for (int j = 0; j < dir.transform.childCount; j++)
                {

                    GameObject car = Instantiate(dir.transform.GetChild(j).gameObject, transform.position + dir.transform.GetChild(j).position, Quaternion.Euler(new Vector3(0, 0, 0)));
                    car.transform.parent = parents[i];
                    car.GetComponent<CarController>().pathGourpLeft = TurningPathsLeft[i];
                    car.GetComponent<CarController>().pathGourpRight = TurningPathsRight[i];
                }
            }

            CarsToGenerate = 0;
            return;
        }

        //template.SetActive(false);


        GenerateCar();
    }

    // Update is called once per frame
    void Update()
    {


    }


    private void GenerateCar()
    {

        // Set the seed value
        Random.InitState(1234);

        // Decide the number of cars for each direction
        Dictionary<string, int> nums = new Dictionary<string, int>();
        nums["northCars"] = System.Math.Min(Random.Range(0, CarsToGenerate), 45);
        nums["westCars"]  = System.Math.Min(Random.Range(0, CarsToGenerate - nums["northCars"]), 45);
        nums["southCars"]  = System.Math.Min(Random.Range(0, CarsToGenerate - nums["northCars"] - nums["westCars"]),45);
        nums["eastCars"]  = System.Math.Min(CarsToGenerate - nums["northCars"] - nums["westCars"] - nums["southCars"], 45);

        CarsToGenerate -= (nums["northCars"] + nums["westCars"] + nums["southCars"] + nums["eastCars"]);

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



        GenerateCrsForDirection(nums["northCars"], 0);
        GenerateCrsForDirection(nums["westCars"], 1);
        GenerateCrsForDirection(nums["southCars"], 2);
        GenerateCrsForDirection(nums["eastCars"], 3);

    }


    void GenerateCrsForDirection(int numOfCars, int streat)
    {
        Vector3 frontCarLeft = startPos[streat*2];
        Vector3 frontCarRight = startPos[streat*2+1];
        for (int j = 0; j < numOfCars; j+=2)
        {

            // Random values for the need variables
            int carIndex = Random.Range(0, CarsPrefabs.Length);
            int a = Random.Range(0, 2);  // Random number from 0 to 1
            bool turn = false;
            if (a == 1) // if a is 0 make the bool false
                turn = true;
            else       // if a is not 0, make the bool true
                turn = false;



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

            if (CarsPrefabs[carIndex].tag == "Truck")
            {
                frontCarLeft.y = 2.4f;
            }else if (CarsPrefabs[carIndex].tag == "Bus")
            {
                frontCarLeft.y = 2.3f;
            }
            GameObject newCar = Instantiate(CarsPrefabs[carIndex], transform.position + frontCarLeft, Quaternion.Euler(rot));
            newCar.GetComponent<CarController>().pathGourpLeft = TurningPathsLeft[streat];
            newCar.GetComponent<CarController>().pathGourpRight = TurningPathsRight[streat];
            if (newCar.tag == "Truck")
            {
                newCar.name = "Truck " + NameTruckNumber++;
            }
            else if (newCar.tag == "Emergency")
            {
                newCar.name = "Poilice " + NameEmergencyNumber++;
            }
            else if (newCar.tag == "Bus")
            {
                newCar.name = "Bus " + NameBusNumber++;
            }
            else
            {
                newCar.name = "Car " + NameCarNumber++;
            }
            
            newCar.transform.SetParent(parents[streat], true);
            //newCar.GetComponent<CarController>().sel = selector;
            //newCar.GetComponent<CarController>().carInfo = GameObject.Find("Canvas").transform.Find("CarInfo").gameObject;
            newCar.GetComponent<CarController>().left = turn;


            frontCarLeft -= (newCar.transform.forward * (newCar.GetComponent<BoxCollider>().size.z + 3));

            // check if finsih
            if (j+1 == numOfCars)
            {
                break;
            }

            // Right

            // Random values for the need variables
            carIndex = Random.Range(0, CarsPrefabs.Length-1);
            a = Random.Range(0, 2);  // Random number from 0 to 1
            turn = false;
            if (a == 1) // if a is 0 make the bool false
                turn = true;
            else       // if a is not 0, make the bool true
                turn = false;



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


            if (CarsPrefabs[carIndex].tag == "Truck")
            {
                frontCarLeft.y = 2.4f;
            }
            else if (CarsPrefabs[carIndex].tag == "Bus")
            {
                frontCarLeft.y = 2.3f;
            }
            newCar = Instantiate(CarsPrefabs[carIndex], transform.position + frontCarRight, Quaternion.Euler(rot));
            newCar.GetComponent<CarController>().pathGourpLeft = TurningPathsLeft[streat];
            newCar.GetComponent<CarController>().pathGourpRight = TurningPathsRight[streat];
            newCar.name = "Car " + NameCarNumber++;
            newCar.transform.SetParent(parents[streat], true);
            //newCar.GetComponent<CarController>().sel = selector;
            //newCar.GetComponent<CarController>().carInfo = GameObject.Find("Canvas").transform.Find("CarInfo").gameObject;
            newCar.GetComponent<CarController>().right = turn;
            if (newCar.tag == "Truck")
            {
                newCar.name = "Truck " + NameTruckNumber++;
            }
            else if (newCar.tag == "Emergency")
            {
                newCar.name = "Poilice " + NameEmergencyNumber++;
            }
            else if (newCar.tag == "Bus")
            {
                newCar.name = "Bus " + NameBusNumber++;
            }
            else
            {
                newCar.name = "Car " + NameCarNumber++;
            }

            frontCarRight -= (newCar.transform.forward * (newCar.GetComponent<BoxCollider>().size.z + 3));

        }

        //yield return null;

    }
}
