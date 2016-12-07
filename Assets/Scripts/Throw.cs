using UnityEngine;
using System.Collections.Generic;

public class Throw : MonoBehaviour
{

    enum p_state
    {
        waiting,
        aiming,
        throwing
    }
    p_state action_state;
    
    public GameObject held_item_;
    
    public float throw_arc_time_step;
    public float line_width;
    [Range (5,20)]
    public float default_throw_power;
    [Range(0, 5)]
    public float max_random_spin;

    float throw_power;
    float throw_angle;

    Vector3 offset;// = gameObject.transform.position + gameObject.transform.forward + gameObject.transform.right / 2f + gameObject.transform.up / 4f; // fake position, we'll probably 

    LineRenderer throw_arc;

    GameObject player;
    PickUp player_pickup;

    UIWheelController ui_wheel;

    int item_index = 0;

    // Use this for initialization
    void Start ()
    {
        action_state = p_state.waiting;
        throw_power = default_throw_power;
        throw_angle = Vector3.Angle(transform.forward, new Vector3(transform.forward.x, 0, transform.forward.z));
        throw_arc = gameObject.AddComponent<LineRenderer>();
        throw_arc.SetWidth(0f, .1f);
        throw_arc.enabled = false;

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_pickup = player.GetComponent<PickUp>();

        GameObject wheel = GameObject.FindGameObjectsWithTag("UI")[0];
        ui_wheel = wheel.GetComponent<UIWheelController>();

        
    }
	
	// Update is called once per frame
	void Update ()
    {
        offset = gameObject.transform.position + gameObject.transform.forward + gameObject.transform.right / 2f + gameObject.transform.up / 4f; // fake position, we'll change this ???
        //print(player_pickup.Inventory_items.Count);
        if (Input.GetMouseButtonDown(1) && player_pickup.Inventory_items.Count > 0)
        {
            //print("ok trying to hold an item now...");
            //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            print("in Throw.cs: item_index = " + item_index);
            if ( item_index >= player_pickup.Inventory_items.Count )
            {
                item_index -= player_pickup.Inventory_items.Count;
            }
            player_pickup.Inventory_items[item_index].SetActive(true);
            holdObjectToThrow(player_pickup.Inventory_items[item_index]);
            throw_power = default_throw_power;
            throw_angle = Vector3.Angle(transform.forward, new Vector3(transform.forward.x, 0, transform.forward.z));
            if (transform.forward.y < 0)
                throw_angle *= -1;
            action_state = p_state.aiming;
        }

        if(Input.GetMouseButtonUp(1))
        {
            action_state = p_state.throwing;
        }
        
        

        if ( action_state == p_state.aiming )
        {
            if (!throw_arc.isVisible)
                throw_arc.enabled = true;

            //  1. Get the inputs! (ScrollWheel and Mouse Y)
            float mouse_y = Input.GetAxis("Mouse Y");
            float scrollwheel = Input.GetAxis("Mouse ScrollWheel");

            //  2. Use ScrollWheel to increase/decrease the power
            throw_power += scrollwheel*2f;
            throw_power = Mathf.Clamp(throw_power, 3f, 30f);
            
            //  3. Use mouse Y to increase/decrease the throw_angle
            throw_angle += mouse_y;
            throw_angle = Mathf.Clamp(throw_angle, -45f, 90f);
            updateObjectFacing( held_item_, throw_angle );

            //  4. Display the line that will show the object's movement
            displayThrowArc(held_item_, throw_power);

            //  5. Check to see if the player wants/tries to cancel the throw
        }

        if ( action_state == p_state.throwing )
        {   //  1. Take the item out of the player's inventory
            
            //  2. Launch the item with the appropriate initial velocity and direction
            Vector3 launch_velocity = getThrowVelocity( held_item_, throw_power );
            throwItem( held_item_, launch_velocity );

            //  3. Remove the object from the player inventory
            item_index = player_pickup.Inventory_items.IndexOf(held_item_);
            player_pickup.removeFromInventory(held_item_);

            //  4. Call update on the UI
            //ui_wheel.updateUIAfterThrow();

            //  4. Set action_state back to waiting
            held_item_ = null;
            action_state = p_state.waiting;
            throw_arc.enabled = false;
        }
        
    }




    /*  Void HoldObjectToThrow
            1. Instantiate a new gameobject based on whatever the selected item is
            2. Make it face the right direction
            3. Parent it to the player (so if the player rotates the object rotates too)
            4. Turn off gravity and stuff
            5. Set the new game object to the player's held_item_ member variable
    */
    void holdObjectToThrow ( GameObject obj )
    {   //  1. Move the GameObject obj to the correct position
        obj.transform.position = offset;

        //  2. Make it face the right direction
        updateObjectFacing( obj, Vector3.Angle(transform.forward, new Vector3(transform.forward.x, 0, transform.forward.z)));
        //print("updated object facing with default_throw_angle of: [ " + default_throw_angle.ToString() + " ]");

        //  3. Parent it to the player (so if the player rotates the object rotates too)
        obj.transform.parent = gameObject.transform;

        //  4. Turn off gravity and stuff
        //obj.AddComponent<Rigidbody>();
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.detectCollisions = false;

        obj.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //  5. Set the new game object to the player's held_item_ member variable
        held_item_ = obj;
    }


    void updateObjectFacing ( GameObject obj, float angle )
    {
        obj.transform.position = offset;
        float forward_x = gameObject.transform.forward.x * Mathf.Cos(angle * Mathf.Deg2Rad);
        float forward_z = gameObject.transform.forward.z * Mathf.Cos(angle * Mathf.Deg2Rad);
        float forward_y = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 forward = new Vector3(forward_x, forward_y, forward_z);

        obj.transform.forward = Vector3.Normalize(forward);
    }

    void displayThrowArc ( GameObject obj, float power )
    {
        Vector3 vel = obj.transform.forward * power / obj.GetComponent<Rigidbody>().mass;
        float vx = vel.x;
        float vy = vel.y;
        float vz = vel.z;

        float h = obj.transform.position.y;
        float x = obj.transform.position.x;
        float z = obj.transform.position.z;
        //float h_prev = h;

        float t = 0;
        float dt = throw_arc_time_step;

        List<Vector3> points = new List<Vector3>();
        points.Add(obj.transform.position);
        while ( h > 0 )
        {
            t += dt;
            h = getCurrentHeight(t, obj.transform.position.y, vy);
            x += vx * dt;
            z += vz * dt;
            points.Add(new Vector3(x, h, z));

            //h_prev = h;
            //print("H_pev: " + h_prev.ToString() + "\tH now: " + h.ToString());
        }

        // convert the List<Vector3> into an Vector3[] array
        Vector3[] positions = new Vector3[points.Count];

        /*  ok well the difference between my function to convert it to an array and the .ToArray() function 
         *      is that 
         */

        //for (int i = 0; i < points.Count; i++) positions[i] = points[i];
        positions = points.ToArray();


        throw_arc.SetVertexCount(positions.Length);
        throw_arc.SetPositions(positions);

    }

    float getCurrentHeight(float t, float h0, float v0)
    {
        float g = Physics.gravity.y;
        return h0 + v0 * t + 1f / 2f * g * t * t;
    }
    
    Color[] resizeColors(Color[] c, int ds)
    {
        Color[] new_c = new Color[c.Length + ds];
        for (int i = 0; i < c.Length; i++) new_c[i] = c[i];
        return new_c;
    }

    Vector3[] resizeVertices(Vector3[] v, int ds)
    {
        Vector3[] new_v = new Vector3[v.Length + ds];
        for (int i = 0; i < v.Length; i++) new_v[i] = v[i];
        return new_v;
    }

    int[] resizeTriangles(int[] t, int ds)
    {
        int[] new_t = new int[t.Length + ds];
        for (int i = 0; i < t.Length; i++) new_t[i] = t[i];
        return new_t;
    }

    void throwItem ( GameObject item, Vector3 init_velocity )
    {
        Rigidbody rb = held_item_.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.detectCollisions = true;
        rb.velocity = init_velocity;
        float max = max_random_spin;
        Vector3 torque = new Vector3(Random.Range(-max, max), Random.Range(-max, max), Random.Range(-max, max));
        rb.AddTorque(torque * init_velocity.magnitude, ForceMode.Impulse);
        item.transform.parent = null;
        //print(rb.velocity);

        
    }


    Vector3 getThrowVelocity(GameObject obj, float power)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        return obj.transform.forward * power / rb.mass;
    }


    // this is the function called by the UI to change the held item
    public void changeObjectHolding ( GameObject obj )
    {
        if(action_state == p_state.aiming)
        {
            if (held_item_ != null)
            {
                held_item_.SetActive(false);
                held_item_.transform.parent = null;
            }

            if (obj.activeSelf)
            {
                print("the object was already active...");
            }
            else
            {
                obj.SetActive(true);
            }

            holdObjectToThrow(obj);
        }
        else
        {
            item_index = player_pickup.Inventory_items.IndexOf(obj);
        }
        
    }

}
