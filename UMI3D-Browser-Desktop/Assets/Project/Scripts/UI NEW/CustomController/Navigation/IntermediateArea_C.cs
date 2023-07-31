using inetum.unityUtils;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.cdk.collaboration.UMI3DEnvironmentClient;

public class IntermediateArea_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<IntermediateArea_C, UxmlTraits> { }

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
            var custom = ve as IntermediateArea_C;
        }
    }

    private Button_C _backButton = new Button_C() { name = "Back" };

    private VisualElement _accountInfos = new VisualElement();
    private Image _userIcon = new Image();
    private VisualElement _userInfos = new VisualElement();
    private Text_C _userText = new Text_C();
    private Text_C _portalText = new Text_C();
    private Button_C _logout = new Button_C();

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Navigation/NavigationIntermediateArea";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("intermediate-area");
        _backButton.AddToClassList("button-base");
        _accountInfos.AddToClassList("account");
        _portalText.AddToClassList("text-important");
        _userInfos.AddToClassList("account-info");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();
        style.alignItems = Align.FlexStart;

        _backButton.LocaliseText = "< BACK";

        _userText.LocalisedText = "Connected as ...";
        _portalText.LocalisedText = "... PORTAL";
        _logout.LocaliseText = "Log out";

        _userInfos.Add(_userText);
        _userInfos.Add(_portalText);
        _userInfos.Add(_logout);

        _accountInfos.Add(_userIcon);
        _accountInfos.Add(_userInfos);

        Add(_backButton);
        Add(_accountInfos);
    }
}
