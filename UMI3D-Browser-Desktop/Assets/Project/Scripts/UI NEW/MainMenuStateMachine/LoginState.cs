public class LoginState : MenuState
{
    public LoginState(MainMenu machine) : base(machine)
    {
    }

    public override void Enter()
    {
        _machine.ConnectionScreen.Show();

        _machine.ConnectionScreen.Back.clicked += _machine.ToHome;
        _machine.ConnectionScreen.Next.clicked += _machine.ToOrganisation;
    }

    public override void Exit()
    {
        _machine.ConnectionScreen.Hide();

        _machine.ConnectionScreen.Back.clicked -= _machine.ToHome;
        _machine.ConnectionScreen.Next.clicked -= _machine.ToOrganisation;
    }
}