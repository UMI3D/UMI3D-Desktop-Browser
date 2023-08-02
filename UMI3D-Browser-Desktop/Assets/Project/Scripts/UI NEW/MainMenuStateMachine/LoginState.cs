using umi3d.baseBrowser.connection;
using umi3d.cdk.menu;

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
        _machine.ConnectionScreen.Next.clicked += _machine.ToOrganisation;
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
        _machine.ConnectionScreen.Next.clicked -= _machine.ToOrganisation;
    }
}