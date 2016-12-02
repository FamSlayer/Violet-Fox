using UnityEngine;
using System.Collections.Generic;

public class UIWheelController : MonoBehaviour {

    public GameObject player;
    public float wheel_radius;

    public List<string> icon_names;
    public List<Sprite> icon_sprites;
    public Dictionary<string, Sprite> icon_map;

    public List<string> Inventory;
    public int selected;

    List<GameObject> children;
	// Use this for initialization
	void Start () {

        // initialize all the data structures
        icon_map = new Dictionary<string, Sprite>();
        children = new List<GameObject>();


        // load up a dictionary of icon_names -> icon sprites
        if (icon_names.Count == icon_sprites.Count && icon_names.Count != 0)
        {
            print("icon_names and icon_sprites are incorrect rn");
            for (int i = 0; i < icon_names.Count; i++)
            {
                icon_map.Add(icon_names[i], icon_sprites[i]);
            }
        }
        else
        {
            print("icon_names and icon_sprites are incorrect rn");
            for (int i = 0; i < icon_names.Count; i++)
            {
                icon_map.Add(icon_names[i], icon_sprites[i]);
            }
        }


        // more importantly, use that dictionary to load up 
        print("");
        print("Now going to print the children:");
        int c = 0;
        foreach ( Transform child in transform )
        {
            //child is your child transform
            print("   [" + c.ToString() + "] " + child.gameObject.ToString());
            children.Add(child.gameObject);
            c++;
        }


        // strings of the 3 inventory items around the currently selected one
        string c1_name, c2_name, c3_name;
        c1_name = getNameAtInventoryIndex(selected - 1);
        c2_name = getNameAtInventoryIndex(selected);
        c3_name = getNameAtInventoryIndex(selected + 1);

        print("c1_name: " + c1_name + "   c2_name: " + c2_name + "   c3_name: " + c3_name);

        children[0].GetComponent<Image>().sprite = icon_map[c1_name];
        children[1].GetComponent<Image>().sprite = icon_map[c2_name];
        children[2].GetComponent<Image>().sprite = icon_map[c3_name];



    }
	
	// Update is called once per frame
	void Update () {
	
	}

    string getNameAtInventoryIndex ( int x )
    {
        int s = Inventory.Count;
        x = x % s;  // the inventory is really a circle, so loop around it with this mod
        if ( x < 0 ) return Inventory[s + x];
        return Inventory[x];
    }
}
