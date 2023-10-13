using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReportBug : MonoBehaviour
{
    [SerializeField]
    protected UIDocument m_UiDocument;

    private Dropdown_C m_DropdownOften;
    private TextElement m_Message;
    private TextElement m_Mail;

    private Mail m_MailSender = new Mail();

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

        m_Mail = m_UiDocument.rootVisualElement.Q<TextElement>("AskMail");

        m_DropdownOften = m_UiDocument.rootVisualElement.Q<Dropdown_C>("HowOftenHappen");
        m_DropdownOften.choices = new List<string>() { "Once", "Some time", "Always" };
        m_DropdownOften.value = "Once";

        m_Message = m_UiDocument.rootVisualElement.Q<TextElement>("Message");

        m_UiDocument.rootVisualElement.Q<Button>("ButtonSend").clicked += () =>
        {
            m_MailSender.Send("Bug", "Mail : " + m_Mail + "\nBug happen : " + m_DropdownOften.value + "\n" +  m_Message.text);
        };
    }
}