using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Car_Generator : MonoBehaviour
{
    public GameObject[] CarsPrefabs;
    public GameObject[] Streats;

    private string[] TurningPaths = { "Turnining Path North", "Turnining Path West", "Turnining Path South", "Turnining Path East"};
    private float timeVariable;
    private float lane = 1;
    private int CarNumber = 17;

    // Start is called before the first frame update
    void Start()
    {
       

      

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
        // Random values for the need variables
        int carIndex = Random.Range(0,CarsPrefabs.Length);
        int StreatIndex = Random.Range(0, Streats.Length);
        int a = Random.Range(0, 2);  // Random number from 0 to 1
        bool turnLeft = false;
        if (a == 0) // if a is 0 make the bool false
            turnLeft = false;
        else       // if a is not 0, make the bool true
            turnLeft = true;
       

        Vector3 pos = new Vector3(Streats[StreatIndex].transform.position.x, Streats[StreatIndex].transform.position.y, Streats[StreatIndex].transform.position.z);
        pos -= (Streats[StreatIndex].transform.localScale.x / 2) * Streats[StreatIndex].transform.right;
        pos.y = 1.5f; 
        if (lane == 1)
        {
            pos += (((Streats[StreatIndex].transform.localScale.z / 4) - (0.5f)) * Streats[StreatIndex].transform.forward);
        }
        else
        {
            pos -= (((Streats[StreatIndex].transform.localScale.z / 4) - (0.25f)) * Streats[StreatIndex].transform.forward);
        }
        
        lane *= -1;

        Vector3 rot = new Vector3(0,0,0);
        Vector3 st = Streats[StreatIndex].transform.right;
        if(st.x == 1f)
        {
            rot.y = 90;
        }
        else if(st.x== -1)
        {
            rot.y = -90;
        }
        else if(st.z < 0)
        {
            rot.y = 180;
        }

        GameObject newCar = Instantiate(CarsPrefabs[carIndex], pos, Quaternion.Euler(rot));
        newCar.GetComponent<CarController>().pathGourp = GameObject.Find(TurningPaths[StreatIndex]).transform;
        newCar.name = "Car " + CarNumber++;
        newCar.GetComponent<CarController>().left = turnLeft;


    }
}
