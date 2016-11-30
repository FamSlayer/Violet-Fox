using UnityEngine;
using System.Collections;

public class MoveScrip : MonoBehaviour {

    public float speed = 6.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    private float pitch = 0.0f;
    private float yaw = 0.0f;
	// Use this for initialization
	void Start () {
	    Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        CharacterController controller = GetComponent<CharacterController>();
        Camera mycam = GetComponent<Camera>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        yaw += speedH * Input.GetAxis("Mouse X"); 
        pitch -= speedV * Input.GetAxis("Mouse Y"); 

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        //transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);
    }
}