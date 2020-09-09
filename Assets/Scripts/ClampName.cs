using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClampName : MonoBehaviour {

    public static Camera cam;

    public Text text;
    public bool placed = false;
    public Vector2 Offset;
    public float a = 1f;
    public float b = 0.4f;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update () {
        if (placed)
        {
            Vector2 pos = cam.WorldToScreenPoint(this.transform.position);
            pos = pos - Offset;
            text.transform.position = pos;
            text.text = GetComponentInParent<Info>().GetDisplayName();
            text.fontSize = 10 + (int)(70f/ cam.orthographicSize);
            Offset = new Vector2(Offset.x, 13.7f * (29.5f / Camera.main.orthographicSize));
        }
        else
        {
            text.text = "";
        }
	}

}
