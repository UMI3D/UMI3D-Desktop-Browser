using System;
using System.Collections.Generic;
using umi3d.commonScreen;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationMenu_C : BaseVisual_C
{
    public new class UxmlFactory : UxmlFactory<NavigationMenu_C, UxmlTraits> { }

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
            var custom = ve as NavigationMenu_C;
        }
    }

    public enum Menu
    {
        Home,
        Organisation,
        Worlds,
        Spawns
    }

    private VisualElement _intermediateArea = new IntermediateArea_C() { name = "IntermediateArea" };
    private VisualElement _mainArea = new VisualElement() { name = "MainArea" };
    private NavigationCentralArea_C _centralArea = new NavigationCentralArea_C() { name = "CentralArea" };
    private Button_C _buttonSettings = new Button_C() { name = "Settings" };
    private Button_C _buttonStorage = new Button_C() { name = "Storage" };

    private Dictionary<Menu, Tuple<VisualElement, NavigationCentralArea_C>> _menus;

    #region USS
    public override string StyleSheetPath_MainTheme => $"UI NEW/USS/Base";
    public string StyleSheetPath_MainStyle => $"UI NEW/USS/Navigation/Navigation";

    protected override void AttachStyleSheet()
    {
        base.AttachStyleSheet();
        this.AddStyleSheetFromPath(StyleSheetPath_MainStyle);
    }

    protected override void AttachUssClass()
    {
        base.AttachUssClass();
        _mainArea.AddToClassList("main-area");
        _centralArea.AddToClassList("central-area");
        _buttonSettings.AddToClassList("menu-button");
        _buttonStorage.AddToClassList("menu-button");
    }
    #endregion

    protected override void InitElement()
    {
        base.InitElement();

        _buttonSettings.VectorImage = Resources.Load<VectorImage>("UI NEW/Icons/settings");
        _buttonSettings.LocaliseText = new LocalisationAttribute("Settings", "LauncherScreen", "Settings");
        _buttonStorage.VectorImage = Resources.Load<VectorImage>("UI NEW/Icons/storage4");
        _buttonStorage.LocaliseText = new LocalisationAttribute("Storage", "LauncherScreen", "Storage");

        _mainArea.Add(_buttonSettings);
        _mainArea.Add(_centralArea);
        _mainArea.Add(_buttonStorage);

        Add(_intermediateArea);
        Add(_mainArea);
    }
}
