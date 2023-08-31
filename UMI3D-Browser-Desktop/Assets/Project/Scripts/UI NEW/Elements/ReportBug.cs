using UnityEngine;
using UnityEngine.UIElements;

public class ReportBug : MonoBehaviour
{
    [SerializeField]
    protected UIDocument m_UiDocument;

    private void Start()
    {
        var reportBug = m_UiDocument.rootVisualElement.Q("ReportBug");

        m_UiDocument.rootVisualElement.Q<Button>("ButtonBug").clicked += () =>
        {
            reportBug.RemoveFromClassList("hidden");
        };

        reportBug.Q<Button>("ButtonClose").clicked += () =>
        {
            reportBug.AddToClassList("hidden");
        };
    }
}