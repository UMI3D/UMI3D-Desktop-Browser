using System;
using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationCentralArea_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<NavigationCentralArea_C, UxmlTraits> { }

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
            var custom = ve as NavigationCentralArea_C;
        }
    }

    protected RadioButtonGroup _navigationButtonGroup = new RadioButtonGroup() { name = "NavigationButton" };
    protected ScrollView _elements = new ScrollView() { name = "Elements" };

    protected List<RadioButton> _navigationButtons = new List<RadioButton>();

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Navigation/NavigationCentralArea";

    protected override void AttachStyleSheet()
    {

    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        AddToClassList("main-area");
        _navigationButtonGroup.AddToClassList("menu-navigation");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _elements.mode = ScrollViewMode.Horizontal;


        CreateNavigation("Last", b =>
        {
            if (!b.newValue) return;
            Debug.Log("Last");
        });
        CreateNavigation("Favorite", b =>
        {
            if (!b.newValue) return;
            Debug.Log("Favorite");
        });
        _navigationButtons[0].value = true;

        _elements.Add(new NavigationItem_C("Tutorial"));
        _elements.Add(new NavigationItem_C("Tutorial"));
        _elements.Add(new NavigationItem_C("Tutorial"));

        Add(_navigationButtonGroup);
        Add(_elements);
    }

    protected void CreateNavigation(string label, EventCallback<ChangeEvent<bool>> callback)
    {
        var button = new RadioButton();
        button.label = label;
        button.RegisterValueChangedCallback(callback);

        _navigationButtons.Add(button);
        _navigationButtonGroup.Add(button);
    }
}
