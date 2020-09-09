using System.Collections;
using System.Collections.Generic;
using UnityWinForms;
using UnityWinForms.Examples;
using UnityEngine;
using System.Windows.Forms;
using UnityWinForms.Examples.Panels;
using System;
using System.Drawing;

public class GameGuiController : MonoBehaviour
{

    public Vector2 pl;
    void Start()
    {
        var f = new frm();
        f.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        f.Show();
        f.uwfMovable = false;
        f.AutoScroll = false;
        f.AutoSize = false;
        f.Location = new Point(0, -22);
        f.Height = 45;
        f.Width = UnityEngine.Screen.width + 30;
        f.MaximumSize = new Size(1000, 45);

    }
    private void Update()
    {

    }
}

public class frm : Form
{
    public frm()
    {

        var t = typeof(PanelMenuStrip);
        var panel = Activator.CreateInstance(t as Type) as BaseExamplePanel;
        var currentPanel = panel;


        currentPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        currentPanel.Location = new Point(0, 20);
        currentPanel.Height = 25; // We don't want to hide SizeGripRenderer with scrollbars.
        currentPanel.Width = 1000;
        Controls.Add(currentPanel);
        currentPanel.Initialize();
    }

}