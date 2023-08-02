using umi3d.baseBrowser.connection;

public class OrganisationState : MenuState
{
    public OrganisationState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        // Skip because not implemented yet
        _machine.ToWorld();
        return;

        _machine.NavigationScreen.Show();
        _machine.NavigationScreen.IsHome = false;
        _machine.NavigationScreen.Title.text = "Connect to an Organisation";

        _machine.NavigationScreen.Back.clicked += () =>
        {
            BaseConnectionProcess.Instance.Leave();
            _machine.ToHome();
        };

        _machine.NavigationScreen.Elements.Clear();
        _machine.NavigationScreen.AddElement("Inetum", _machine.ToHighLevelLogin);
    }

    public override void Exit()
    {
        _machine.NavigationScreen.Hide();

        _machine.NavigationScreen.Back.clicked -= _machine.ToHome;
    }
}