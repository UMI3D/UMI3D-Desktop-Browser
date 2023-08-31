using inetum.unityUtils;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowsBar : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UiDocument;
    [SerializeField]
    protected WindowsManager m_WindowManager;

    private VisualElement m_WindowBar;
    private Button m_WindowBarButton;

    private void Start()
    {
        m_WindowBar = m_UiDocument.rootVisualElement.Q("WindowBar");

        SetupWhenToEnable();

        SetupShowHide();
        SetupButtons();
    }

    private void SetupButtons()
    {
        m_UiDocument.rootVisualElement.Q<Button>("Quit").clicked += () =>
        {
            QuittingManager.ApplicationIsQuitting = true;
            Application.Quit();
        };
        m_UiDocument.rootVisualElement.Q<Button>("Minimize").clicked += m_WindowManager.Minimize;
        m_UiDocument.rootVisualElement.Q<Button>("MinimizeWindow").clicked += m_WindowManager.Maximize;
    }

    private void SetupWhenToEnable()
    {
        m_WindowManager.FullScreenEnabled = value =>
        {
            if (value)
                m_UiDocument.rootVisualElement.Q("Header").RemoveFromClassList("hidden");
            else
                m_UiDocument.rootVisualElement.Q("Header").AddToClassList("hidden");
        };
    }

    private void SetupShowHide()
    {
        m_WindowBarButton = m_UiDocument.rootVisualElement.Q<Button>("DisplayWindowBar");
        m_WindowBarButton.clicked += () =>
        {
            if (m_WindowBar.GetClasses().Contains("hidden"))
            {
                m_WindowBarButton.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                m_WindowBar.RemoveFromClassList("hidden");
            }
            else
            {
                m_WindowBarButton.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                m_WindowBar.AddToClassList("hidden");
            }

        };
    }
}