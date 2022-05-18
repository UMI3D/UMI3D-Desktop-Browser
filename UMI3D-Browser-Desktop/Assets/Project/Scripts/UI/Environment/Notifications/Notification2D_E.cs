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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Notification2D_E
    {
        public Label_E Title { get; protected set; } = null;
        public Label_E Message { get; protected set; } = null;
        public ProgressBar_E ProgressBar { get; protected set; } = null;

        public void SetProgressBar()
        {

        }
    }

    public partial class Notification2D_E : Box_E
    {
        public Notification2D_E(string title, string message, int displayTime, Action OnDelete = null) :
            base("UI/UXML/notification2D", "Notification2D", StyleKeys.DefaultBackground)
        {
            Title.value = title;
            Message.value = message;
        }

        private void ProgressBarAnimation()
        {

        }

        protected override void Initialize()
        {
            base.Initialize();

            Title = new Label_E(QR<Label>("title"), "Notification2DTitle", StyleKeys.Default_Text_Border);
            Message = new Label_E(QR<Label>("message"), "Notification2DMessage", StyleKeys.DefaultText);
            ProgressBar = new ProgressBar_E("Notification2DProgressBar", StyleKeys.DefaultBackground);
        }
    }
}
