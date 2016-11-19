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
    public LineRenderer throw_arc;

    public float throw_arc_time_step;
    public float line_width;
    public float default_throw_power;
    public float default_throw_angle;
    float throw_power;
    float throw_angle;

	// Use this for initialization
	void Start ()
    {
        action_state = p_state.waiting;
        throw_power = default_throw_power;
        throw_angle = default_throw_angle;
        throw_arc = new LineRenderer();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            holdObjectToThrow("garbage boy");
            throw_power = default_throw_power;
            throw_angle = default_throw_angle;
            action_state = p_state.aiming;
        }

        if(Input.GetMouseButtonUp(0))
        {
            action_state = p_state.throwing;
        }
        

        if ( action_state == p_state.aiming )
        {   //  1. Get the inputs! (ScrollWheel and Mouse Y)
            float mouse_y = Input.GetAxis("Mouse Y");
            float scrollwheel = Input.GetAxis("Mouse ScrollWheel");

            //  2. Use ScrollWheel to increase/decrease the power
            throw_power += scrollwheel;
            Mathf.Clamp(throw_power, 0f, 25f);

            //  3. Use mouse Y to increase/decrease the throw_angle
            throw_angle += mouse_y;
            Mathf.Clamp(throw_angle, -45f, 90f);
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

            //  3. Set action_state back to waiting
            held_item_ = null;
            action_state = p_state.waiting;
        }
        
    }




    /*  Void HoldObjectToThrow
            1. Instantiate a new gameobject based on whatever the selected item is
            2. Make it face the right direction
            3. Parent it to the player (so if the player rotates the object rotates too)
            4. Turn off gravity and stuff
            5. Set the new game object to the player's held_item_ member variable
    */
    void holdObjectToThrow ( string item )
    {   //  1. Instantiate a new gameobject based on whatever the selected item is
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = gameObject.transform.position + gameObject.transform.forward + gameObject.transform.up; // fake position, we'll probably 

        //  2. Make it face the right direction
        updateObjectFacing( obj, default_throw_angle );
        //print("updated object facing with default_throw_angle of: [ " + default_throw_angle.ToString() + " ]");

        //  3. Parent it to the player (so if the player rotates the object rotates too)
        obj.transform.parent = gameObject.transform;

        //  4. Turn off gravity and stuff
        obj.AddComponent<Rigidbody>();
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.detectCollisions = false;

        //  5. Set the new game object to the player's held_item_ member variable
        held_item_ = obj;
    }


    void updateObjectFacing ( GameObject obj, float angle )
    {
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
        print(" [ " + vx.ToString() + ", " + vy.ToString() + ", " + vz.ToString() + " ] ");

        float h = obj.transform.position.y;
        float x = obj.transform.position.x;
        float z = obj.transform.position.z;
        float h_prev = h;
        //float x_prev = x;
        //float z_prev = z;

        float t = 0;
        float dt = throw_arc_time_step;

        //Mesh line = new Mesh();
        int iterations = 0;

        List<Vector3> points = new List<Vector3>();
        while ( h > 0  && iterations < 10 )
        {
            t += dt;
            h = getCurrentHeight(t, obj.transform.position.y, vy);
            float dy = h - h_prev;

            float dx = vx * dt;
            float dz = vz * dt;
            x += dx;
            z += dz;

            points.Add(new Vector3(x, h, z));
            //drawSegment(new Vector3(x, h, z), new Vector3(x - dx, h - dy, z - dz), line);
            // OR
            //drawSegment(new Vector3(x, h, z), new Vector3(x_prev, h_prev, z_prev));

            //x_prev = x;
            //z_prev = z;

            h_prev = h;


            t += dt;
            iterations++;
        }

        Vector3[] positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++) positions[i] = points[i];
        //throw_arc.SetPositions = positions;

        //print(" NOW we made a mesh... ");
        //Graphics.DrawMeshNow(line, obj.transform.position, Quaternion.identity);
        //Destroy(line);
    }

    float getCurrentHeight(float t, float h0, float v0)
    {
        float g = Physics.gravity.y;
        return h0 + v0 * t - 1f / 2f * g * t * t;
    }

    void drawSegment( Vector3 start, Vector3 end, Mesh m)
    {
        int vlen = m.vertexCount;
        int clen = m.colors.Length;
        int tlen = m.triangles.Length;

        //  1. Get vertices for the quad that is gonna be the line
        Vector3[] quad = MakeQuad(start, end, line_width);

        //  2. Add in the new quad
        Vector3[] vs = m.vertices;
        vs = resizeVertices(vs, quad.Length);
        vs[vlen] = quad[0];
        vs[vlen + 1] = quad[1];
        vs[vlen + 2] = quad[2];
        vs[vlen + 3] = quad[3];

        //  3. Add some color in there
        Color[] cs = m.colors;
        cs = resizeColors(cs, quad.Length);
        for (int i = 0; i < quad.Length; i++)
        {
            cs[clen + i] = Color.Lerp(Color.red, Color.green, quad[i].y);
        }
        
        //  4. Add in the new triangles
        int[] ts = m.triangles;
        ts = resizeTriangles(ts, 6);
        ts[tlen] = vlen;
        ts[tlen + 1] = vlen + 1;
        ts[tlen + 2] = vlen + 2;
        ts[tlen + 3] = vlen + 1;
        ts[tlen + 4] = vlen + 3;
        ts[tlen + 5] = vlen + 2;

        //  5. Update the changes back into the mesh
        m.vertices = vs;
        m.colors = cs;
        m.triangles = ts;
        m.RecalculateBounds();

        //print("made a mesh: " + m.ToString());
        //Graphics.DrawMeshNow(line, new Vector3(0, 0, 0), Quaternion.identity);
    }

    Vector3[] MakeQuad(Vector3 s, Vector3 e, float w)
    {
        w = w / 2;
        Vector3[] q = new Vector3[4];

        Vector3 n = Vector3.Cross(s, e);
        Vector3 l = Vector3.Cross(n, e - s);
        l.Normalize();

        q[0] = transform.InverseTransformPoint(s + l * w);
        q[1] = transform.InverseTransformPoint(s + l * -w);
        q[2] = transform.InverseTransformPoint(e + l * w);
        q[3] = transform.InverseTransformPoint(e + l * -w);

        return q;
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
        item.transform.parent = null;
        //print(rb.velocity);
    }


    Vector3 getThrowVelocity(GameObject obj, float power)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        return obj.transform.forward * power / rb.mass;
    }

}
