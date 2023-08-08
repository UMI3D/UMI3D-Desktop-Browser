using System;
using System.Collections.Generic;
using umi3d.baseBrowser.connection;
using umi3d.cdk.menu;
using umi3d.common.interaction;
using UnityEngine.UIElements;

public class LoginState : MenuState
{
    private MenuAsset _formMenu;

    public LoginState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.ConnectionScreen.Show();
    }

    public override void Exit()
    {
        _machine.ConnectionScreen.Hide();

    }

    public override void SetData(VisualElement elements, Action callback)
    {
        _machine.ConnectionScreen.Clear();
        _machine.ConnectionScreen.Root.Add(elements);
    }
}