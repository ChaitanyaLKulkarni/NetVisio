using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : BaseComp {


    private Dictionary<string, int> database = new Dictionary<string, int>();

    public void Start()
    {
        base.AddParas(new string[] { "DisplayName", "Ports" });
        Dictionary<string, string> Disabled = new Dictionary<string, string>
        {
            { "Connectors", "RJ45" },
            { "Operating Temp.", "0 - 40 C" },
            { "Operating Humadity.", "8 - 80 %" },
            { "Max Power","265 W" }
        };

        base.SetDisablePara(Disabled);
        SetDisplayname(this);
    }
    void Update()
    {
        
    }


    public override void Acceptpack(Packet p)
    {
        if(!database.ContainsKey(p.Para["Source IP"]))
        {
            List<Links> ln = GetComponent<Info>().links;
            for (int i=0;i< ln.Count;i++)
            {
                if(ln[i].lr == p.lr)
                {
                    database.Add(p.Para["Source IP"], i);
                    break;
                }
            }
        }
        if (p.pType != Packet.PType.ACK)
        {
            Packet ori = p;
            packs.Add(ori);
            if (database.ContainsKey(ori.Para["Destination IP"]))
            {

                List<Links> ln = GetComponent<Info>().links;
                Links l = ln[database[ori.Para["Destination IP"]]];
                SendPack(ori, TO.SINGLE,false,l);
            }
            else
            {
                SendPack(ori, TO.ALL, true);
            }
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
            if (database.ContainsKey(p.Para["Destination IP"]))
            {

                List<Links> ln = GetComponent<Info>().links;
                Links l = ln[database[p.Para["Destination IP"]]];
                SendPack(p, TO.SINGLE, false, l);
            }
            else
            {
                SendPack(p, TO.ALL);
            }
            
            Destroy(p.gameObject);
            Remove();
        }
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


    void OnDestroy()
    {
        ComponentManger.Instance.ClearDeleted(this);
    }

}
