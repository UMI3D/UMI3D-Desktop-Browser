public class WorldState : MenuState
{
    public WorldState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.Title.text = "Connect to a World";

        _machine.NavigationScreen.Back.clicked += _machine.ToOrganisation;

        _machine.NavigationScreen.Elements.Clear();
        _machine.NavigationScreen.AddElement("Inetum World", _machine.ToPlaceSpawn);
        _machine.NavigationScreen.AddElement("Inetum Madrid", _machine.ToPlaceSpawn);
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Back.clicked -= _machine.ToOrganisation;
    }
}