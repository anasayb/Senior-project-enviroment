using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;

public class Car_Generator : MonoBehaviour
{
    public GameObject[] CarsPrefabs;
    public GameObject[] Streats;
    public int CarsToGenerate = 0;

    private bool[] postion = new bool[8];
    private Transform[] parents;
    private Transform[] TurningPathsLeft;
    private Transform[] TurningPathsRight;
    private float timeVariable;
    private int CarNumber = 20;
    

    // Start is called before the first frame update
    void Start()
    {
        parents = new Transform[4];
        parents[0] = GameObject.Find("North Cars").transform;
        parents[1] = GameObject.Find("West Cars").transform;
        parents[2] = GameObject.Find("South Cars").transform;
        parents[3] = GameObject.Find("East Cars").transform;

        TurningPathsLeft = new Transform[4];
        TurningPathsLeft[0] = GameObject.Find("Turnining Left Path North").transform;
        TurningPathsLeft[1] = GameObject.Find("Turnining Left Path West").transform;
        TurningPathsLeft[2] = GameObject.Find("Turnining Left Path South").transform;
        TurningPathsLeft[3] = GameObject.Find("Turnining Left Path East").transform;

        TurningPathsRight = new Transform[4];
        TurningPathsRight[0] = GameObject.Find("Turnining Right Path North").transform;
        TurningPathsRight[1] = GameObject.Find("Turnining Right Path West").transform;
        TurningPathsRight[2] = GameObject.Find("Turnining Right Path South").transform;
        TurningPathsRight[3] = GameObject.Find("Turnining Right Path East").transform;


    }

    // Update is called once per frame
    void Update()
    {
        timeVariable += Time.deltaTime;
        if (timeVariable >= 5)
        {
            GenerateCar();
            timeVariable= 0;
        }
    }


    private void GenerateCar()
    {

        // No cars to generate
        if (CarsToGenerate == 0)
        {
            return;
        }
        
      
        // Choose the postion of the cars to generate
        int number = 0;
        for (int i = 0; i < postion.Length; i++)
        {
            int temp = Random.Range(0, 2);  // Random number from 0 to 1
            if (temp == 1)
            {
                // if a is 0 make the bool false
                postion[i] = true;
                number++;
            }
            else       // if a is not 0, make the bool true
                postion[i] = false;
        }


        // Generate car
        for (int i = 0; i < postion.Length; i++)
        {
            if (number == 0 || CarsToGenerate == 0)
            {
                break;
            }

            if (postion[i])
            {
                // Random values for the need variables
                int carIndex = Random.Range(0, CarsPrefabs.Length);
                int a = Random.Range(0, 2);  // Random number from 0 to 1
                bool turn = false;
                if (a == 1) // if a is 0 make the bool false
                    turn = true;
                else       // if a is not 0, make the bool true
                    turn = false;


                Vector3 pos = new Vector3(Streats[i/2].transform.position.x, Streats[i / 2].transform.position.y, Streats[i / 2].transform.position.z);
                pos -= (Streats[i / 2].transform.localScale.x / 2) * Streats[i / 2].transform.right;
                pos.y = 1.5f;
                if (i % 2 == 1)
                {
                    pos += (((Streats[i / 2].transform.localScale.z / 4) - (0.5f)) * Streats[i / 2].transform.forward);
                }
                else
                {
                    pos -= (((Streats[i / 2].transform.localScale.z / 4) - (0.25f)) * Streats[i / 2].transform.forward);
                }


                Vector3 rot = new Vector3(0, 0, 0);
                Vector3 st = Streats[i / 2].transform.right;
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

                GameObject newCar = Instantiate(CarsPrefabs[carIndex], pos, Quaternion.Euler(rot));
                newCar.GetComponent<CarController>().pathGourpLeft = TurningPathsLeft[i / 2];
                newCar.GetComponent<CarController>().pathGourpRight = TurningPathsRight[i / 2];
                newCar.name = "Car " + CarNumber++;
                newCar.transform.SetParent(parents[i / 2], true);
                if (i % 2 == 1)
                {
                    newCar.GetComponent<CarController>().left = turn;
                }
                else
                {
                    newCar.GetComponent<CarController>().right = turn;
                }

                number--;
                CarsToGenerate--;
            }
        }
        


    }
}
