using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleObj : MonoBehaviour {

    public Modem sendBy;
    public Packet packet;

    public bool isStarted;

    public float size=0.1f;
    private float incre=3f;
    private float maxsize=11f;

    private CircleCollider2D cl;
    private void Start()
    {
        cl = GetComponent<CircleCollider2D>();
    }

    public void SetCircle(Modem m,Packet p)
    {
        sendBy = m;
        packet = p;
        isStarted = true;
    }

    private void Update()
    {
        if (size>=maxsize)
        {
            Destroy(this.gameObject);
            return;
        }
        if (isStarted)
        {
            size += incre * Time.deltaTime;
            transform.localScale = new Vector3(size + 1, size + 1, 1f);
        }
    }
}
