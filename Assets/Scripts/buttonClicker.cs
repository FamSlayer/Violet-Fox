using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class buttonClicker : MonoBehaviour
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener( TaskOnClick );
        print("added onclick function");
        //print(btn.onClick.ToString);
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

        }
    }

    public void TaskOnClick()
    {
        print("You have clicked the button!");
        SceneManager.LoadScene("OrphanageMap", LoadSceneMode.Additive);
    }
}