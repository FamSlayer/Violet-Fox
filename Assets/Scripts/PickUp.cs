using UnityEngine;
using System.Collections.Generic;

public class PickUp : MonoBehaviour {

    public List<string> Inventory_names;
    public List<GameObject> Inventory_items;


    List<GameObject> nearby_items;

	// Use this for initialization
	void Start () {
        nearby_items = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void onTriggerEnter( Collider other )
    {
        print("collider happened!");
        print(other);
    }

    void onCollisionEnter(Collision other)
    {
        print("collision happened!");
        print(other);
    }


    public bool addToInventory( GameObject obj, string obj_name )
    {
        if (Inventory_items.Contains(obj))
        {
            return false;
        }
        Inventory_names.Add(obj_name);
        Inventory_items.Add(obj);
        return true;
    }

    public void removeFromInventory(GameObject obj, string obj_name)
    {
        Inventory_names.RemoveAt(  Inventory_items.IndexOf(obj)  );
        Inventory_items.Remove(obj);
    }
}
