using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;
using inetum.unityUtils;

public class ConnectionMenu_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<ConnectionMenu_C, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="bag"></param>
        /// <param name="cc"></param>
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as ConnectionMenu_C;
        }
    }

    private VisualElement _backSection = new VisualElement();
    private Button_C _backButton = new Button_C() { name = "Back" };
    private Text_C _connectionTo = new Text_C();
    private Text_C _connectionName = new Text_C();

    private RadioButtonGroup _navigationButtons = new RadioButtonGroup();
    private RadioButton _loginButton = new RadioButton();
    private RadioButton _pinButton = new RadioButton();

    private VisualElement _form = new VisualElement();
    private VisualElement _formMail = new VisualElement();
    private TextField _mail = new TextField() { name = "Mail" };
    private TextField _password = new TextField() { name = "Password" };
    private VisualElement _formPin = new VisualElement();
    private TextField _pin = new TextField() { name = "Mail" };
    private ToggleButton_C _rememberMe = new ToggleButton_C();

    private Button_C _submit = new Button_C() { name = "Submit" };

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Connection/Connection";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("connection");
        _backSection.AddToClassList("back-section");
        _backButton.AddToClassList("button-base");
        _connectionName.AddToClassList("text-important");
        _navigationButtons.AddToClassList("menu-navigation");
        _submit.AddToClassList("button-base");
        _submit.AddToClassList("submit");
        _form.AddToClassList("form");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();
        InitHeader();
        InitNavigation();
        InitForm();
    }

    private void InitHeader()
    {
        _backButton.LocaliseText = "< BACK";
        _connectionTo.LocalisedText = "Connection to ";
        _connectionName.LocalisedText = "ToDo WORLD";

        _backSection.Add(_backButton);
        _backSection.Add(_connectionTo);
        _backSection.Add(_connectionName);

        Add(_backSection);
    }

    private void InitNavigation()
    {
        _loginButton.label = "Login";
        _pinButton.label = "PIN";

        _loginButton.RegisterValueChangedCallback(e =>
        {
            if (!e.newValue) return;
            _formMail.RemoveFromClassList("hidden");
            if (!_formPin.ClassListContains("hidden"))
                _formPin.AddToClassList("hidden");
        });
        _pinButton.RegisterValueChangedCallback(e =>
        {
            if (!e.newValue) return;
            if (!_formMail.ClassListContains("hidden"))
                _formMail.AddToClassList("hidden");
            _formPin.RemoveFromClassList("hidden");
        });

        _loginButton.value = true;
        _formMail.RemoveFromClassList("hidden");
        if (!_formPin.ClassListContains("hidden"))
            _formPin.AddToClassList("hidden");

        _navigationButtons.Add(_loginButton);
        _navigationButtons.Add(_pinButton);

        Add(_navigationButtons);
    }

    private void InitForm()
    {
        _mail.label = "Mail:";
        _mail.SetPlaceholderText("example@gmail.com");
        _password.label = "Password:";
        _password.isPasswordField = true;
        _password.SetPlaceholderText("Password");
        _pin.label = "Pin:";
        _pin.SetPlaceholderText("123456");
        _rememberMe.label = "Remember me";

        _submit.LocaliseText = "OK";

        _formMail.Add(_mail);
        _formMail.Add(_password);
        _formPin.Add(_pin);
        _form.Add(_formMail);
        _form.Add(_formPin);
        _form.Add(_rememberMe);

        Add(_form);
        Add(_submit);
    }
}
