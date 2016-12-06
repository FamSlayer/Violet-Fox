using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

    public float speed = 20.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float jump_speed = 20;
    [Range(0,1)]
    public float camera_slow_when_firing = 0.5f;
    //private Vector3 moveDirection = Vector3.zero;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private Rigidbody rb;

    //CharacterController controller;
	// Use this for initialization
	void Start () {
	    Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        float s_input = Input.GetAxis("Horizontal");    // side movement
        float f_input = Input.GetAxis("Vertical");      // forward movement

        if (s_input != 0 || f_input != 0)   // there is input
        {
            //print("input is given: -   forward: " + f_input.ToString() + "  side: " + s_input.ToString());

            Vector3 f_movement = new Vector3(0, 0, 0);
            Vector3 s_movement = new Vector3(0, 0, 0);

            // get the xz facing components of the character
            Vector3 facing = transform.forward;
            facing.y = 0;
            facing = facing.normalized;
            //print("facing: " + facing.ToString());

            f_movement = facing * f_input;

            Vector3 side = Vector3.Cross(facing, new Vector3(0, 1, 0));
            side = side.normalized;
            s_movement = -side * s_input;
            //print("s_movement: " + s_movement);

            Vector3 resulting_movement = f_movement + s_movement;
            resulting_movement = resulting_movement.normalized;
            //print("resulting_movement: " + resulting_movement);

            resulting_movement *= speed;// * Time.deltaTime;

            //print(transform.position);
            //print("resulting_movement: " + resulting_movement);


            //transform.position = transform.position + resulting_movement;
            //rb.velocity = new Vector3(rb.velocity.x + resulting_movement.x, rb.velocity.y, rb.velocity.z + resulting_movement.z);
            rb.velocity = new Vector3(resulting_movement.x, rb.velocity.y, resulting_movement.z);

            //print(transform.position);


        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }


        //Input.getKey
        /*
        if ( Input.GetKeyDown(KeyCode.Space))
        {
            print(rb.velocity);
            rb.velocity = new Vector3(rb.velocity.x, jump_speed, rb.velocity.z);
        }*/


        if (!Input.GetMouseButton(0))
        { 
            pitch -= speedV * Input.GetAxis("Mouse Y");
            yaw += speedH * Input.GetAxis("Mouse X");
        }
        else
        {
            pitch -= speedV * Input.GetAxis("Mouse Y") * camera_slow_when_firing;
            yaw += speedH * Input.GetAxis("Mouse X") * camera_slow_when_firing;
        }
            

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);




        /*
        //Camera mycam = GetComponent<Camera>();
        moveDirection = new Vector3(0, 0, 0);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        print("x: " + x.ToString() + "   z: " + z.ToString());
        moveDirection.x = x;
        moveDirection.z = z;

        
        if (controller.isGrounded)
        {
            print("controller is grounded");
            moveDirection.x = Input.GetAxis("Horizontal");
            moveDirection.z = Input.GetAxis("Vertical");

            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        print(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);

        //yaw += speedH * Input.GetAxis("Mouse X"); 
        //pitch -= speedV * Input.GetAxis("Mouse Y"); 

        //transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        //transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);


        */
    }
}