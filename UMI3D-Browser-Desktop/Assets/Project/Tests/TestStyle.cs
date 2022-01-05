using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IUIDisplay
{
    public DisplayStyle Display { get; }
    public float Opacity { get; }
    public Visibility Visibility { get; }
}

public interface IUIPosition 
{
    public float Bottom { get; }
    public float Left { get; }
    public Position Position { get; }
    public float Rght { get; }
    public float Top { get; }
}

public interface IUISize
{
    public StyleFloat MaxHeight { get; }
    public StyleFloat MaxWidth { get; }
    public StyleFloat MinHeight { get; }
    public StyleFloat MinWidth { get; }
    public float Height { get; }
    public float Width { get; }
}

public interface IUIMarginAndPadding
{
    public float MarginBottom { get; }
    public float MarginLeft { get; }
    public float MarginRight { get; }
    public float MarginTop { get; }
    public float PaddingBottom { get; }
    public float PaddingLeft { get; }
    public float PaddingRight { get; }
    public float PaddingTop { get; }
}

public interface IUIText
{
    public Color Color { get; }
    public float LetterSpacing { get; }
    public float FontSize { get; }
    public TextOverflow TextOverflow { get; }
    public Font UnityFont { get; }
    public FontDefinition UnityFontDefinition { get; }
    public FontStyle UnityFontStyleAndWeight { get; }
    public float UnityParagraphSpacing { get; }
    public TextAnchor UnityTextAlign { get; }
    public TextOverflowPosition UnityTextOverflowPosition { get; }
    public Color UnityTextOutlineColor { get; }
    public float UnityTextOutlineWidth { get; }
    public WhiteSpace WhiteSpace { get; }
    public float WordSpacing { get; }
}

public interface IUIBackground
{
    public Color BackgroundColor { get; }
    public Background BackgroundImage { get; }
    public Color UnityBackgroundImageTintColor { get; }
    public ScaleMode UnityBackgroundScaleMode { get; }
    public int UnitySliceBottom { get; }
    public int UnitySliceLeft { get; }
    public int UnitySliceRight { get; }
    public int UnitySliceTop { get; }
}

public interface IUIBorder
{
    public Color BorderBottomColor { get; }
    public float BorderBottomLeftRadius { get; }
    public float BorderBottomRightRadius { get; }
    public float BorderBottomWidth { get; }
    public Color BorderLeftColor { get; }
    public float BorderLeftWidth { get; }
    public Color BorderRightColo { get; }
    public float BorderRightWidth { get; }
    public Color BorderTopColor { get; }
    public float BorderTopLeftRadius { get; }
    public float BorderTopRightRadius { get; }
    public float BorderTopWidth { get; }
}

[Serializable]
public class UIDisplay : IUIDisplay
{
    [SerializeField]
    private DisplayStyle m_display;
    [SerializeField]
    private float m_opacity;
    [SerializeField]
    private Visibility m_visibility;

    public DisplayStyle Display => throw new NotImplementedException();

    public float Opacity => throw new NotImplementedException();

    public Visibility Visibility => throw new NotImplementedException();
}

[Serializable]
public class UIPosition : IUIPosition
{
    [SerializeField]
    private float m_bottom;
    [SerializeField]
    private float m_left;
    [SerializeField]
    private Position m_position;
    [SerializeField]
    private float m_right;
    [SerializeField]
    private float m_top;

    public float Bottom => throw new NotImplementedException();

    public float Left => throw new NotImplementedException();

    public Position Position => throw new NotImplementedException();

    public float Rght => throw new NotImplementedException();

    public float Top => throw new NotImplementedException();
}

[Serializable]
public class UISize : IUISize
{
    [SerializeField]
    private StyleFloat m_maxHeight;
    [SerializeField]
    private StyleFloat m_maxWidth;
    [SerializeField]
    private StyleFloat m_minHeight;
    [SerializeField]
    private StyleFloat m_minWidth;
    [SerializeField]
    private float m_height;
    [SerializeField]
    private float m_width;

    public StyleFloat MaxHeight => throw new NotImplementedException();

    public StyleFloat MaxWidth => throw new NotImplementedException();

    public StyleFloat MinHeight => throw new NotImplementedException();

    public StyleFloat MinWidth => throw new NotImplementedException();

    public float Height => throw new NotImplementedException();

    public float Width => throw new NotImplementedException();
}

[Serializable]
public class UIMarginAndPadding : IUIMarginAndPadding
{
    [SerializeField]
    private float m_marginBottom;
    [SerializeField]
    private float m_marginLeft;
    [SerializeField]
    private float m_marginRight;
    [SerializeField]
    private float m_marginTop;
    [SerializeField]
    private float m_paddingBottom;
    [SerializeField]
    private float m_paddingLeft;
    [SerializeField]
    private float m_paddingRight;
    [SerializeField]
    private float m_paddingTop;

    public float MarginBottom => throw new NotImplementedException();

    public float MarginLeft => throw new NotImplementedException();

    public float MarginRight => throw new NotImplementedException();

    public float MarginTop => throw new NotImplementedException();

    public float PaddingBottom => throw new NotImplementedException();

    public float PaddingLeft => throw new NotImplementedException();

    public float PaddingRight => throw new NotImplementedException();

    public float PaddingTop => throw new NotImplementedException();
}

[Serializable]
public class UIText : IUIText
{
    [SerializeField]
    private Color m_color;
    [SerializeField]
    private float m_letterSpacing;
    [SerializeField]
    private float m_fontSize;
    [SerializeField]
    private TextOverflow m_textOveflow;
    [SerializeField]
    private Font m_unityFont;
    [SerializeField]
    private FontDefinition m_unityFontDefinition;
    [SerializeField]
    private FontStyle m_unityFontStyleAndWeight;
    [SerializeField]
    private float m_unityParagraphSpacing;
    [SerializeField]
    private TextAnchor m_unityTextAlign;
    [SerializeField]
    private TextOverflowPosition m_unityTextOverflowPosition;
    [SerializeField]
    private Color m_unityTextOutlineColor;
    [SerializeField]
    private float m_unityTextOutlineWidth;
    [SerializeField]
    private WhiteSpace m_WhiteSpace;
    [SerializeField]
    private float m_wordSpacing;


    public Color Color => throw new NotImplementedException();

    public float LetterSpacing => throw new NotImplementedException();

    public float FontSize => throw new NotImplementedException();

    public TextOverflow TextOverflow => throw new NotImplementedException();

    public Font UnityFont => throw new NotImplementedException();

    public FontDefinition UnityFontDefinition => throw new NotImplementedException();

    public FontStyle UnityFontStyleAndWeight => throw new NotImplementedException();

    public float UnityParagraphSpacing => throw new NotImplementedException();

    public TextAnchor UnityTextAlign => throw new NotImplementedException();

    public TextOverflowPosition UnityTextOverflowPosition => throw new NotImplementedException();

    public Color UnityTextOutlineColor => throw new NotImplementedException();

    public float UnityTextOutlineWidth => throw new NotImplementedException();

    public WhiteSpace WhiteSpace => throw new NotImplementedException();

    public float WordSpacing => throw new NotImplementedException();
}

[Serializable]
public class UIBackground : IUIBackground
{
    [SerializeField]
    private Color m_backgroundColor;
    [SerializeField]
    private Background m_backgroundImage;
    [SerializeField]
    private Color m_unityBackgroundImageTintColo;
    [SerializeField]
    private int m_unitySliceBottom;
    [SerializeField]
    private int m_unitySliceLeft;
    [SerializeField]
    private int m_unitySliceRight;
    [SerializeField]
    private int m_unitySliceTop;

    public Color BackgroundColor => throw new NotImplementedException();

    public Background BackgroundImage => throw new NotImplementedException();

    public Color UnityBackgroundImageTintColor => throw new NotImplementedException();

    public ScaleMode UnityBackgroundScaleMode => throw new NotImplementedException();

    public int UnitySliceBottom => throw new NotImplementedException();

    public int UnitySliceLeft => throw new NotImplementedException();

    public int UnitySliceRight => throw new NotImplementedException();

    public int UnitySliceTop => throw new NotImplementedException();
}

[Serializable]
public class UIBorder : IUIBorder
{
    [SerializeField]
    private Color m_borderBottomColor;
    [SerializeField]
    private float m_borderBottomLeftRadius;
    [SerializeField]
    private float m_borderBottomRightRadius;
    [SerializeField]
    private float m_borderBottomWidth;
    [SerializeField]
    private Color m_borderLeftColor;
    [SerializeField]
    private float m_borderLeftWidth;
    [SerializeField]
    private Color m_borderRightColo;
    [SerializeField]
    private float m_borderRightWidth;
    [SerializeField]
    private Color m_borderTopColor;
    [SerializeField]
    private float m_borderTopLeftRadius;
    [SerializeField]
    private float m_borderTopRightRadius;
    [SerializeField]
    private float m_borderTopWidth;


    public Color BorderBottomColor => throw new NotImplementedException();

    public float BorderBottomLeftRadius => throw new NotImplementedException();

    public float BorderBottomRightRadius => throw new NotImplementedException();

    public float BorderBottomWidth => throw new NotImplementedException();

    public Color BorderLeftColor => throw new NotImplementedException();

    public float BorderLeftWidth => throw new NotImplementedException();

    public Color BorderRightColo => throw new NotImplementedException();

    public float BorderRightWidth => throw new NotImplementedException();

    public Color BorderTopColor => throw new NotImplementedException();

    public float BorderTopLeftRadius => throw new NotImplementedException();

    public float BorderTopRightRadius => throw new NotImplementedException();

    public float BorderTopWidth => throw new NotImplementedException();
}

public class TestStyle : MonoBehaviour
{

    public TestStyle2 testStyle2 = new TestStyle2();


    [Serializable]
    public class TestStyle2 
    {
        public UIDisplay UIDisplay = new UIDisplay();
        public UIPosition UIPosition = new UIPosition();
        public UISize UISize = new UISize();
        public UIMarginAndPadding UIMarginAndPadding = new UIMarginAndPadding();
        public UIText UIText = new UIText();
        public UIBackground UIBackground = new UIBackground();
        public UIBorder UIBorder = new UIBorder();

        #region fields
        public Align m_alignContent;

        public Align m_alignItems;

        public Align m_alignSelf;

        public Color m_backgroundColor;

        public Background m_backgroundImage;

        public Color m_borderBottomColor;

        public float m_borderBottomLeftRadius;

        public float m_borderBottomRightRadius;

        public float m_borderBottomWidth;

        public Color m_borderLeftColor;

        public float m_borderLeftWidth;

        public Color m_borderRightColor;

        public float m_borderRightWidth;

        public Color m_borderTopColor;

        public float m_borderTopLeftRadius;

        public float m_borderTopRightRadius;

        public float m_borderTopWidth;

        public float m_bottom;

        public Color m_color;

        public DisplayStyle m_display;

        public StyleFloat m_flexBasis;

        public FlexDirection m_flexDirection;

        public float m_flexGrow;

        public float m_flexShrink;

        public Wrap m_flexWrap;

        public float m_fontSize;

        public float m_height;

        public Justify m_justifyContent;

        public float m_left;

        public float m_letterSpacing;

        public float m_marginBottom;

        public float m_marginLeft;

        public float m_marginRight;

        public float m_marginTop;

        public StyleFloat m_maxHeight;

        public StyleFloat m_maxWidth;

        public StyleFloat m_minHeight;

        public StyleFloat m_minWidth;

        public float m_opacity;

        public float m_paddingBottom;

        public float m_paddingLeft;

        public float m_paddingRight;

        public float m_paddingTop;

        public Position m_position;

        public float m_right;

        public TextOverflow m_textOverflow;

        public float m_top;

        public Color m_unityBackgroundImageTintColor;

        public ScaleMode m_unityBackgroundScaleMode;

        public Font m_unityFont;

        public FontDefinition m_unityFontDefinition;

        public FontStyle m_unityFontStyleAndWeight;

        public float m_unityParagraphSpacing;

        public int m_unitySliceBottom;

        public int m_unitySliceLeft;

        public int m_unitySliceRight;

        public int m_unitySliceTop;

        public TextAnchor m_unityTextAlign;

        public Color m_unityTextOutlineColor;

        public float m_unityTextOutlineWidth;

        public TextOverflowPosition m_unityTextOverflowPosition;

        public Visibility m_visibility;

        public WhiteSpace m_whiteSpace;

        public float m_width;

        public float m_wordSpacing;

        #endregion



        //public Align alignContent => throw new NotImplementedException();
        //public Align alignSelf => throw new NotImplementedException();

        //public Align alignItems => throw new NotImplementedException();
        //public Justify justifyContent => throw new NotImplementedException();




        //public StyleFloat flexBasis => throw new NotImplementedException();

        //public FlexDirection flexDirection => throw new NotImplementedException();

        //public float flexGrow => throw new NotImplementedException();

        //public float flexShrink => throw new NotImplementedException();

        //public Wrap flexWrap => throw new NotImplementedException();

    }
}
