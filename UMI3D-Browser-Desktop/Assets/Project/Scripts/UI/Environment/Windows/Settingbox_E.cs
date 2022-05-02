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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public partial class Settingbox_E
    {
        public Button_E Avatar { get; private set; } = null;
        public Button_E Sound { get; private set; } = null;
        public Button_E Mic { get; private set; } = null;
        public Button_E LeaveButton { get; private set; } = null;

        private static string s_uxml = "UI/UXML/settingbox";
        private static string s_style = "UI/Style/Settings/Settingbox";
        private static StyleKeys s_keys = new StyleKeys(null, "", null);
        private static string s_buttonStyle = "UI/Style/Settings/Buttons";
        private static StyleKeys s_buttonKeys = new StyleKeys(null, "", "");
        private static string s_buttonIconStyle = "UI/Style/Settings/Icons";

        private IEnumerator AnimeWindowVisibility(bool state)
        {
            yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
            Anime(Root, -Root.resolvedStyle.width, 0, 100, state, (elt, val) =>
            {
                elt.style.right = val;
            }); 
        }
    }

    public partial class Settingbox_E : ISingleUI
    {
        public static Settingbox_E Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new Settingbox_E();
                return s_instance;
            }
        }

        private static Settingbox_E s_instance;
    }

    public partial class Settingbox_E : AbstractWindow_E
    {
        public override void Reset()
        {
            base.Reset();
            Avatar.ResetClickedEvent();
            Sound.ResetClickedEvent();
            Mic.ResetClickedEvent();
            LeaveButton.ResetClickedEvent();
        }

        public override void Display()
        {
            UIManager.StartCoroutine(AnimeWindowVisibility(true));
            IsDisplaying = true;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            UIManager.StartCoroutine(AnimeWindowVisibility(false));
            IsDisplaying = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            StyleKeys titleKeys = new StyleKeys("", "", null);
            SetTopBar("Options", m_topBarStyle, titleKeys, false);

            var buttonbox = Root.Q("buttonbox");
            Button_E SetButton(string on, string off)
            {
                var button = new Button_E(s_buttonStyle, s_buttonKeys);
                var icon = new Visual_E();
                button.Add(icon);
                button.Toggle(true);
                StyleKeys onKeys = new StyleKeys(null, on, null);
                StyleKeys offKeys = new StyleKeys(null, off, null);
                button.AddStateKeys(icon, s_buttonIconStyle, onKeys, offKeys);
                LinkMouseBehaviourChanged(icon, button, false);

                return button;
            }

            Avatar = SetButton("avatarOn", "avatarOff");
            Avatar.InsertRootTo(buttonbox);
            Sound = SetButton("SoundOn", "SoundOff");
            Sound.InsertRootTo(buttonbox);
            Mic = SetButton("MicOn", "MicOff");
            Mic.InsertRootTo(buttonbox);
            LeaveButton = SetButton("Leave", null);
            LeaveButton.InsertRootTo(buttonbox);
        }

        private Settingbox_E() :
            base(s_uxml, s_style, s_keys)
        { }
    }
}
