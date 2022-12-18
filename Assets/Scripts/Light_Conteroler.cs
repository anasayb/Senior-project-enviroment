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

        Lights();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {   
        // Update the traffic light singals
        Lights();

    }


    /// <summary>
    /// Method <c>changeToGreen</c> change the light of the traffic light to green.
    /// </summary>
    public void chagneToGreen()
    {
        flag[0] = false;
        flag[2] = true;
        GetComponent<BoxCollider>().enabled = false;
    }


    /// <summary>
    /// Method <c>chagneToRed</c> change the light of the traffic light to red.
    /// </summary>
    public void chagneToRed()
    {
        flag[2] = false;
        flag[0] = true;
        GetComponent<BoxCollider>().enabled = true;
    }


    /// <summary>
    /// Method <c>Lights</c> update the traffic singals light according to the values of the flag array.
    /// </summary>
    private void Lights()
    {

        for (int i = 0; i < standingLights.Length; i++)
        {

            standingLights[i].gameObject.SetActive(flag[i]);
            upLLights[i].gameObject.SetActive(flag[i]);

        }
    }
}
