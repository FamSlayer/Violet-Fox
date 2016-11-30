using UnityEngine;
using System.Collections;

public class Patrol : MonoBehaviour {

    public GameObject goal;

    NavMeshAgent agent;
    Vector3 origin;


    void Start()
    {
        origin = transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.transform.position;
    }
    


    void Update () {
        //print(Vector3.Distance(transform.position, goal.transform.position));
	    if ( Vector3.Distance(transform.position, goal.transform.position) < 1.25f )
        {
            agent.destination = origin;
        }

        else if ( Vector3.Distance(transform.position, origin) < 1.25f )
        {
            agent.destination = goal.transform.position;
        }
    }
}
