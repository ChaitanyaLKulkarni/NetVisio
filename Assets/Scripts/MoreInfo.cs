using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoreInfo : MonoBehaviour {

    public Image symbol;
    public Image ori;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Button wiki;
    public string url;

    private void Start()
    {
        Default();
    }

    public void ShowInfo(Component comp)
    {
        symbol.sprite = comp.img;
        symbol.color = Color.white; 

        ori.sprite = comp.oriimg;
        ori.color = Color.white;

        title.text = comp.Title;
        desc.text = comp.info;
        url = comp.wikilink;
        wiki.gameObject.SetActive(true);
    }

    public void Default()
    {
        if (Tutmanager.isTut)
            return;
        symbol.sprite = null;
        symbol.color = new Color(1,1,1,0);

        ori.sprite = null;
        ori.color = new Color(1, 1, 1, 0);

        title.text = "NetVisio";
        desc.text = "Network Visual Simulator (NetViSio) Is a simulator that can be used to simulate real life networks. With our software you can design a system for any type of usage. This helps us to visualize any type of network configurations, topologies. With this software various we can simulate different real-time protocols. \nAll Rights Reserved © 2019 Network Visual Simulator.";
        wiki.gameObject.SetActive(false);
        url = "";
    }
    public void ShowS(string t, string des)
    {
        title.text = t;
        desc.text = des;
    }

    public void ShowWiki()
    {
        Application.OpenURL(url);
    }
}
