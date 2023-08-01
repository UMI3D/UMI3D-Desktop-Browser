public class HighLevelLoginState : MenuState
{
    public HighLevelLoginState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.ConnectionScreen.Show();

        _machine.ConnectionScreen.Back.clicked += _machine.ToOrganisation;
        _machine.ConnectionScreen.Next.clicked += _machine.ToWorld;
    }

    public override void Exit()
    {
        _machine.ConnectionScreen.Hide();

        _machine.ConnectionScreen.Back.clicked -= _machine.ToOrganisation;
        _machine.ConnectionScreen.Next.clicked -= _machine.ToWorld;
    }
}