using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modem : BaseComp {


    bool isStarted=false;

    public void Start()
    {
        base.AddParas(new string[] { "DisplayName", "Ports","Channel" });
        SetDisplayname(this);

    }

    public override void Acceptpack(Packet p)
    {
        if (p.pType != Packet.PType.ACK)
        {
            Packet ori = p;
            packs.Add(ori);
            Send(p);
        }
        else
        {
            int id = p.PacketId;
            List<Packet> temp = packs;
            foreach (Packet pa in temp)
            {
                if (pa)
                {
                    if (pa.PacketId == id)
                    {
                        Destroy(pa.gameObject);
                    }
                }
                else
                {
                    Remove();
                }
            }
            Send(p);
            Destroy(p.gameObject);
            Remove();
        }
    }


    /// <summary>
    /// This is Same as Accept packet but it accepts it from the wireless communication
    /// </summary>
    /// <param name="p"></param>
    /// <param name="t"></param>
    public void Acceptpack(Packet p,bool t)
    {
        if (p.pType != Packet.PType.ACK)
        {
            Packet ori = p;
            packs.Add(ori);
            SendPack(p, TO.ALL);
        }
        else
        {
            int id = p.PacketId;
            List<Packet> temp = packs;
            foreach (Packet pa in temp)
            {
                if (pa)
                {
                    if (pa.PacketId == id)
                    {
                        Destroy(pa.gameObject);
                    }
                }
                else
                {
                    Remove();
                }
            }
            SendPack(p, TO.ALL);
            Destroy(p.gameObject);
            Remove();
        }
    }

    /// <summary>
    /// Send the incoming data to wireless channel
    /// </summary>
    /// <param name="p"></param>
    private void Send(Packet p)
    {
        GameObject go = Instantiate(Manager.Instance.circleObj, transform.position,Quaternion.identity,transform);
        go.GetComponent<CircleObj>().SetCircle(this, p);
    }

    void Remove()
    {
        List<Packet> temp = new List<Packet>();
        foreach (Packet pa in packs)
        {
            if (pa != null)
            {
                temp.Add(pa);
            }
        }
        packs = temp;
    }


    /// <summary>
    /// Detects any incoming packet that has trasmitted from other in wireless channel
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CircleObj>())
        { 
            if (collision.gameObject.GetComponent<CircleObj>().sendBy != this && collision.gameObject.GetComponent<CircleObj>().sendBy.Para["Channel"] == Para["Channel"])
                Acceptpack(collision.gameObject.GetComponent<CircleObj>().packet, true);
        }
    }

    void OnDestroy()
    {
        ComponentManger.Instance.ClearDeleted(this);
    }
}
