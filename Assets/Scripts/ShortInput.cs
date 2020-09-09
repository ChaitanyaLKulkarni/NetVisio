using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortInput : MonoBehaviour {

    private static ShortInput instance;
    public List<ShortKeys> shortk = new List<ShortKeys>();
    private Dictionary<string, int> shorts = new Dictionary<string, int>();

	// Use this for initialization
	void Start () {
        instance = this;
        shorts.Clear();
        for (int i = 0; i < shortk.Count; i++)
        {
            shorts.Add(shortk[i].name, i);
        }
	}

    public static bool GetKey(string s)
    {
        return instance.GetKey(s,true);
    }

    private bool GetKey(string s,bool d)
    {
        int i = shorts[s];
        if (shortk[i].Ctrl)
        {
            if (!(Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && !UnityEngine.Application.isEditor)
            {
                return false;
            }
        }
        if (shortk[i].Shift)
        {
            if (!(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))
            {
                return false;
            }
        }
        if (Input.GetKeyDown(shortk[i].Key))
        {
            return true;
        }
        return false;
    }
}

[System.Serializable]
public struct ShortKeys
{
    public string name;
    public KeyCode Key;
    public bool Shift;
    public bool Ctrl;
}