public abstract class MenuState
{
    protected MainMenu _machine;

    public MenuState(MainMenu machine)
    {
        _machine = machine;
    }

    public abstract void Enter();
    public abstract void Exit();
}