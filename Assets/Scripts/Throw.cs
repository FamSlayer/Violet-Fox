using UnityEngine;
using System.Collections.Generic;

public class Throw : MonoBehaviour
{

    public enum p_state
    {
        waiting,
        aiming,
        throwing
    }
    public p_state action_state;


    public GameObject held_item_;



    public float default_throw_distance = 20f;
    float throw_distance;

	// Use this for initialization
	void Start ()
    {
        action_state = p_state.waiting;
        throw_distance = default_throw_distance;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            action_state = p_state.aiming;

            // instantiate the object
            holdObjectToThrow("garbage boy");
        }

        if(Input.GetMouseButtonUp(0))
        {
            action_state = p_state.throwing;
        }



        if ( action_state == p_state.aiming )
        {   /*  1. Allow the mouse scrollwheel to be used to increase/decrease the distance the object will be sent
                2. Display the line that will show the object's movement
                3. Check to see if the player wants to cancel the throw
                    - Do other stuff to put it back into the inventory
                    - Maybe drop it
                    - Most importantly set current_state to p_state.waiting and don't throw anything
            */
            
            //  2. Allow the mouse scrollwheel to be used to increase/ decrease the distance the object will be sent


            //  3. Display the line that will show the object's movement

            
        }

        if ( action_state == p_state.throwing )
        {   /*  1. Take the item out of the player's inventory
                2. Launch the item with the appropriate initial velocity and direction
                3. Set action_state back to waiting
            */
            

            Vector3 launch_velocity = getThrowVelocity(held_item_, throw_distance);
            throwItem( held_item_, launch_velocity );


            held_item_ = null;
            action_state = p_state.waiting;
            throw_distance = default_throw_distance;
            
        }


	}


    void holdObjectToThrow ( string item )
    {   /*  1. Instantiate a new gameobject based on whatever the selected item is
            2. Parent it to the player (so if the player rotates the object rotates too)
        */
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = gameObject.transform.position + gameObject.transform.forward + gameObject.transform.up;
        obj.transform.forward = gameObject.transform.forward;
        obj.transform.parent = gameObject.transform;
        obj.AddComponent<Rigidbody>();
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.detectCollisions = false;
        held_item_ = obj;
    }


    void displayThrowArc ()
    {

    }


    Vector3 getThrowVelocity ( GameObject obj, float dist )
    {   /*  
         *  vy = upward velocity
         *  vx = magnitude of horizontal velocity, like yeah it's 3D but i'm not crazy enough to do those kind of calcs
         *  t_airborne = positive x-intercept of h(t) = h0 + v0*t -1/2*a*t^2 ~ that old thing
         *  dist = t_airborne * vx   OR   vx = dist / t_airborne   OR   t_airborne = vx / dist
         *  
         *  F = ma, so when m = 1 i guess F = a
         *  h(t) = h0 + throw_velocity * t - 4.9 * t * t
         *  h0 = obj.transform.position.y
         *  throw_velocity = solve for this
         *  
         *  time_in_air = [-throw_velocity_y +- sqrt( throw_velocity_y * throw_velocity_y - 4 * h0 * -4.9)] / 2*h0
         *  
         *  
         *  t_in_air * 2 * h0 = -vy + sqrt( vy * vy + 4.9 * 4 * h0)
         *  sqrt( vy*vy + 4.9*4*h0 ) = vy + 2*h0*t_in_air                               <-- isolate square root
         *  vy*vy + 4.9*4*h0 = vy*vy + 2*vy*2*h0*t_in_air + 4*h0*h0*t_in_air*t_in_air
         *  4.9*4*h0 = 4*vy*h0*t_in_air + 4*h0*h0*t_in_air*t_in_air
         *  4.9*h0 = vy*h0*t_in_air + h0*h0*t_in_air*t_in_air
         *  vy*h0*t_in_air = h0*h0*t_in_air*t_in_air - 4.9*h0                           <-- isolate term I'm solving for (vy)
         *  vy = h0*t_in_air - 4.9 / t_in_air                                           <-- divide both sides by h0*t_airborne
         *  
         *  
         *  
         *  
         *  
         *  
         */ 


        float h0 = obj.transform.position.y;




        return gameObject.transform.forward * dist + gameObject.transform.up * dist/8f;
        //return new Vector3(10f,5f,0f);
    }

    void throwItem ( GameObject item, Vector3 init_velocity )
    {
        Rigidbody rb = held_item_.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.detectCollisions = true;
        rb.velocity = init_velocity;


        print(rb);
    }



}
