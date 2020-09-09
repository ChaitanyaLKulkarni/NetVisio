using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class recentFiles : MonoBehaviour {

    public string path;
    private string fname;

    private void Start()
    {
        

    }

    public void Init(string p)
    {
        path = p;
        int n = path.LastIndexOf('/');
        fname = path.Substring(n + 1, path.Length - n - 1);
        transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = fname;
    }

    public void Open()
    {

        Manager.Instance.Load(path);
    }

    private void OnMouseDown()
    {
        Open();
    }
}
