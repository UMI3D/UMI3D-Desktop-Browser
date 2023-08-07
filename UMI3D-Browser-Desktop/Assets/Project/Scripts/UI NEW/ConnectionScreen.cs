using inetum.unityUtils;
using System;
using umi3d.common.interaction;
using UnityEngine.UIElements;

public class ConnectionScreen : BaseScreen
{
    private VisualElement _loginForm;
    private VisualElement _pinForm;

    private TextField _mail;
    private TextField _password;
    private TextField _pin;

    private RadioButton _loginButton;
    private RadioButton _pinButton;

    private bool _usePin;

    public VisualElement LoginForm => _loginForm;
    public VisualElement PinForm => _pinForm;

    public TextField Mail => _mail;
    public TextField Password => _password;
    public TextField Pin => _pin;

    public RadioButton LoginButton => _loginButton;
    public RadioButton PinButton => _pinButton;

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

        _loginForm = _root.Q("FormLogin");
        _pinForm = _root.Q("FormPin");

        _loginButton = _root.Q<RadioButton>("Login");
        _pinButton = _root.Q<RadioButton>("Pin");

        _loginButton.RegisterValueChangedCallback(e =>
        {
            _usePin = !e.newValue;
            if (e.newValue)
                _loginForm.RemoveFromClassList("hidden");
            else
                _loginForm.AddToClassList("hidden");
        });
        _pinButton.RegisterValueChangedCallback(e =>
        {
            if (e.newValue)
                _pinForm.RemoveFromClassList("hidden");
            else
                _pinForm.AddToClassList("hidden");
        });

        _loginButton.value = true;
        _usePin = false;
    }

    public void Clear()
    {
        _loginForm.Clear();
        _pinForm.Clear();
    }
}