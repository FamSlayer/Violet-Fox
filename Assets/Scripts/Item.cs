using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public AnimationCurve curve_;
    public string name_;
    public float weight_;
    public float default_sound_volume_;

    public AudioClip sound_;

    AudioSource audio_src_;

    public int prev_velocities_kept;
    float[] previous_velocities;

    int index_rofl = 0;

    /* so let's say 0.5 is our average volume for EVERYTHING in the game 
     * should objects going above 0.5 fall off with a logithmic approach towards 1?
     * 
     * 
     * 
     */ 

    void Start ()
    {
        previous_velocities = new float[prev_velocities_kept];
        audio_src_ = GetComponent<AudioSource>();
        weight_ = GetComponent<Rigidbody>().mass;
    }

    void Update()
    {
        // store the current velocity at the end of the array and bump it
        float mag = GetComponent<Rigidbody>().velocity.magnitude;

        /*
        string output = "Before: [ ";
        for(int i=0; i<index_rofl; i++)
        {
            output += previous_velocities[i].ToString() + ", ";
        }
        output += "]";
        print(output);
        print("\tnew mag: " + mag.ToString());
        */
        
        if ( index_rofl >= prev_velocities_kept )
        {   //print("EXECUTING COPY!");
            System.Array.Copy(previous_velocities, 0, previous_velocities, 1, prev_velocities_kept - 1);
            previous_velocities[0] = mag;
        }
        else
        {
            previous_velocities[prev_velocities_kept - index_rofl - 1] = mag;
            index_rofl++;
        }

        /*
        output = "After: [ ";
        for (int i = 0; i < index_rofl; i++)
        {
            output += previous_velocities[i].ToString() + ", ";
        }
        output += "]";
        print(output);
        */

    }

    void OnCollisionEnter( Collision collision )
    {   /*
        string output = " [ ";
        for (int i = 0; i < index_rofl; i++)
        {
            output += previous_velocities[i].ToString() + ", ";
        }
        output += "]";
        print("current state of previous_velocities is:\n" + output);
        */
        float speed = previous_velocities[0];
        print("speed: " + speed.ToString());
        print("before: " + audio_src_.volume.ToString());
        print("scalar: " + (default_sound_volume_ * speed * weight_).ToString());
        audio_src_.volume = default_sound_volume_ * speed * weight_;
        print("after: " + audio_src_.volume.ToString());
        audio_src_.PlayOneShot(sound_);
        AudioSource.PlayClipAtPoint(sound_, transform.position, default_sound_volume_ * speed * weight_);
    }

    
}
