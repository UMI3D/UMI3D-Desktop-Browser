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

        private IEnumerator DisplayWithoutAnimation()
        {
            yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
            Root.style.right = 0;
        }

        private IEnumerator HideWithoutAnimation()
        {
            yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
            Root.style.right = -Root.resolvedStyle.width;
        }

        //private IEnumerator AnimeWindowVisibility(bool state)
        //{
        //    yield return new WaitUntil(() => Root.resolvedStyle.width > 0f);
        //    Anime(Root, -Root.resolvedStyle.width, 0, 100, state, (elt, val) =>
        //    {
        //        elt.style.right = val;
        //    }); 
        //}
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

    public partial class Settingbox_E : AbstractPinnedWindow_E
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
            UIManager.StartCoroutine(DisplayWithoutAnimation());
            Console_E.Instance.Hide();
            IsDisplaying = true;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            UIManager.StartCoroutine(HideWithoutAnimation());
            IsDisplaying = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SetTopBar("Options");

            var buttonbox = Root.Q("buttonbox");
            Button_E SetButton(string on, string off)
            {
                var button = new Button_E("Square_m", StyleKeys.Bg_Border("menuOff", ""));
                button.AddIconInFront(new Icon_E(), "Square2", StyleKeys.Bg(on), StyleKeys.Bg(off));
                button.Root.style.alignContent = Align.Center;
                button.Root.style.alignItems = Align.Center;
                button.InsertRootTo(buttonbox);
                return button;
            }

            Avatar = SetButton("avatarOn", "avatarOff");
            Sound = SetButton("SoundOn", "SoundOff");
            Mic = SetButton("MicOn", "MicOff");
            LeaveButton = SetButton("Leave", "Leave");
        }

        private Settingbox_E() :
            base("settingbox", "Settingbox", StyleKeys.DefaultBackground)
        { }
    }
}
