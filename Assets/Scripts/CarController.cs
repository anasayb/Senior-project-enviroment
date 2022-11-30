using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public WheelCollider[] wheels;
    public float MAX_SPEED = 40f;

    [Header("Sensors")]
    public float sensorLength = 25f;
    private bool colide = false;

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

        checkColide();

        if (!colide) {
            
            Rigidbody speed = GetComponent<Rigidbody>();
            if (speed.velocity.magnitude > MAX_SPEED)
            {
                return;
            }
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = MAX_SPEED;
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = 500;
            }
        }

    }

    private void checkColide()
    {

        RaycastHit hit;
        Vector3 pos = transform.position;
        Vector3 test = new Vector3(0f, 0f, 1f);
        if (Physics.Raycast(pos, test, out hit, sensorLength))
        {
            print("hello its me the freaking "+transform.gameObject.name);
            colide = true;
            return;
        }

        //Debug Code
        //Debug.DrawRay(transform.position, test*10, Color.red);

        colide = false;

    }
}
