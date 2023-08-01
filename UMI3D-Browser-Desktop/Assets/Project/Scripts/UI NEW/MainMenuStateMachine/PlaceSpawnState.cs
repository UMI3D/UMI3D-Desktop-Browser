public class PlaceSpawnState : MenuState
{
    public PlaceSpawnState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.Title.text = "Choose a Place";

        _machine.NavigationScreen.Back.clicked += _machine.ToPlaceSpawn;

        _machine.NavigationScreen.Elements.Clear();
        _machine.NavigationScreen.AddElement("Esplanade", _machine.ToSpawn);
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Back.clicked -= _machine.ToPlaceSpawn;
    }
}