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
using umi3d.baseBrowser.ui.viewController;
using umi3d.cdk.menu;
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Notification2D_E
    {
        public event Action Complete;
        public Label_E Title { get; protected set; } = null;
        public Label_E Message { get; protected set; } = null;
        public Button_E ChoiceA { get; protected set; }
        public Button_E ChoiceB { get; protected set; }
        public ProgressBar_E ProgressBar { get; protected set; } = null;
        private Action<bool> m_callback;

        protected void ResetButtons()
        {
            ChoiceA.ResetClickedEvent();
            ChoiceB.ResetClickedEvent();
            VisualManipulator choiceAManip = ChoiceA.GetVisualManipulator(ChoiceA.Root);
            VisualManipulator choicBAManip = ChoiceB.GetVisualManipulator(ChoiceB.Root);

            choiceAManip.ApplyStyle(MouseBehaviour.MouseOut);
            choicBAManip.ApplyStyle(MouseBehaviour.MouseOut);
        }
    }

    public partial class Notification2D_E : Box_E
    {
        public Notification2D_E(string title, string message, int displayTime, Action OnDelete = null) :
            base("UI/UXML/notification2D", "Notification2D", StyleKeys.DefaultBackground)
        {
            Title.value = title;
            Message.value = message;
            ChoiceA.Hide();
            ChoiceB.Hide();
            if (displayTime > 0)
                ProgressBar.LaunchTimeBar(displayTime, false);
            else
                ProgressBar.Hide();
        }

        public Notification2D_E(string title, string message, string choiceA, string choiceB, Action<bool> callback) :
            base("UI/UXML/notification2D", "Notification2D", StyleKeys.DefaultBackground)
        {
            ResetButtons();
            Title.value = title;
            Message.value = message;
            ChoiceA.Display();
            ChoiceA.Text = choiceA;
            ChoiceB.Display();
            ChoiceB.Text = choiceB;
            m_callback = callback;
            ProgressBar.Hide();
        }

        protected override void Initialize()
        {
            base.Initialize();

            Title = new Label_E(QR<Label>("title"), "TitleNotification2D", StyleKeys.Default_Text_Border);
            Message = new Label_E(QR<Label>("message"), "CorpsNotification2D", StyleKeys.DefaultText);
            ChoiceA = new Button_E(QR<Button>("choiceA"), "DialogueBoxChoice", StyleKeys.Default);
            ChoiceB = new Button_E(QR<Button>("choiceB"), "DialogueBoxChoice", StyleKeys.Default);
            ChoiceA.Clicked += () => m_callback?.Invoke(true);
            ChoiceB.Clicked += () => m_callback?.Invoke(false);
            ProgressBar = new ProgressBar_E(QR("progressBar"), "ProgressBarNotification2D", null);
            ProgressBar.Complete += () => Complete?.Invoke();
            ProgressBar.SetBar("ProgressBarNotification2D", StyleKeys.DefaultBackground);
        }
    }
}
