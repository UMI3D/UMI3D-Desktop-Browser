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
        protected static VisualElement s_dialogueBox { get; set; } = null;
        protected static Label_E s_title { get; set; } = null;
        protected static Label_E s_message { get; set; } = null;
        protected static VisualElement s_choiceBox { get; set; } = null;
        protected static Button_E s_choiceA { get; set; } = null;
        protected static Button_E s_choiceB { get; set; } = null;

        public static event Action<bool> ChoiceMade;
        private static Action<object> s_cursorSetMovement { get; set; } = null;
        private static Action<object> s_cursorUnsetMovement { get; set; } = null;

        protected static float s_width { get; set; } = default;
        protected static float s_height { get; set; } = default;
        protected static bool s_shouldCenter { get; set; } = false;
        private static string s_uxml => "UI/UXML/dialogueBox";
        private static string s_style => "UI/Style/DialogueBox/DialogueBox";

        public static void SetCursorMovementActions(Action<object> cursorSetMovement, Action<object> cursorUnsetMovement)
        {
            s_cursorSetMovement = cursorSetMovement;
            s_cursorUnsetMovement = cursorUnsetMovement;
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

            s_choiceB.Display();
            s_choiceA.Text = optionA;
            s_choiceB.Text = optionB;

            ChoiceMade += (choice) =>
            {
                s_cursorUnsetMovement(Instance);
                choiceCallback(choice);
                Instance.RemoveRootFromHierarchy();
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

            s_choiceB.Hide();
            s_choiceA.Text = optionA;

            ChoiceMade += (_) =>
            {
                s_cursorUnsetMovement(Instance);
                choiceCallback();
                Instance.RemoveRootFromHierarchy();
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

            ResetButtons();
            s_cursorSetMovement(Instance);

            s_choiceA.Clicked += () =>
            {
                ChoiceMade(true);
            };
            s_choiceB.Clicked += () =>
            {
                
                ChoiceMade(false);
            };

            s_title.value = title;
            s_message.value = message;
        }

        public static void DisplayFrom(UIDocument uiDocument)
        {
            if (Instance.IsDisplaying) return;
            else Instance.IsDisplaying = true;
            Instance.InsertRootTo(uiDocument.rootVisualElement);
            Center();
            s_shouldCenter = true;
        }

        protected void OnSizeChanged(GeometryChangedEvent e)
        {
            s_width = e.newRect.width;
            s_height = e.newRect.height;
            if (s_shouldCenter)
                Center();
        }

        protected static void Center()
        {
            s_dialogueBox.style.top = Screen.height / 2f - s_height / 2f;
            s_dialogueBox.style.left = Screen.width / 2f - s_width / 2f;
            s_shouldCenter = false;
        }

        protected static void ResetButtons()
        {
            ChoiceMade = null;
            s_choiceA.ResetClickedEvent();
            s_choiceB.ResetClickedEvent();
            VisualManipulator choiceAManip = s_choiceA.GetVisualManipulator(s_choiceA.Root);
            VisualManipulator choicBAManip = s_choiceB.GetVisualManipulator(s_choiceB.Root);

            choiceAManip.ApplyStyle(MouseBehaviour.MouseOut);
            choicBAManip.ApplyStyle(MouseBehaviour.MouseOut);
        }
    }

    public partial class DialogueBox_E : ISingleUI
    {
        private static DialogueBox_E s_instance;

        public static DialogueBox_E Instance
        {
            get
            {
                Create();
                return s_instance;
            }
        }

        private static void Create()
        {
            if (s_instance == null)
                s_instance = new DialogueBox_E();
        }
    }

    public partial class DialogueBox_E : View_E
    {
        private DialogueBox_E() :
            base(s_uxml, null, null)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            s_dialogueBox = Root.Q("dialogueBox");
            AddVisualStyle(s_dialogueBox, s_style, StyleKeys.DefaultBackground, new PopUpManipulator(s_dialogueBox));

            Label title = Root.Q<Label>("title");
            string titleStyle = "UI/Style/DialogueBox/DialogueBox_title";
            s_title = new Label_E(title, titleStyle, StyleKeys.DefaultText);

            Label message = Root.Q<Label>("message");
            string messageStyle = "UI/Style/DialogueBox/DialogueBox_message";
            s_message = new Label_E(message, messageStyle, StyleKeys.DefaultTextAndBorder);

            VisualElement choiceBox = Root.Q("choiceBox");
            string choiceBoxStyle = "UI/Style/DialogueBox/DialogueBox_choiceBox";
            AddVisualStyle(choiceBox, choiceBoxStyle, null);

            string choiceStyle = "UI/Style/DialogueBox/DialogueBox_Choice";
            Button choiceA = Root.Q<Button>("choiceA");
            s_choiceA = new Button_E(choiceA, choiceStyle, StyleKeys.Default);
            Button choiceB = Root.Q<Button>("choiceB");
            s_choiceB = new Button_E(choiceB, choiceStyle, StyleKeys.Default);

            s_dialogueBox.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);
        }
    }
}
