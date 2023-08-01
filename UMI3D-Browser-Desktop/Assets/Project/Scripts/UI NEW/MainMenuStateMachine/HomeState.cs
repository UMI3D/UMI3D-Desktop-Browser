using UnityEngine;

public class HomeState : MenuState
{
    public HomeState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.IsHome = true;

        _machine.NavigationScreen.Next.clicked += _machine.ToLogin;

        _machine.NavigationScreen.Elements.Clear();
        _machine.NavigationScreen.AddElement("Tutorials", _machine.ToLogin);
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Next.clicked -= _machine.ToLogin;
    }
}