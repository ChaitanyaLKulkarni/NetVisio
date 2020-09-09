using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class UpdateCheck : MonoBehaviour {

    private WWW wwwData;

    // Use this for initialization
    void Start () {
        StartCoroutine(WaitFor("http://netvisio.dx.am/version.txt"));
    }

    IEnumerator WaitFor(string s)
    {
        wwwData = new WWW(s);
        while (!wwwData.isDone)
        {
            yield return null;

        }
        Debug.Log(wwwData.text);
        string[] uparr = wwwData.text.Split('|');
        if (uparr[0] != Application.version)
        {
            Application.OpenURL(uparr[1]);
        }
    }
}
