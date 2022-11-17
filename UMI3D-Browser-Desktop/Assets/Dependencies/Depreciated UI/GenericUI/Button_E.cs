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
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class Button_E
    {

        public void SetTouchManipulator(TouchManipulator touchManipulator)
        {
            m_button.clickable = null;
            m_button.AddManipulator(touchManipulator);
            touchManipulator.ClickedDown += OnClickedDown;
            touchManipulator.ClickedUp += OnClickedUp;
        }


        public string Text
        {
            get => m_button.text;
            set
            {
                if (value == m_button.text)
                    return;
                m_rawText = value;
                var styleSO = GetVisualStyle(m_button);
                string newValue = m_rawText;
                if (styleSO == null) m_button.text = value;
                else
                    m_button.text = GetTextAfterFormatting(styleSO.TextFormat.NumberOfVisibleCharacter, value);
            }
        }

        protected string m_rawText { get; set; } = null;
        protected Button m_button => (Button)Root;

        public void AddIconInFront(Icon_E icon, string style, StyleKeys keys)
            => AddIconInFront(icon, style, keys, keys);
        public void AddIconInFront(Icon_E icon, string style, StyleKeys on, StyleKeys off)
        {
            if (icon == null)
                throw new NullReferenceException("Visual null when trying to add in front of button.");
            Add(icon);
            AddStateKeys(icon, style, on, off);
            m_clickableManipulator.MouseBehaviourChanged += icon.GetRootManipulator().OnMouseBehaviourChanged;
            LinkMouseBehaviourChanged(this, icon);
            GetRootManipulator().ProcessDuringBubbleUp = true;
        }
    }

    public partial class Button_E : IStateCustomisableElement
    {
        public bool IsOn { get; protected set; } = false;
        public Dictionary<View_E, (StyleKeys, StyleKeys)> StateKeys { get; protected set; } = null;

        public virtual void Toggle(bool value)
        {
            IsOn = value;

            foreach (View_E visual in StateKeys.Keys)
            {
                StyleKeys current = (IsOn) ? StateKeys[visual].Item1 : StateKeys[visual].Item2;
                visual.UpdateRootKeys(current);
            }
        }
        public void AddStateKeys(View_E view, string styleResourcePath, StyleKeys on, StyleKeys off)
        {
            if (view == null)
                throw new NullReferenceException("Visual null when trying to add state keys");
            if (StateKeys.ContainsKey(view))
                StateKeys[view] = (on, off);
            else
                StateKeys.Add(view, (on, off));
            view.UpdateRootStyleAndKeysAndManipulator(styleResourcePath, (IsOn) ? on : off);
        }
        public void UpdateStateKeys(View_E view, StyleKeys on, StyleKeys off)
        {
            if (view == null)
                throw new NullReferenceException("Visual null when trying to add state keys");
            if (StateKeys.ContainsKey(view))
                StateKeys[view] = (on, off);
            else
                throw new NullReferenceException($"StateKeys doesn't contain the view");
            view.UpdateRootKeys((IsOn) ? on : off);
        }
    }

    public partial class Button_E : IHoldableElement
    {
        public event Action ClickedDown;
        public event Action ClickedUp;
        public bool IsPressed { get; protected set; }

        private ClickableManipulator m_clickableManipulator { get; set; } = null;

        public void OnClickedDown()
            => ClickedDown?.Invoke();
        public void OnClickedUp()
            => ClickedUp.Invoke();
        public void SetHoldableButton()
        {
            m_clickableManipulator.ClickedDown += OnClickedDown;
            m_clickableManipulator.ClickedUp += OnClickedUp;
            ClickedDown += () => IsPressed = true;
            ClickedUp += () => IsPressed = false;
        }
        public void UnSetHoldableButton()
        {
            throw new System.NotImplementedException();
        }
    }

    public partial class Button_E : IClickableElement
    {
        public event Action Clicked;
        public Button_E Button => this;

        public void ResetClickedEvent()
            => Clicked = null;
        public void OnClicked()
            => Clicked?.Invoke();
    }

    public partial class Button_E : View_E
    {
        public Button_E() :
            this(new Button())
        { }
        public Button_E(Button button) :
            this(button, null, null)
        { }
        public Button_E(string partialStylePath, StyleKeys keys) :
            this(new Button(), partialStylePath, keys)
        { }
        public Button_E(Button button, string partialStylePath, StyleKeys keys, bool isOn = false) :
            this(button, partialStylePath, keys, keys, isOn)
        { }
        public Button_E(string partialStylePath, StyleKeys onKeys, StyleKeys offKeys, bool isOn = false) :
            this(new Button(), partialStylePath, onKeys, offKeys, isOn)
        { }
        public Button_E(Button button, string partialStylePath, StyleKeys onKeys, StyleKeys offKeys, bool isOn = false) :
            base(button, partialStylePath, (isOn) ? onKeys : offKeys)
        {
            IsOn = isOn;
            AddStateKeys(Button, partialStylePath, onKeys, offKeys);
        }

        public override void Add(View_E child)
        {
            base.Add(child);
            child.InsertRootTo(Root);
        }
        public override void Insert(int index, View_E child)
        {
            base.Insert(index, child);
            child.InsertRootAtTo(index, Root);
        }
        public override void Remove(View_E view)
        {
            base.Remove(view);
            view.RemoveRootFromHierarchy();
        }

        public override void Reset()
        {
            base.Reset();
            m_button.clicked -= OnClicked;

            Clicked = null;
            ClickedDown = null;
            ClickedUp = null;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var buttonManipulator = new ButtonManipulator();
            UpdateManipulator(Root, buttonManipulator);
            m_clickableManipulator = new ClickableManipulator();
            m_button.clickable = m_clickableManipulator;
            m_clickableManipulator.MouseBehaviourChanged += buttonManipulator.OnMouseBehaviourChanged;

            m_button.clicked += OnClicked;
            SetHoldableButton();
            
            StateKeys = new Dictionary<View_E, (StyleKeys, StyleKeys)>();
        }

        protected override CustomStyle_SO GetStyleSO(string resourcePath)
        {
            var path = (resourcePath == null) ? null : $"UI/Style/Buttons/{resourcePath}";
            return base.GetStyleSO(path);
        }

        protected override void ApplyStyle(CustomStyle_SO styleSO, StyleKeys keys, IStyle style, MouseBehaviour mouseBehaviour)
        {
            base.ApplyStyle(styleSO, keys, style, mouseBehaviour);
            Text = m_rawText;
        }
    }
}