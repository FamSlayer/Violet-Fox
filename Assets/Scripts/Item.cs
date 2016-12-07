using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    enum pickup_state
    {
        active,
        inactive,
    };

    pickup_state p_state;

    public string name_;
    public Vector3 text_offset;
    public float default_sound_volume_;
    public AudioClip sound_;

    AudioSource audio_src_;

    public int prev_velocities_kept;
    Vector3[] previous_velocities;
    int index_rofl = 0;

    private PickUp player_pickup;

    Rigidbody rb;
    GameObject player;
    GameObject text_obj;
    private float weight_;

    /* so let's say 0.5 is our average volume for EVERYTHING in the game 
     * should objects going above 0.5 fall off with a logithmic approach towards 1?
     * 
     * 
     * 
     */

    void Start ()
    {
        previous_velocities = new Vector3[prev_velocities_kept];
        audio_src_ = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        weight_ = rb.mass;

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_pickup = player.GetComponent<PickUp>();
        p_state = pickup_state.inactive;

        text_obj = transform.GetChild(0).gameObject;
        TextMesh t_mesh = text_obj.GetComponent<TextMesh>();
        t_mesh.text = "-F to pickup " + name_ + "-";



    }

    void Update()
    {
        // store the current velocity at the end of the array and bump it
        Vector3 vel = rb.velocity;
        
        text_obj.transform.position = transform.position + text_offset;
        text_obj.transform.rotation = Quaternion.identity;

        if ( index_rofl >= prev_velocities_kept )
        {   //print("EXECUTING COPY!");
            System.Array.Copy(previous_velocities, 0, previous_velocities, 1, prev_velocities_kept - 1);
            previous_velocities[0] = vel;
        }
        else
        {
            previous_velocities[prev_velocities_kept - index_rofl - 1] = vel;
            index_rofl++;
        }
        

        if ( p_state == pickup_state.active )
        {
            text_obj.SetActive(true);
            text_obj.transform.rotation = Quaternion.LookRotation(text_obj.transform.position - player.transform.position);
            
            if(Input.GetKeyDown(KeyCode.F))
            {
                print(" ay pick it up lmao ");
                player_pickup.addToInventory(gameObject, name_);
                //gameObject.SetActive(false);
            }
            
        }

        else if ( p_state == pickup_state.inactive )
        {
            text_obj.SetActive(false);
        }

    }

    void OnCollisionEnter( Collision collision )
    {
        float speed = previous_velocities[0].magnitude;
        if( speed > 2)
        {
            audio_src_.volume = default_sound_volume_ * speed * weight_;
            audio_src_.PlayOneShot(sound_);
            AudioSource.PlayClipAtPoint(sound_, transform.position, default_sound_volume_ * speed * weight_);
        }
        


    }


    void OnTriggerEnter( Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //print("TriggerEnter");
            // display text if the player is looking at the GameObject

            // 
            //print(player_pickup);
            //player_pickup.addToInventory(gameObject, name_);
            p_state = pickup_state.active;
        }
        
    }

    void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //print("TriggerEXIT");
            // display text if the player is looking at the GameObject

            // 
            //print(player_pickup);
            //player_pickup.addToInventory(gameObject, name_);

            p_state = pickup_state.inactive;
        }
        // disable the text
    }


}
