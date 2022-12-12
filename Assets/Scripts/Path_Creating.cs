using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Path_Creating : MonoBehaviour
{


    private List<Transform> path = new List<Transform>();

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        


    }

    void getObjects()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            path.Add(transform.GetChild(i).transform);
        }
    }

    private void OnDrawGizmosSelected() 
    {
        getObjects();
        Gizmos.color = Color.white;
        for (int i = 1; i < path.Count-1; i++)
        {
                Gizmos.DrawLine(path[i - 1].position, path[i].position);
                Gizmos.DrawWireSphere(path[i].position, 0.3f);
        }
    }
 
}
