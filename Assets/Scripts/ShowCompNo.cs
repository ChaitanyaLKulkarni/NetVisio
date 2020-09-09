using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowCompNo : MonoBehaviour {

    public static ShowCompNo instance;
    private void Awake()
    {
        instance = this; 
    }

    public void Set(List<string> nos)
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = nos[i - 1]; 
        }
    }
}
