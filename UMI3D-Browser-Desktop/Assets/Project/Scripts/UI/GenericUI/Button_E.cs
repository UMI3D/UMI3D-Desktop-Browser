/*
Copyright 2019 - 2021 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Button_E
    {
        public string Text
        {
            get => m_button.text;
            set
            {
                if (value == m_button.text)
                    return;
                m_rawText = value;
                var (styleSO, _, _) = m_visualStylesMap[m_button];
                string newValue = m_rawText;
                if (styleSO != null)
                    newValue = m_styleApplicator.GetTextAfterFormatting(styleSO.TextFormat.NumberOfVisibleCharacter, value);
                m_button.text = newValue;
            }
        }

        protected string m_rawText { get; set; } = null;
        protected Button m_button => (Button)Root;
    }

    public partial class Button_E
    {
        public Button_E(Button button) :
            this(button, null, null)
        { }
        public Button_E(string styleResourcePath, StyleKeys keys) :
            this(new Button(), styleResourcePath, keys)
        { }
        public Button_E(Button button, string styleResourcePath, StyleKeys keys) :
            this(button, styleResourcePath, keys, null, true)
        { }
        public Button_E(string styleResourcePath, StyleKeys onKeys, StyleKeys offKeys, bool isOn = false) :
            this(new Button(), styleResourcePath, onKeys, offKeys, isOn)
        { }
        public Button_E(Button button, string styleResourcePath, StyleKeys onKeys, StyleKeys offKeys, bool isOn = false) :
            base(button, styleResourcePath, (isOn) ? onKeys : offKeys)
        {
            IsOn = isOn;
            AddStateKeys(Button, styleResourcePath, onKeys, offKeys);
        }
    }

    public partial class Button_E : IStateCustomisableElement
    {
        public bool IsOn { get; protected set; } = false;
        public Dictionary<Visual_E, (StyleKeys, StyleKeys)> StateKeys { get; protected set; } = null;

        public virtual void Toggle(bool value)
        {
            IsOn = value;

            foreach (Visual_E visual in StateKeys.Keys)
            {
                StyleKeys current = (IsOn) ? StateKeys[visual].Item1 : StateKeys[visual].Item2;
                visual.UpdateRootKeys(current);
            }
        }
        public void AddStateKeys(Visual_E view, string styleResourcePath, StyleKeys on, StyleKeys off)
        {
            if (view == null)
                throw new NullReferenceException("Visual null when trying to add state keys");
            if (StateKeys.ContainsKey(view))
                StateKeys[view] = (on, off);
            else
                StateKeys.Add(view, (on, off));
            view.UpdateRootStyleAndKeysAndManipulator(styleResourcePath, (IsOn) ? on : off);
        }
    }

    public partial class Button_E : IHoldableElement
    {
        public event Action ClickedDown;
        public event Action ClickedUp;
        public bool IsPressed { get; protected set; }

        private HoldableButtonManipulator m_holdableManipulator { get; set; } = null;

        public void OnClickedDown()
            => ClickedDown?.Invoke();
        public void OnClickedUp()
            => ClickedUp.Invoke();
        public void SetHoldableButton()
        {
            m_holdableManipulator = new HoldableButtonManipulator();
            m_holdableManipulator.ClickedDown = OnClickedDown;
            m_holdableManipulator.ClickedUp = OnClickedUp;
            m_button.AddManipulator(m_holdableManipulator);
            ClickedDown += () => IsPressed = true;
            ClickedUp += () => IsPressed = false;
        }
        public void UnSetHoldableButton()
        {
            throw new System.NotImplementedException();
        }
    }

    public partial class Button_E : IButtonCustomisableElement
    {
        public event Action Clicked;
        public Button_E Button => this;

        public void ResetClickedEvent()
            => Clicked = null;
        public void OnClicked()
            => Clicked?.Invoke();
        public void SetButton(string styleResourcePath, StyleKeys keys, Action clicked = null)
        {
            UpdateButtonStyle(styleResourcePath, keys);
            Clicked += clicked;
        }
        public void SetButton(VisualElement button, string styleResourcePath, StyleKeys keys, Action clicked = null)
            => throw new Exception("You shouldn't use this method here (see UpdateButton methods)");
        public void UpdateButtonStyle(string styleResourcePath, StyleKeys keys)
            => UpdateRootStyleAndKeysAndManipulator(styleResourcePath, keys);
        public void UpdateButtonKeys(StyleKeys keys)
            => UpdateRootKeys(keys);
    }

    public partial class Button_E : IIconCustomisableElement
    {
        public Visual_E Icon { get; protected set; } = null;

        public void AddIcon(VisualElement parent)
            => AddIcon(parent, null, null);
        public void AddIcon(VisualElement parent, string styleResourcePath, StyleKeys keys)
        {
            if (Icon != null)
                throw new Exception("Icon of the button is already affected");

            Icon = new Visual_E(parent, new VisualElement(), styleResourcePath, keys);
        }
        public void SetIcon(string styleResourcePath, StyleKeys keys)
            => SetIcon(Root.Q<VisualElement>("icon"), styleResourcePath, keys);
        public void SetIcon(VisualElement icon, string styleResourcePath, StyleKeys keys)
        {
            if (Icon != null)
                throw new Exception("Icon of the button is already affected");
            if (icon == null)
                throw new NullReferenceException("Icon null when trying to setup");

            Icon = new Visual_E(Root, icon, styleResourcePath, keys);
        }
        public void UpdateIconStyle(string styleResourcePath, StyleKeys keys)
        {
            if (Icon == null)
                throw new NullReferenceException("Icon null when trying to update");

            Icon.UpdateRootStyleAndKeysAndManipulator(styleResourcePath, keys);
        }
        public void UpdateIconKeys(StyleKeys keys)
        {
            if (Icon == null)
                throw new NullReferenceException("Icon null when trying to update");

            Icon.UpdateRootKeys(keys);
        }
    }

    public partial class Button_E : Visual_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            UpdateManipulator(Root, new ButtonManipulator());
            m_button.clicked += OnClicked;
            StateKeys = new Dictionary<Visual_E, (StyleKeys, StyleKeys)>();
        }

        public override void Reset()
        {
            base.Reset();
            m_button.clicked -= OnClicked;
            
            ClickedDown = null;
            ClickedUp = null;
        }

        protected override void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            base.ApplyStyle(styleSO, keys, style, mouseBehaviour);
            Text = m_rawText;
        }
    }
}