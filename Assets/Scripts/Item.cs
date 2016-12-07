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
    
    public int prev_velocities_kept;
    Vector3[] previous_velocities;
    int index_rofl = 0;



    // publics for sound
    public AudioClip crash_sound_;
    public AudioClip drag_sound_;
    AudioSource audio_src_;

    public float minimum_speed_to_make_sound = 2f;
    public float quiet_vol_impact_threshold = 5f;
    public float middle_vol_impact_threshold = 25f;
    public float loud_vol_impact_threshold = 50f;

    [Range(0, 1)]
    public float quiet_vol = .33f;
    [Range(0, 1)]
    public float middle_vol = .66f;
    [Range(0, 1)]
    public float loud_vol = 1f;


    // privates
    GameObject player;
    private PickUp player_pickup;
    Rigidbody rb;
    GameObject text_obj;
    private float weight_;

    

    void Start ()
    {
        previous_velocities = new Vector3[prev_velocities_kept];
        audio_src_ = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        weight_ = rb.mass;

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_pickup = player.GetComponent<PickUp>();
        p_state = pickup_state.inactive;

        text_offset = new Vector3(0f, 0.5f, 0f);

        GameObject text_obj = new GameObject();
        TextMesh t_mesh = text_obj.GetComponent<TextMesh>();
        t_mesh.characterSize = 1;
        t_mesh.anchor = UnityEngine.TextAnchor.MiddleCenter;
        t_mesh.alignment = UnityEngine.TextAlignment.Center;
        t_mesh.text = "-F to pickup " + name_ + "-";

        text_obj.transform.parent = transform;
        text_obj.transform.localPosition = text_offset;



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

        float impact = speed * weight_;
        print("Item impact = " + impact);

        float play_volume;
        bool sliding = false;
        bool should_play = true;
        if( Mathf.Abs(previous_velocities[0].y) < minimum_speed_to_make_sound && Mathf.Abs(previous_velocities[0].y) > 0.2f)
        {
            play_volume = 0;
            sliding = true;
            should_play = false;
        }
        else if ( impact <= quiet_vol_impact_threshold )
        {
            play_volume = quiet_vol;
            sliding = true;
            should_play = true;
        }
        else if (impact <= middle_vol_impact_threshold)
        {
            play_volume = quiet_vol;
            sliding = false;
            should_play = true;
        }
        else if (impact <= loud_vol_impact_threshold)
        {
            play_volume = middle_vol;
            sliding = false;
            should_play = true;
        }
        else // impact > middle_vol_impact_threshold
        {
            play_volume = loud_vol;
            sliding = false;
            should_play = true;
        }

        if(should_play)
        {
            audio_src_.volume = play_volume;
            if (sliding)
            {
                print("item is sliding");
                //audio_src_.PlayOneShot(drag_sound_);
                AudioSource.PlayClipAtPoint(drag_sound_, transform.position, play_volume);
            }
            else
            {
                //audio_src_.PlayOneShot(crash_sound_);
                AudioSource.PlayClipAtPoint(crash_sound_, transform.position, play_volume);
            }
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
