using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hub : BaseComp
{



    public void Start()
    {
        base.AddParas(new string[] { "DisplayName", "Ports" });

        Dictionary<string, string> Disabled = new Dictionary<string, string>
        {
            { "Connectors", "RJ45" },
            { "Access Method", "CSMA/CD" },
            { "Media Supported", "100 ohm UTP or STP" },
            { "Operating Temp.", "0 - 40 C" },
            { "Power","13.5 VDC, 2A" }
        };

        base.SetDisablePara(Disabled);
        SetDisplayname(this);

    }


    /// <summary>
    /// Check if this packet is already given to the HUB
    /// </summary>
    /// <param name="packid">ID of thet incoming packet</param>
    /// <returns></returns>
    bool CheckForDupli(int packid)
    {
        foreach (Packet p in packs)
        {
            if (p.PacketId == packid)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Accept the incoming packet and implement the algorithm of HUB
    /// </summary>
    /// <param name="p">Packet that has arrived</param>
    public override void Acceptpack(Packet p)
    {
        if (p.pType != Packet.PType.ACK)
        {
            Packet ori = p;
            packs.Add(ori);
            SendPack(p, TO.ALL, true);
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
    /// Remove the Packet that has been succesfully send
    /// </summary>
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
    /// This Get called when the object is Get destroyed to clear any residues
    /// </summary>
    void OnDestroy()
    {
        ComponentManger.Instance.ClearDeleted(this);
    }
}