using BrowserDesktop.UI;
using BrowserDesktop.UserPreferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesktopBrowser.UI.CustomElement
{
    public sealed partial class Icon_E : ICustomisableElement
    {
        public string Key { get; set; }
        public IList<string> Values { get; private set; }
        public string CurrentValue { get; set; }
        public bool IsEmpty { get => Root == null || string.IsNullOrEmpty(Key) || IsValuesEmpty(); }

        public ICustomisableElement SetValues(params string[] values)
        {
            if (values.Length > 2) 
                throw new Exception("Icon_E must have less than 2 values.");
            Values.Clear();
            for (int i = 0; i < values.Length; ++i)
                Values.Add(values[i]);
            return this;
        }

        public void SwitchValue(int index)
        {
            if (index >= Values.Count)
                throw new Exception("index out of range in SwitchValue from Icon_E.");
            CurrentValue = Values[index];
            OnApplyUserPreferences();
        }

        public override void Reset()
        {
            base.Reset();
            Key = null;
            Values.Clear();
        }
    }

    public partial class Icon_E
    {
        public Icon_E(VisualElement root) : base(root) { }

        private bool IsValuesEmpty()
        {
            if (Values.Count == 0) return true;
            else
            {
                foreach (string value in Values)
                    if (string.IsNullOrEmpty(value)) return true;
                return false;
            }
        }
    }

    public partial class Icon_E : AbstractGenericAndCustomElement
    {

        protected override void Initialize()
        {
            base.Initialize();
            Values = new List<String>();
            ReadyToDisplay();
        }

        public override void OnApplyUserPreferences()
        {
            if (IsEmpty)
                return;
            string theme = "darkTheme"; //TODO to be replace by theme checked.
            UserPreferences.TextAndIconPref.ApplyIconPref(Root, Key, $"{theme}-{CurrentValue}");
        }
    }
}