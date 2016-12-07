using UnityEngine;
using System.Collections.Generic;

public class UIWheelController : MonoBehaviour {

    enum state_enum
    {
        spinning,
        stationary
    }
    state_enum w_state = state_enum.stationary;

    
    public float wheel_radius;
    public float wheel_turn_speed;
    public float snapping_threshold;
    [Range(.2f, .8f)]
    public float item_pop;

    public List<string> icon_names;
    public List<Sprite> icon_sprites;
    public Dictionary<string, Sprite> icon_map;

    public PickUp player_pickup;
    public List<string> Inventory_n;
    public List<GameObject> Inventory_i;
    public int selected = 0;

    GameObject player;
    List<GameObject> children;
    int goal_orientation;
    int inventory_size;
    Vector3 stating_rot;
    Vector3 goal_rot_change;

    Throw player_throw;

    
    bool setup0 = false;
    bool setup1 = false;
    bool setup2 = false;
    bool setup3 = false;
    // Use this for initialization
    void Start () {

        // initialize all the data structures
        icon_map = new Dictionary<string, Sprite>();
        children = new List<GameObject>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_pickup = player.GetComponent<PickUp>();
        player_throw = player.GetComponent<Throw>();
        Inventory_n = player_pickup.Inventory_names;
        Inventory_i = player_pickup.Inventory_items;

        inventory_size = Inventory_n.Count;

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


        // load the icons into the UI from an Inventory
        foreach ( Transform child in transform )
        {
            children.Add(child.gameObject);
        }


        // if starting the game with a small default inventory
        if ( inventory_size < 3 )
        {
            for(int i=0; i< children.Count; i++)
            {
                children[i].SetActive(false);
            }

            if (inventory_size == 1 || inventory_size == 2)
            {
                children[1].SetActive(true);
            }
            if (inventory_size == 2)
            {
                children[2].SetActive(true);
            }

        }
        else
        {
            // strings of the 3 Inventory items around the currently selected one
            string c1_name = getNameAtInventoryIndex(selected - 1);
            string c2_name = getNameAtInventoryIndex(selected);
            string c3_name = getNameAtInventoryIndex(selected + 1);

            // load up the images on the UI with the names of 
            children[0].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c1_name];
            children[1].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c2_name];
            children[2].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c3_name];

            children[1].transform.localScale = new Vector3(item_pop, item_pop, 0f);
        }

        


        

    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if ( inventory_size != player_pickup.Inventory_items.Count )
        {
            updateUI();
            inventory_size = player_pickup.Inventory_items.Count;
        }
        


        /*  4 states
         *  1. Inventory.Count == 0
         *      don't display anything, all the children should be disabled
         *  2. Inventory.Count == 1
         *      display icon at children[i], do not allow Q and E movement of the wheel
         *          - play the sound that says "you can't spin any further"
         *  3. Inventory.Count == 2
         *      display icons at children[1] and children[2], allow Q and E movement of the wheel
         *          - but ONLY between them, and play "you can't spin any further" sound 
         *  4. Inventory.Count >= 3
         *      enable all the icons and stuff
         *      do everything normally
         */


        if(inventory_size == 0)
        {
            if ( !setup0 )
            {
                int converted = goal_orientation % 8;
                if (converted < 0) converted += 8;

                // set enabled
                children[converted].SetActive(false);
                children[converted + 1].SetActive(false);
                children[converted + 2].SetActive(false);
                setup0 = true;

                setup1 = false;
                setup2 = false;
                setup3 = false;
            }

        }
        else if( inventory_size == 1)
        {
            if ( !setup1 )
            {
                int converted = goal_orientation % 8;
                if (converted < 0) converted += 8;

                int c1_index = converted;
                int c2_index = converted + 1;
                if (c2_index > 7) c2_index -= 8;
                int c3_index = converted + 2;
                if (c3_index > 7) c3_index -= 8;

                // set enabled
                //print("inventory_i.Count == 1, and converted = " + converted);
                children[c1_index].SetActive(false);
                children[c2_index].SetActive(true);
                children[c3_index].SetActive(false);

                oneItemSetup();

                setup1 = true;
                setup0 = false;
                setup2 = false;
                setup3 = false;
            }
            

        }
        else if(inventory_size == 2)
        {
            if ( !setup2 )
            {
                int converted = goal_orientation % 8;
                if (converted < 0) converted += 8;

                //print("converted = " + converted);
                // set disabled on all the kiddos
                for(int i=0; i<children.Count; i++)
                {
                    children[i].SetActive(false);
                }

                // and be sure to loop around!
                if(converted + 1 >= children.Count)
                {
                    children[converted + 1 - children.Count].SetActive(true);
                }
                else
                {
                    children[converted + 1].SetActive(true);
                }


                if (selected == 0)
                {
                    if (converted + 2 >= children.Count)
                    {
                        children[converted + 2 - children.Count].SetActive(true);
                    }
                    else
                    {
                        children[converted + 2].SetActive(true);
                    }
                }
                else if (selected == 1)
                {
                    children[converted].SetActive(true);
                }

                twoItemSetup();

                setup2 = true;
                setup0 = false;
                setup1 = false;
                setup3 = false;
            }
            
            // handle input
            if (Input.GetKeyDown(KeyCode.E))
            {
                if ( selected == 0 )
                {
                    goal_orientation++;
                    selected = 1;
                    goal_rot_change = new Vector3(0, 0, 45 * goal_orientation);
                    w_state = state_enum.spinning;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if ( selected == 1 )
                {
                    goal_orientation--;
                    selected = 0;
                    goal_rot_change = new Vector3(0, 0, 45 * goal_orientation);
                    w_state = state_enum.spinning;
                }
                
            }

            // loop 'selected' around the length of the Inventory_n
            if (selected == Inventory_n.Count) selected = selected % Inventory_n.Count;
            if (selected < 0) selected += Inventory_n.Count;

            
            //int converted = goal_orientation % 8;
            //if (converted < 0) converted += 8;

            if (w_state == state_enum.spinning)
            {
                Quaternion quat = Quaternion.identity;
                quat.eulerAngles = goal_rot_change;
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, wheel_turn_speed * Time.deltaTime);

                foreach (Transform child in transform)
                {
                    child.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }

                int converted = goal_orientation % 8;
                if (converted < 0) converted += 8;
                // converted is now the index of the child at the bottom of the visible wheel

                int c1, c2;
                if ( selected == 1 )
                {
                    c1 = converted;
                    c2 = converted + 1;
                    if (c2 > 7) c2 -= 8;
                    children[c1].transform.localScale = new Vector3(0.2f, 0.2f, 1);
                    children[c2].transform.localScale = new Vector3(item_pop, item_pop, 1);
                }
                else
                {
                    c1 = converted + 1;
                    if (c1 > 7) c1 -= 8;
                    c2 = converted + 2;
                    if (c2 > 7) c2 -= 8;
                    children[c1].transform.localScale = new Vector3(item_pop, item_pop, 1);
                    children[c2].transform.localScale = new Vector3(0.2f, 0.2f, 1);
                }


                player_throw.changeObjectHolding(Inventory_i[selected]);

                if (Mathf.Abs(quat.eulerAngles.z - transform.rotation.eulerAngles.z) < snapping_threshold)
                {
                    transform.rotation = quat;
                    w_state = state_enum.stationary;
                }
            }




        }
        // INVENTORY_SIZE == 3
        else 
        {
            if ( !setup3 )
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].SetActive(true);
                }
                threeItemSetup();

                setup3 = true;
                setup0 = false;
                setup1 = false;
                setup2 = false;
            }

            /*for(int i=0; i<Inventory_i.Count; i++)
            {
                print("[" + i.ToString() + "] - " + Inventory_n[i]);
            }*/


            // handle input
            if (Input.GetKeyDown(KeyCode.E))
            {
                goal_orientation++;
                selected++;
                goal_rot_change = new Vector3(0, 0, 45 * goal_orientation);
                w_state = state_enum.spinning;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                goal_orientation--;
                selected--;
                goal_rot_change = new Vector3(0, 0, 45 * goal_orientation);
                w_state = state_enum.spinning;
            }

            // loop 'selected' around the length of the Inventory_n
            if (selected == Inventory_n.Count) selected = selected % Inventory_n.Count;
            if (selected < 0) selected += Inventory_n.Count;

            // converted is now the index of the child at the bottom of the visible wheel
            int converted = goal_orientation % 8;
            if (converted < 0) converted += 8;

            if (w_state == state_enum.spinning)
            {
                Quaternion quat = Quaternion.identity;
                quat.eulerAngles = goal_rot_change;
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, wheel_turn_speed * Time.deltaTime);

                foreach (Transform child in transform)
                {
                    child.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                
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

                // NOW call the function to change held_item_ in Throw.cs
                // convert to the size of the inventory
                c2_index = c2_index % Inventory_i.Count;
                player_throw.changeObjectHolding(Inventory_i[selected]);

                if (Mathf.Abs(quat.eulerAngles.z - transform.rotation.eulerAngles.z) < snapping_threshold)
                {
                    transform.rotation = quat;
                    w_state = state_enum.stationary;
                }
            }
        }
        
        
        







	}

    string getNameAtInventoryIndex ( int x )
    {
        int s = Inventory_n.Count;
        x = x % s;  // the inventory is really a circle, so loop around it with this mod
        if ( x < 0 ) return Inventory_n[s + x];
        return Inventory_n[x];
    }
    

    void updateUI ()
    {
        //print("start of updateUI.");
        //print("Previously, inventory_size was " + inventory_size + ", but it is now " + player_pickup.Inventory_items.Count);

        if ( player_pickup.Inventory_items.Count > 2 )
        {
            //print("Inventory will be greater than 2 anyway");
            string c1_name = getNameAtInventoryIndex(selected - 1);
            string c2_name = getNameAtInventoryIndex(selected);
            string c3_name = getNameAtInventoryIndex(selected + 1);

            //print("bot = " + c1_name + "  middle = " + c2_name + "  top = " + c3_name);

            threeItemSetup();
            //children[0].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c1_name];
            //children[1].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c2_name];
            //children[2].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[c3_name];

            //children[1].transform.localScale = new Vector3(item_pop, item_pop, 0f);
        }



    }




    
    void oneItemSetup()
    {

        if (selected != 0)
            selected = 0;
        //print("in one item setup: selected = " + selected);

        int converted = goal_orientation % 8 + 1;
        if (converted < 0) converted += 8;

        string item_name = getNameAtInventoryIndex(selected);
        children[converted].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[item_name];
    }

    void twoItemSetup()
    {
        if (selected > 1)
            selected = 0;
        //print("in two item setup: selected = " + selected);
        string item_name = getNameAtInventoryIndex(selected);

        int converted = goal_orientation % 8;
        if (converted < 0) converted += 8;

        int c1_index = converted;
        int c2_index = converted + 1;
        if (c2_index > 7) c2_index -= 8;
        int c3_index = converted + 2;
        if (c3_index > 7) c3_index -= 8;

        children[c2_index].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[item_name];

        string item2_name;
        if ( selected == 0 )
        {
            item2_name = getNameAtInventoryIndex(1); // <-- b/c selected == 0 , selected + 1 = 1
            children[c3_index].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[item2_name];
        }
        else if ( selected == 1 )
        {
            item2_name = getNameAtInventoryIndex(0); // <-- b/c selected == 1 , selected + 1 = 0
            children[c1_index].GetComponent<UnityEngine.UI.Image>().sprite = icon_map[item2_name];
        }

    }


    void threeItemSetup()
    {
        if (selected > player_pickup.Inventory_items.Count)
            selected = 0;
        //print("in threeItemSetup: selected = " + selected.ToString());
        int converted = goal_orientation % 8;
        if (converted < 0) converted += 8;

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

        children[c1_index].transform.localScale = new Vector3(0.2f, 0.2f, 0f);
        children[c2_index].transform.localScale = new Vector3(item_pop, item_pop, 0f);
        children[c3_index].transform.localScale = new Vector3(item_pop, item_pop, 0f);
    }


    public void updateUIAfterThrow()
    {
        int size = player_pickup.Inventory_items.Count;
        if ( size == 1 )
        {
            oneItemSetup();
        }
        else if( size == 2 )
        {
            twoItemSetup();
        }
        else if( size > 2 )
        {
            threeItemSetup();
        }
    }

}
