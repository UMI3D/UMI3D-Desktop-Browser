using DesktopBrowser.UI.CustomElement;
using DesktopBrowser.UIControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolboxesWindow_UIController : UIController
{
    private ToolboxWindow_E toolboxWindow;

    // Start is called before the first frame update
    void Start()
    {
        toolboxWindow = new ToolboxWindow_E(BindVisual("toolboxWindow"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
