using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class WinTitles : MonoBehaviour
{
    //Import the following.
    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern System.IntPtr FindWindow(System.String className, System.String windowName);
    //Get the window handle.

    public static WinTitles win;

    private void Awake()
    {
        win = this;
    }
    public static void  ChangeTitle(string newTi)
    {
        string oldTitle = PlayerPrefs.GetString("CurrentTitle");
        var windowPtr = FindWindow(null, oldTitle);
        //Set the title text using the window handle.
        SetWindowText(windowPtr, newTi);
        
        PlayerPrefs.SetString("CurrentTitle", newTi);
    }
    
}
