using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieManager : MonoBehaviour {
   
    public static CookieManager Instance = null;
   
    // will be set to some session id after login
    private Hashtable session_id = new Hashtable();

    void Awake()
    {
        Instance = this;
    }

    // and some helper functions and properties
    public void ClearSessionCookie(){
        session_id["Cookie"] = null;
    }
   
    public void SetSessionCookie(string s){
        session_id["Cookie"] = s;
    }
   
    public Hashtable SessionCookie{
        get { return session_id; }
    }
   
    public string GetSessionCookie(){
        return session_id["Cookie"] as string;
    }
   
    public bool SessionCookieIsSet{
        get { return session_id["Cookie"] != null; }
    }
   
   
    
}
