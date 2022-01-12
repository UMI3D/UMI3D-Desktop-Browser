using BrowserDesktop.UI;
using BrowserDesktop.UserPreferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public partial class Icon_E
    {
        public Icon_E(VisualElement root, string customStyleKey, string customStyleBackgroundKey = "") : 
            base(root, customStyleKey, customStyleBackgroundKey) { }

        public void ChangeBackground(string customStyleBackgroundKey)
        {
            CustomStyleBackgroundKey = customStyleBackgroundKey;
        }
    }

    public partial class Icon_E : AbstractGenericAndCustomElement
    {

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void OnApplyUserPreferences()
        {
            //if (IsEmpty)
            //    return;
            //ApplyCustomSize();
        }
    }
}