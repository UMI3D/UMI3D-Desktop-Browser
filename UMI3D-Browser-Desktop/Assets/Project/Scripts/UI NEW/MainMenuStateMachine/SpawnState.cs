using UnityEngine;

public class SpawnState : MenuState
{
    public SpawnState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.Title.text = "Choose a spawn";

        _machine.NavigationScreen.Back.clicked += _machine.ToPlaceSpawn;

        _machine.NavigationScreen.Elements.Clear();
        _machine.NavigationScreen.AddElement("...", () => Debug.Log("TODO : Connection"));
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Back.clicked -= _machine.ToPlaceSpawn;
    }
}