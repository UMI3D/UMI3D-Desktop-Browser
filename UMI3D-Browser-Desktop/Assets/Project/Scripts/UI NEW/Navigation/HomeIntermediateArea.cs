using inetum.unityUtils;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class HomeIntermediateArea : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<HomeIntermediateArea, UxmlTraits> { }

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
            var custom = ve as HomeIntermediateArea;
        }
    }

    protected VisualElement _body = new VisualElement() { name = "UrlField" };
    protected Image _icon = new Image();
    protected TextField _url = new TextField();
    protected Button_C _submit = new Button_C();

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Base";
    public string StyleSheetPath_MainStyle => $"UI NEW/USS/Navigation/NavigationIntermediateArea";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
        this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("intermediate-area");
        _body.AddToClassList("url-field");
        _icon.AddToClassList("icon");
        _submit.AddToClassList("submit");
        _submit.AddToClassList("disabled");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _icon.vectorImage = Resources.Load<VectorImage>("UI NEW/Icons/world mini");
        _submit.VectorImage = Resources.Load<VectorImage>("UI NEW/Icons/validation_button_arrow 1");
        
        _url.SetPlaceholderText("myShortUrl.com");

        _body.Add(_icon);
        _body.Add(_submit);
        _body.Add(_url);

        Add(_body);
    }
}
