using System;
using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEngine.UIElements;

public abstract class MenuState
{
    protected MainMenu _machine;

    public MenuState(MainMenu machine)
    {
        _machine = machine;
    }

    public abstract void Enter();
    public abstract void Exit();
    public virtual void SetData(VisualElement elements, Action callback) { }
}