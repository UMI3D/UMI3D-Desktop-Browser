using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class NotificationSettingsPanel_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<NotificationSettingsPanel_C, UxmlTraits> { }

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
            var custom = ve as NotificationSettingsPanel_C;
        }
    }

    private RadioButtonGroup _hideNotification = new RadioButtonGroup();

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Settings/Settings";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("settings-panel");

        _hideNotification.AddToClassList("setting");

        _hideNotification.AddToClassList("radio-2-elements");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _hideNotification.label = "Hide Notification";
        var notificationDisable = new RadioButton("Disable");
        _hideNotification.Add(new RadioButton("Enable"));
        _hideNotification.Add(notificationDisable);
        notificationDisable.value = true;

        Add(_hideNotification);
    }
}
