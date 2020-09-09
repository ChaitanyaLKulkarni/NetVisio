using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileFS : MonoBehaviour {

    public string fn;
    public int id;

	public void Set(string fn,int id)
    {
        this.fn = fn;
        this.id = id;
        GetComponentInChildren<Text>().text = fn;
    }

    public void Show()
    {
        StartCoroutine(Client.instance.GetFile(fn,id));
    }
}
