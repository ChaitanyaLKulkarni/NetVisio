using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPro : MonoBehaviour {
    
    public void SetProp(string s)
    {
        string PropertyName = GetComponent<Text>().text;
        transform.GetComponentInParent<ShowProperties>().Current.SetParaValue(PropertyName, s);
        BaseComp old = transform.GetComponentInParent<ShowProperties>().Current;
        transform.GetComponentInParent<ShowProperties>().Current = null;
        transform.GetComponentInParent<ShowProperties>().ShowPorp(ref old);


    }
}
