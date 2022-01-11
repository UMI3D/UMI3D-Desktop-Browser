using BrowserDesktop.UI;
using DesktopBrowser.UI.GenericElement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public partial class ToolboxWindow_E
    {
        public UnityEvent UnPinedButtonPressed { get; set; }

        private IScrollable scrollViewData;
    }

    public partial class ToolboxWindow_E
    {
        public ToolboxWindow_E(VisualElement root) : base(root) { }

        private void OnCloseButtonPressed()
        {
            //Collapse evrything
            //hide the window
            Root.style.display = DisplayStyle.None;
            Debug.Log($"display of icon = {(icon as VisualElement).resolvedStyle.display}");
        }
        private Icon_E icon;
    }

    public partial class ToolboxWindow_E : AbstractGenericAndCustomElement
    {
        protected override void Initialize()
        {
            base.Initialize();

            scrollViewData = new AbstractScrollView_E(Root.Q("scrollView"))
            {
            }.InitFromSrollViewToProperties();

            icon = new Icon_E(Root.Q("icon"), "square-radius");

            //new Button_GE(Root.Q("closeButton"))
            //{
            //    OnClicked = () => { OnCloseButtonPressed(); },
            //    IconPref = "square-button"
            //}.SetIcon("", "", true);

            //new Button_GE(Root.Q("unPinedButton"))
            //{
            //    OnClicked = () => { UnPinedButtonPressed.Invoke(); },
            //    IconPref = "square-button"
            //}.SetIcon("", "", true);
        }

        public override void OnApplyUserPreferences()
        {
            
        }
    }
}