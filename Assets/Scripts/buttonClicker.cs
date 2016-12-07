using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class buttonClicker : MonoBehaviour
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        print("added onclick function");
    }




    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
    }
}