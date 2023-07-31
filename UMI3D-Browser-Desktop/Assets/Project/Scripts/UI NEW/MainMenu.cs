using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private void Start()
    {
        Debug.Assert(_uiDocument != null);

        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        _uiDocument.rootVisualElement.Q<Label>("Version").text = BrowserDesktop.BrowserVersion.Version;
    }
}
