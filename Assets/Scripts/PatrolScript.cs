using UnityEngine;
using System.Collections;

public class PatrolScript : MonoBehaviour {

    public Transform[] points;
    public GameObject player;
    public int visionDistance;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private Transform investipoint;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        if(investipoint)
        {
            agent.destination = investipoint.position;
        }

        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    public void Investigate(Transform duck)
    {
        investipoint = duck;
        GotoNextPoint();
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (agent.remainingDistance < 1.5f)
            if(investipoint)
            {
                investipoint = null;
                GotoNextPoint();
            }
            GotoNextPoint();
    }
    private void FixedUpdate()
    {
        Vector3 fromPosition = this.transform.position;
        Vector3 toPosition = player.transform.position;
        Vector3 direction = toPosition - fromPosition;
        //Vector3 direction = transform.TransformDirection(Vector3.forward);
        float angle = Vector3.Angle(transform.forward, player.transform.position);
        if (angle > 60)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit, visionDistance);
            if(hit.collider.tag == "Player")
            {
                //Idk do game over stuff
                print("You got caught asshole");
            }
        }
    }
}
