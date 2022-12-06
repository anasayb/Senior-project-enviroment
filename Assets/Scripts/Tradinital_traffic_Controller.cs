using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tradinital_traffic_Controller : MonoBehaviour
{
    public Component[] trafficLights;
    public int time;


    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
 
            ChangeLight(1);
            float temp = time;
            do{
                temp -= Time.deltaTime;
 
            }while (temp > 0);
            ChangeLight(2);
    }

    private void FixedUpdate()
    {
       


    }

    /*
    * Input: NONE
    * Output: void
    * This function update the traffic singals light according to the values of the flag array
    * 
    */
    private void ChangeLight(int to)
    {

        if (to-2 < 0)
        {
            trafficLights[3].GetComponent<Light_Conteroler>().chagneRed();
        }
        else
        {
            trafficLights[to - 2].GetComponent<Light_Conteroler>().chagneRed();
        }
       

        trafficLights[to - 1].GetComponent<Light_Conteroler>().chagneGreen();
     
    }
}
