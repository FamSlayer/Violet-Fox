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
    

    public void TaskOnClick()
    {
        print("You have clicked the button!");
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        print("done loading..?");
    }
}