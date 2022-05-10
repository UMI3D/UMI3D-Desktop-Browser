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
using System.Collections;
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

        protected static float s_dialogueboxWidth => s_dialogueBox.resolvedStyle.width;
        protected static float s_dialogueboxHeight => s_dialogueBox.resolvedStyle.height;

        protected static float s_centerRadius = 20f;
        protected static bool s_isCentered
            => s_dialogueBox.resolvedStyle.top == (Screen.height - s_dialogueboxHeight) / 2f
            && s_dialogueBox.resolvedStyle.left == (Screen.width - s_dialogueboxWidth) / 2f;
        protected static bool s_canBeCentered 
            => (s_dialogueBox.resolvedStyle.top <= (Screen.height - s_dialogueboxHeight) / 2f + s_centerRadius && s_dialogueBox.resolvedStyle.top >= (Screen.height - s_dialogueboxHeight) / 2f - s_centerRadius) 
            && (s_dialogueBox.resolvedStyle.left <= (Screen.width - s_dialogueboxWidth) / 2f + s_centerRadius && s_dialogueBox.resolvedStyle.left >= (Screen.width - s_dialogueboxWidth) / 2f - s_centerRadius);

        public static void SetCursorMovementActions(Action<object> cursorSetMovement, Action<object> cursorUnsetMovement)
        {
            s_cursorSetMovement = cursorSetMovement;
            s_cursorUnsetMovement = cursorUnsetMovement;
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
        public void Setup(string title, string message, string optionA, string optionB, Action<bool> choiceCallback)
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
        /// Sets up the dialogue box for one choice.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="optionA"></param>
        /// <param name="optionB"></param>
        /// <param name="choiceCallback"></param>
        /// <param name="marginForTitleBar"></param>
        public void Setup(string title, string message, string optionA, Action choiceCallback)
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
            ResetButtons();
            s_cursorSetMovement(Instance);

            s_choiceA.Clicked += () => ChoiceMade(true);
            s_choiceB.Clicked += () => ChoiceMade(false);

            s_title.value = title;
            s_message.value = message;
        }

        public void DisplayFrom(UIDocument uiDocument)
        {
            if (Instance.IsDisplaying) return;
            else Instance.IsDisplaying = true;
            Instance.InsertRootTo(uiDocument.rootVisualElement);
            s_dialogueBox.style.visibility = Visibility.Hidden;
            Instance.OnDisplayedOrHiddenTrigger(true);
            UIManager.StartCoroutine(Center());
        }

        protected void OnBackgroundSizeChanged(GeometryChangedEvent e)
        {
            if (s_canBeCentered) UIManager.StartCoroutine(Center());
        }

        protected static IEnumerator Center()
        {
            yield return new WaitUntil(() => s_dialogueboxWidth > 0f && s_dialogueboxHeight > 0f);
            s_dialogueBox.style.top = (Screen.height - s_dialogueboxHeight) / 2f;
            s_dialogueBox.style.left = (Screen.width - s_dialogueboxWidth) / 2f;
            s_dialogueBox.style.visibility = Visibility.Visible;
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
        public static DialogueBox_E Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new DialogueBox_E();
                return s_instance;
            }
        }

        private static DialogueBox_E s_instance;
    }

    public partial class DialogueBox_E : View_E
    {
        private DialogueBox_E() :
            base("UI/UXML/dialogueBox", null, null)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            s_dialogueBox = Root.Q("dialogueBox");
            AddVisualStyle(s_dialogueBox, "UI/Style/DialogueBox/DialogueBox", StyleKeys.DefaultBackground);
            var manipulator = new WindowManipulator(s_dialogueBox);
            s_dialogueBox.AddManipulator(manipulator);
            manipulator.MouseUp += () =>
            {
                bool shouldCenter()
                => !s_isCentered &&
                s_canBeCentered;

                if (shouldCenter()) UIManager.StartCoroutine(Center());
            };

            s_title = new Label_E(Root.Q<Label>("title"), "Title2", StyleKeys.DefaultText);
            s_message = new Label_E(Root.Q<Label>("message"), "Corps1", StyleKeys.DefaultText);

            VisualElement choiceBox = Root.Q("choiceBox");
            string choiceBoxStyle = "UI/Style/DialogueBox/DialogueBox_choiceBox";
            AddVisualStyle(choiceBox, choiceBoxStyle, null);

            s_choiceA = new Button_E(QR<Button>("choiceA"), "Rectangle1", StyleKeys.Default);
            s_choiceB = new Button_E(QR<Button>("choiceB"), "Rectangle1", StyleKeys.Default);

            Root.RegisterCallback<GeometryChangedEvent>(OnBackgroundSizeChanged);
        }
    }
}
