using UnityEngine;
using System.Collections.Generic;

public class PickUp : MonoBehaviour {

    public List<string> Inventory_names;
    public List<GameObject> Inventory_items;

    public AudioClip prefab_sound_;


    //List<GameObject> nearby_items;

	// Use this for initialization
	void Start ()
    {
        //nearby_items = new List<GameObject>();

        Inventory_names.Add("bottle");
        Inventory_items.Add(makeDefaultItem(PrimitiveType.Cylinder, "bottle"));

        Inventory_names.Add("chamber pot");
        Inventory_items.Add(makeDefaultItem(PrimitiveType.Cube, "chamber pot"));

        Inventory_names.Add("ring");
        Inventory_items.Add(makeDefaultItem(PrimitiveType.Sphere, "ring"));

        Inventory_names.Add("fork");
        Inventory_items.Add(makeDefaultItem(PrimitiveType.Capsule, "fork"));

        



    }

    // Update is called once per frame
    void Update ()
    {
	
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

    public void removeFromInventory(GameObject obj)
    {
        print("are Inventory_items.Count == Inventory_names.Count);
        Inventory_names.RemoveAt(  Inventory_items.IndexOf(obj)  );
        Inventory_items.Remove(obj);
    }


    GameObject makeDefaultItem( PrimitiveType type, string nombre )
    {
        GameObject obj = GameObject.CreatePrimitive(type);

        // adjust scale
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        // add child text object
        GameObject text_obj = new GameObject();
        text_obj.AddComponent<TextMesh>();
        text_obj.transform.parent = obj.transform;


        // add audio source
        obj.AddComponent<AudioSource>();

        // rigidbody mass and stuff
        obj.AddComponent<Rigidbody>();
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.mass = 0.5f;

        // add Item (Script) component
        obj.AddComponent<Item>();
        Item item_script = obj.GetComponent<Item>();
        item_script.name_ = nombre;
        item_script.text_offset = new Vector3(0f, 0.5f, 0f);
        item_script.default_sound_volume_ = 1;
        item_script.sound_ = prefab_sound_;
        item_script.prev_velocities_kept = 2;

        // add capsule collider
        obj.AddComponent<CapsuleCollider>();
        CapsuleCollider capn = obj.GetComponent<CapsuleCollider>();
        capn.isTrigger = true;
        capn.radius = 10;
        capn.height = 40;

        obj.SetActive(false);

        return obj;

    }
}
