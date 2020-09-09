using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class showPackInfo : MonoBehaviour {

    public int PackId;
	public void Call()
    {
        
        ReportManager.instance.ShowPackInfo(PackId);
    }
}
