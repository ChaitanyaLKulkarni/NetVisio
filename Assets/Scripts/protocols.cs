using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class protocols : MonoBehaviour {

    public int packsend = 0;

    Animator anime;


    public TMP_Text fut;
    public TMP_Text sut;
    public TMP_Text fdt;
    public TMP_Text sdt;

    private bool iscurrupt = false;
    private bool skip2 = false;
    public enum Proto
    {
        StopAWait,
        GoBackN,
        Selective
    }

    public Proto proto;

    private void Start()
    {
        anime = GetComponent<Animator>();
    }

    public void Startpack()
    {
        if(!skip2)
            fdt.text = "Packet ID : " + packsend;
    }

    public void AcceptPack()
    {
        if(proto == Proto.GoBackN && iscurrupt)
        {
            sdt.text = "Packet ID : " + packsend + " \nERROR";
            return;
        }
        
        if (skip2)
        {
            sdt.text = "Packet ID : 1 \nOK";
            sut.text += "ID : 1\n";
            skip2 = false;
            return;
        }
        if (proto == Proto.Selective && packsend == 1)
        {
            sdt.text = "Packet ID : " + packsend + " \nERROR";
            return;
        }
        sdt.text = "Packet ID : " + packsend + " \nOK";
        sut.text += "ID : " + packsend + "\n";  
    }

    public void ACKPack()
    {
        if (iscurrupt)
        {
            iscurrupt = false;
        }
        else if (skip2)
        {
            fdt.text = "Packet ID : 1";
            packsend = 2;
        }
        else
        {
            
            packsend++;
            fut.text = "";
            for (int i = packsend; i < 4; i++)
            {
                fut.text += "ID : " + i + "\n";
            }

            if (packsend == 4)
            {
                anime.SetBool("isrun", false);
            }
            if (proto == Proto.GoBackN && packsend == 1)
            {
                anime.SetTrigger("iscurrupt");
                iscurrupt = true;
            }
            if (proto == Proto.Selective && packsend == 1)
            {
                anime.SetTrigger("isselect");
            }

        }

        
    }

    public void SecondStart()
    {
        fdt.text = "Packet ID : 2";
    }

    public void SecondAccept()
    {

        sdt.text = "Packet ID : 2 \nOK";
        sut.text += "ID : 2\n";
        skip2 = true;
    }

    public void SecondAck()
    {
        fut.text = "ID : 1\nID : 3";
    }
    

    public void Wait()
    {
        Stop();
        proto = Proto.StopAWait;
        anime.SetBool("iscomplete",false);
        anime.SetBool("isrun", true);
        Ss();
    }
    public void GoBack()
    {
        Stop();
        proto = Proto.GoBackN;
        anime.SetBool("iscomplete",false);
        anime.SetBool("isrun", true);
        Ss();
    }
    public void Selective()
    {
        Stop();
        proto = Proto.Selective;
        anime.SetBool("iscomplete",false);
        anime.SetBool("isrun", true);
        Ss();
    }
    public void Stop()
    {
        packsend = 0;
        anime.SetBool("iscomplete",true);
        anime.SetBool("isrun",false); 
    }

    private void Ss()
    {
        packsend = 0;
        fut.text = "";
        for (int i = packsend; i < 4; i++)
        {
            fut.text += "ID : " + i + "\n";
        }
        sdt.text = "";
        sut.text = "";
        fdt.text = "";
    }
}
