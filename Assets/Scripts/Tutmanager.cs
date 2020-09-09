using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutmanager : MonoBehaviour {

    public static Tutmanager Instance { get; private set; }
    public static bool isTut=false;
    private CanvasGroup cg;
    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }
    public void Show()
    {
        Manager.Instance.Load(true);
        isTut = true;
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true; 
    }

    public void Hide()
    {
        isTut = false;
        Manager.Instance.Load(-1);
        Manager.Instance.contents.SetActive(true);
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void Show(int id)
    {
        ReportManager.instance.Stop();
        Manager.Instance.Load(id);
        Invoke("Play", 1f);
    }

    private void Play()
    {
        ReportManager.instance.StartR();
    }
}
