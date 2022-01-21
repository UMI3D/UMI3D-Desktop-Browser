using umi3DBrowser.UICustomStyle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;




public class TestStyle : MonoBehaviour
{

    //public TestStyle2 testStyle2 = new TestStyle2();

    [Serializable]
    public class TestStyle2 
    {
        public UIDisplay UIDisplay = new UIDisplay();
        public UIPosition UIPosition = new UIPosition();
        public UISize UISize = new UISize();
        public UIMarginAndPadding UIMarginAndPadding = new UIMarginAndPadding();
        //public UIText UIText = new UIText();
        public UIBackground UIBackground = new UIBackground();
        public UIBorder UIBorder = new UIBorder();

        #region fields
        /*
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
        */
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



    public CustomStyle_SO CustomStyle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Debug.Log($"max height = [{testStyle2.UISize.MaxHeight.ToString()}]");
            Debug.Log($"max height = [{CustomStyle.UISize.MaxHeight.ToString()}]");
        }
    }
}
