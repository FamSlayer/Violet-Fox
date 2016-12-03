using UnityEngine;
using System.Collections.Generic;

public class UIWheelController : MonoBehaviour {

    enum state_enum
    {
        spinning,
        stationary
    }
    state_enum w_state = state_enum.stationary;

    public GameObject player;
    public float wheel_radius;
    public float wheel_turn_speed;
    public float snapping_threshold;
    [Range(.2f, .8f)]
    public float item_pop;

    public List<string> icon_names;
    public List<Sprite> icon_sprites;
    public Dictionary<string, Sprite> icon_map;

    public List<string> Inventory;
    public int selected;

    List<GameObject> children;
    int goal_orientation;
    Vector3 stating_rot;
    Vector3 goal_rot_change;
	// Use this for initialization
	void Start () {

        // initialize all the data structures
        icon_map = new Dictionary<string, Sprite>();
        children = new List<GameObject>();
        goal_orientation = 0;

        // load up a dictionary of icon_names -> icon sprites
        if (icon_names.Count == icon_sprites.Count && icon_names.Count != 0)
        {
            for (int i = 0; i < icon_names.Count; i++)
            {
                icon_map.Add(icon_names[i], icon_sprites[i]);
            }
        }
        else
        {
            print("icon_names and icon_sprites are incorrect. icon_map not created.");
        }


        // load the icons into the UI from an inventory
        foreach ( Transform child in transform )
        {
            children.Add(child.gameObject);
        }


        // strings of the 3 inventory items around the currently selected one
        string c1_name = getNameAtInventoryIndex(selected - 1);
        string c2_name = getNameAtInventoryIndex(selected);
        string c3_name = getNameAtInventoryIndex(selected + 1);

        // load up the images on the UI with the names of 
        children[0].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c1_name];
        children[1].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c2_name];
        children[2].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c3_name];

        children[1].transform.localScale = new Vector3(item_pop, item_pop, 0f);

    }
	
	// Update is called once per frame
	void Update ()
    {
        // handle input
        if ( Input.GetKeyDown(KeyCode.E) )
        {
            goal_orientation++;
            selected++;
            goal_rot_change = new Vector3(0, 0, 45 * goal_orientation);
            w_state = state_enum.spinning;
        }
        if ( Input.GetKeyDown(KeyCode.Q) )
        {
            goal_orientation--;
            selected--;
            goal_rot_change = new Vector3(0, 0, 45 * goal_orientation);
            w_state = state_enum.spinning;
        }

        // loop 'selected' around the length of the inventory
        if ( selected == Inventory.Count) selected = selected % Inventory.Count;
        if ( selected < 0 ) selected += Inventory.Count;

        

        int converted = goal_orientation % 8;
        if (converted < 0) converted += 8;

        // converted is now the index of the child at the bottom of the visible wheel

        //print("converted: " + converted.ToString());


        if (w_state == state_enum.spinning)
        {
            Quaternion quat = Quaternion.identity;
            quat.eulerAngles = goal_rot_change;
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, wheel_turn_speed * Time.deltaTime);

            foreach (Transform child in transform)
            {
                child.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            //print("quat = " + quat.eulerAngles);
            //print("transform.rotation = " + transform.rotation.eulerAngles);


            // strings of the 3 inventory items around the currently selected one
            string c1_name = getNameAtInventoryIndex(selected - 1);
            string c2_name = getNameAtInventoryIndex(selected);
            string c3_name = getNameAtInventoryIndex(selected + 1);

            // load up the images on the UI with the names of 
            int c1_index = converted;
            int c2_index = converted + 1;
            if (c2_index > 7) c2_index -= 8;
            int c3_index = converted + 2;
            if (c3_index > 7) c3_index -= 8;


            children[c1_index].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c1_name];
            children[c2_index].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c2_name];
            children[c3_index].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c3_name];

            children[c1_index].transform.localScale = new Vector3(0.2f, 0.2f, 1);
            children[c2_index].transform.localScale = new Vector3(item_pop, item_pop, 1);
            children[c3_index].transform.localScale = new Vector3(0.2f, 0.2f, 1);



            if (Mathf.Abs(quat.eulerAngles.z - transform.rotation.eulerAngles.z) < snapping_threshold)
            {
                transform.rotation = quat;
                w_state = state_enum.stationary;
            }
        }
        







	}

    string getNameAtInventoryIndex ( int x )
    {
        int s = Inventory.Count;
        x = x % s;  // the inventory is really a circle, so loop around it with this mod
        if ( x < 0 ) return Inventory[s + x];
        return Inventory[x];
    }
}
