using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	void activated()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
	// Update is called once per frame
	void Update () {
        
	}

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            activated();
        }
    }
}
