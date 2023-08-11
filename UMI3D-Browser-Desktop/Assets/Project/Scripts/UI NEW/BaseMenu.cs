using UnityEngine;
using UnityEngine.UIElements;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] protected UIDocument _uiDocument;

    private VisualElement _errorBox;

    protected virtual void Start()
    {
        Debug.Assert(_uiDocument != null);

        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        _uiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;

        SetupErrorBox();

        InitLocalisation();
    }

    protected void InitLocalisation()
    {
        var labels = _uiDocument.rootVisualElement.Query<TextElement>().ToList();

        foreach (var label in labels)
        {
            if (label.text == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(label.text);
            if (trad != null)
                label.text = trad;
        }
    }

    private void SetupErrorBox()
    {
        _errorBox = _uiDocument.rootVisualElement.Q("ErrorBox");
        _errorBox.Q<Button>("ButtonOk").clicked += () => _errorBox.AddToClassList("hidden");
        _errorBox.AddToClassList("hidden");
    }

    protected void OpenErrorBox(string message)
    {
        _errorBox.Q<TextElement>("Message").text = message;
        _errorBox.RemoveFromClassList("hidden");
    }
}