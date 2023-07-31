using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationItem_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<NavigationItem_C, UxmlTraits> { }

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
            var custom = ve as NavigationItem_C;
        }
    }

    protected Button_C _button = new Button_C();
    protected Image _image = new Image();
    protected Text_C _name = new Text_C() { name = "Name" };

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Navigation/NavigationItem";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("navigation-item");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _button.Add(_image);

        Add(_button);
        Add(_name);
    }

    public NavigationItem_C(string name, Texture image) : base()
    {
        _name.LocalisedText = name;
        _image.image = image;
    }

    public NavigationItem_C(string name) : base()
    {
        _name.LocalisedText = name;
        _image.vectorImage = Resources.Load<VectorImage>("UI NEW/Icons/picto_meta_world");
        _image.AddToClassList("icon");
    }

    public NavigationItem_C() : base()
    {
        _name.LocalisedText = "No name!";
        _image.vectorImage = Resources.Load<VectorImage>("UI NEW/Icons/picto_meta_world");
        _image.AddToClassList("icon");
    }
}
