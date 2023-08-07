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

        _machine.ConnectionScreen.Back.clicked += Back;
    }

    private void Back()
    {
        BaseConnectionProcess.Instance.Leave();
        _machine.ToHome();
    }

    public override void Exit()
    {
        _machine.ConnectionScreen.Hide();

        _machine.ConnectionScreen.Back.clicked -= Back;
    }

    public override void SetData(List<VisualElement> elements, Action callback)
    {
        _machine.ConnectionScreen.Clear();
        var currentForm = _machine.ConnectionScreen.LoginForm;
        _machine.ConnectionScreen.PinButton.AddToClassList("hidden");
        foreach (var element in elements)
        {
            if (element.name.ToLower() == "or") 
            {
                currentForm = _machine.ConnectionScreen.PinForm;
                _machine.ConnectionScreen.PinButton.RemoveFromClassList("hidden");
                continue;
            }
            currentForm.Add(element);
        }
        _machine.ConnectionScreen.Next.clicked += callback;
    }
}