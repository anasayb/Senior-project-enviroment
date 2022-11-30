using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Conteroler : MonoBehaviour
{
    public Component[] standingLights;
    public Component[] upLLights;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < standingLights.Length; i++)
        {
            if (standingLights[i].name == "Red")
            {
                standingLights[i].gameObject.SetActive(true);
            }
            else
            {
                standingLights[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < upLLights.Length; i++)
        {
            if (upLLights[i].name == "Red")
            {
                upLLights[i].gameObject.SetActive(true);
            }
            else
            {
                upLLights[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
