using inetum.unityUtils;
using UnityEngine.UIElements;

public class ConnectionScreen : BaseScreen
{
    private TextField _mail;
    private TextField _password;
    private TextField _pin;

    private bool _usePin;

    public TextField Mail => _mail;
    public TextField Password => _password;
    public TextField Pin => _pin;

    public ConnectionScreen(VisualElement element) : base(element)
    {
        SetupNavigationButtons();
    }

    private void SetupNavigationButtons()
    {
        _mail = _root.Q<TextField>("Mail");
        _password = _root.Q<TextField>("Password");
        _pin = _root.Q<TextField>("Pin");

        _mail.SetPlaceholderText("example@gmail.com");
        _password.SetPlaceholderText("password");
        _pin.SetPlaceholderText("123456");

        var loginForm = _root.Q("FormLogin");
        var pinForm = _root.Q("FormPin");

        var loginButton = _root.Q<RadioButton>("Login");
        var pinButton = _root.Q<RadioButton>("Pin");

        loginButton.RegisterValueChangedCallback(e =>
        {
            _usePin = !e.newValue;
            if (e.newValue)
                loginForm.RemoveFromClassList("hidden");
            else
                loginForm.AddToClassList("hidden");
        });
        pinButton.RegisterValueChangedCallback(e =>
        {
            if (e.newValue)
                pinForm.RemoveFromClassList("hidden");
            else
                pinForm.AddToClassList("hidden");
        });

        loginButton.value = true;
        _usePin = false;
    }
}