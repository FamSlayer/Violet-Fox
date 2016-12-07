﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	void activated()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
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
