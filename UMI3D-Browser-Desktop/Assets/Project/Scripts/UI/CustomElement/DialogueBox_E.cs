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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class DialogueBox_E
    {
        public static DialogueBox_E Instance
        {
            get
            {
                Create();
                return m_instance;
            }
        }

        protected static VisualElement m_dialogueBox { get; set; } = null;
        protected static Label_E m_title { get; set; } = null;
        protected static Label_E m_message { get; set; } = null;
        protected static VisualElement m_choiceBox { get; set; } = null;
        protected static Button_E m_choiceA { get; set; } = null;
        protected static Button_E m_choiceB { get; set; } = null;

        public static Action<bool> ChoiceCallback { get; private set; }
        private static Action<object> m_cursorSetMovement { get; set; } = null;
        private static Action<object> m_cursorUnsetMovement { get; set; } = null;

        protected static float m_width { get; set; } = default;
        protected static float m_height { get; set; } = default;
        protected static bool m_shouldCenter { get; set; } = false;
        private static DialogueBox_E m_instance;
        private static string m_uxml => "UI/UXML/dialogueBox";
        private static string m_style => "UI/Style/DialogueBox/DialogueBox";
        private static StyleKeys m_keys => new StyleKeys(null, "", "");
    }

    public partial class DialogueBox_E
    {
        public static void SetCursorMovementActions(Action<object> cursorSetMovement, Action<object> cursorUnsetMovement)
        {
            m_cursorSetMovement = cursorSetMovement;
            m_cursorUnsetMovement = cursorUnsetMovement;
        }

        /// <summary>
        /// Sets up the dialogue box for two choices. And displayed it at the root of the [uiDocument].
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="optionB"></param>
        /// <param name="choiceCallback"></param>
        /// <param name="uiDoc"></param>
        public static void Setup(string title, string message, string optionA, string optionB, Action<bool> choiceCallback, UIDocument uiDoc)
        {
            Setup(title, message, optionA, optionB, choiceCallback);
            DisplayFrom(uiDoc);
        }

        /// <summary>
        /// Sets up the dialogue box for two choices.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="optionB"></param>
        /// <param name="choiceCallback"></param>
        /// <param name="marginForTitleBar"></param>
        public static void Setup(string title, string message, string optionA, string optionB, Action<bool> choiceCallback)
        {
            Setup(title, message);

            m_choiceB.Display();
            m_choiceA.Text = optionA;
            m_choiceB.Text = optionB;

            ChoiceCallback = (b) =>
            {
                m_cursorUnsetMovement(Instance);
                choiceCallback(b);
                Instance.Remove();
            };
        }

        /// <summary>
        /// Sets up the dialogue box for one choice. And displayed it at the root of the [uiDocument].
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="choiceCallback"></param>
        /// <param name="uiDoc"></param>
        public static void Setup(string title, string message, string optionA, Action choiceCallback, UIDocument uiDoc)
        {
            Setup(title, message, optionA, choiceCallback);
            DisplayFrom(uiDoc);
        }

        /// <summary>
        /// Sets up the dialogue box for one choice.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="optionB"></param>
        /// <param name="choiceCallback"></param>
        /// <param name="marginForTitleBar"></param>
        public static void Setup(string title, string message, string optionA, Action choiceCallback)
        {
            Setup(title, message);

            m_choiceB.Hide();
            m_choiceA.Text = optionA;

            ChoiceCallback = (b) =>
            {
                m_cursorUnsetMovement(Instance);
                choiceCallback();
                Instance.Remove();
            };
        }

        /// <summary>
        /// Sets ups elements which do not depends on the number of choices.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static void Setup(string title, string message)
        {
            Create();

            m_cursorSetMovement(Instance);

            m_choiceA.OnClicked = () =>
            {
                ChoiceCallback(true);
            };
            m_choiceB.OnClicked = () =>
            {
                ChoiceCallback(false);
            };

            m_title.value = title;
            m_message.value = message;
        }

        public static void DisplayFrom(UIDocument uiDocument)
        {
            if (Instance.Displayed) return;
            else Instance.Displayed = true;
            Instance.InsertRootTo(uiDocument.rootVisualElement);
            Center();
            m_shouldCenter = true;
        }

        protected void OnSizeChanged(GeometryChangedEvent e)
        {
            m_width = e.newRect.width;
            m_height = e.newRect.height;
            if (m_shouldCenter)
                Center();
        }

        protected static void Center()
        {
            m_dialogueBox.style.top = Screen.height / 2f - m_height / 2f;
            m_dialogueBox.style.left = Screen.width / 2f - m_width / 2f;
            m_shouldCenter = false;
        }
    }

    public partial class DialogueBox_E : Visual_E
    {
        private DialogueBox_E() :
            base(m_uxml, null, null)
        { }

        private static void Create()
        {
            if (m_instance == null)
                m_instance = new DialogueBox_E();
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_dialogueBox = Root.Q("dialogueBox");
            AddVisualStyle(m_dialogueBox, m_style, m_keys, new PopUpManipulator(m_dialogueBox));

            Label title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/DialogueBox/DialogueBox_title";
            StyleKeys titleKeys = new StyleKeys("", null, null);
            m_title = new Label_E(title, titleStyle, titleKeys);

            Label message = Root.Q<Label>("message");
            string messageStyle = "UI/Style/DialogueBox/DialogueBox_message";
            StyleKeys messageKeys = new StyleKeys("", null, "");
            m_message = new Label_E(message, messageStyle, messageKeys);

            VisualElement choiceBox = Root.Q("choiceBox");
            string choiceBoxStyle = "UI/Style/DialogueBox/DialogueBox_choiceBox";
            StyleKeys choiceBoxeKeys = new StyleKeys();
            AddVisualStyle(choiceBox, choiceBoxStyle, choiceBoxeKeys);

            string choiceStyle = "UI/Style/DialogueBox/DialogueBox_Choice";
            StyleKeys choiceKeys = new StyleKeys("", "", "");
            Button choiceA = Root.Q<Button>("choiceA");
            m_choiceA = new Button_E(choiceA, choiceStyle, choiceKeys);
            Button choiceB = Root.Q<Button>("choiceB");
            m_choiceB = new Button_E(choiceB, choiceStyle, choiceKeys);

            m_dialogueBox.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);
        }
    }
}
