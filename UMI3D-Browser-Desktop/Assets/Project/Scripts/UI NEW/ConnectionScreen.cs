using System;
using UnityEngine.UIElements;

public class ConnectionScreen : BaseScreen
{
    public ConnectionScreen(VisualElement element) : base(element)
    {
        SetupNavigationButtons();
    }

    private void SetupNavigationButtons()
    {
        var loginForm = _root.Q("FormLogin");
        var pinForm = _root.Q("FormPin");

        var loginButton = _root.Q<RadioButton>("Login");
        var pinButton = _root.Q<RadioButton>("Pin");

        loginButton.RegisterValueChangedCallback(e =>
        {
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
    }
}