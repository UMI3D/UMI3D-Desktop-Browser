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
        public IList<string> CurrentValues { get; set; }
        public bool IsEmpty { get => Root == null || string.IsNullOrEmpty(Key) || IsValuesEmpty(); }

        public ICustomisableElement SetValues(params string[] values)
        {
            if (values.Length > 2) 
                throw new Exception("Icon_E must have less than 2 values.");
            Values.Clear();
            for (int i = 0; i < values.Length; ++i)
                Values.Add(values[i]);
            OnApplyUserPreferences();
            return this;
        }

        public void SelectCurrentValues(params int[] indexes)
        {
            for (int i = 0; i < indexes.Length; ++i)
            {
                int index = indexes[i];
                if (index >= Values.Count)
                    throw new Exception("index out of range in SwitchValue from Icon_E.");
                CurrentValues.Add(Values[index]);
            }
            
            OnApplyUserPreferences();
        }
        public void DeselectCurrentValues(params string[] values)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                string value = values[i];
                CurrentValues.Remove(value);
            }
        }
        public void DeselectAllCurrentValues()
        {
            CurrentValues.Clear();
        }
        public void DeselectLasCurrentValues()
        {
            CurrentValues.RemoveAt(CurrentValues.Count - 1);
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
        //public Icon_E(vi)

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
            CurrentValues = new List<string>();
            //ReadyToDisplay();
        }

        public  void GetUserPreferences()
        {

        }

        public override void OnApplyUserPreferences()
        {
            if (IsEmpty)
                return;
            string theme = "darkTheme"; //TODO to be replace by theme checked.
            UserPreferences.TextAndIconPref.ApplyIconPref(Root, Key, $"{theme}-{CurrentValues[0]}");
        }
    }
}