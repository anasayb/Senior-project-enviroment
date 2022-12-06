using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Conteroler : MonoBehaviour
{
    public Component[] standingLights;
    public Component[] upLLights;

    private bool[] flag = {true, false, false}; 

    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {   
        // Update the traffic light singals
        Lights();

        // If the traffic light is green, disable colide box
        if (flag[2])
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void chagneGreen()
    {
        flag[0] = false;
        flag[2] = true;
    }

    public void chagneRed()
    {
        flag[2] = false;
        flag[0] = true;
    }

    /*
    * Input: NONE
    * Output: void
    * This function update the traffic singals light according to the values of the flag array
    * 
    */
    private void Lights()
    {
        for (int i = 0; i < standingLights.Length; i++)
        {

            standingLights[i].gameObject.SetActive(flag[i]);
            upLLights[i].gameObject.SetActive(flag[i]);

        }
    }
}
