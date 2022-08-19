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
using System.Collections.Generic;
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
        public Button_E AllMic { get; private set; } = null;
        public Button_E LeaveButton { get; private set; } = null;
        private Coroutine m_coroutine;

        public ListView_E<User_item_E> UserList { get; private set; } = null;

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
                if (s_instance == null) s_instance = new Settingbox_E();
                return s_instance;
            }
        }
        public static void DestroySingleton()
        {
            if (s_instance == null) return;
            if (Instance.m_coroutine != null) UIManager.StopCoroutine(Instance.m_coroutine);
            s_instance.Destroy();
            s_instance = null;
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
            m_coroutine = UIManager.StartCoroutine(DisplayWithoutAnimation());
            Console_E.Instance.Hide();
            EmoteWindow_E.Instance.Hide();
            IsDisplaying = true;
            OnDisplayedOrHiddenTrigger(true);
        }

        public override void Hide()
        {
            m_coroutine = UIManager.StartCoroutine(HideWithoutAnimation());
            IsDisplaying = false;
            OnDisplayedOrHiddenTrigger(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SetTopBar("Options");

            //userLabel userButton

            var userbox = Root.Q<ListView>("userbox");

            Debug.Assert(userbox != null, "User box is null");

            UserList = new ListView_E<User_item_E>(userbox, User_item_E.height, User_item_E.Make);

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
            AllMic = SetButton("allMicOff", "allMicOff");
            LeaveButton = SetButton("Leave", "Leave");
        }

        private Settingbox_E() :
            base("settingbox", "Settingbox", StyleKeys.DefaultBackground)
        { }
    }


    public class User_item_Event : UnityEngine.Events.UnityEvent<User_item_E> { }

    public class User_item_E : ListView_item_E
    {
        public static User_item_Event OnItemBinded = new User_item_Event();
        public static User_item_Event OnItemUnbinded = new User_item_Event();

        ItemData data;

        public Button_E Avatar => data?.Avatar;
        public Button_E Sound => data?.Sound;
        public Button_E Mic => data?.Mic;

        public override void Bind(VisualElement element)
        {
            base.Bind(element);
            data = element.userData as ItemData;
            OnItemBinded.Invoke(this);
        }

        public override void Unbind(VisualElement element)
        {
            OnItemUnbinded.Invoke(this);
            data = null;
            base.Unbind(element);
        }

        public static VisualElement Make()
        {
            VisualElement item = _GetVisualRoot(@"UI/UXML/userItem");
            item.style.marginBottom = 10f;
            var buttonbox = item.Q("userButton");
            Button_E SetButton(string on, string off)
            {
                var button = new Button_E("Square_m", StyleKeys.Bg_Border("menuOff", ""));
                button.AddIconInFront(new Icon_E(), "Square2", StyleKeys.Bg(on), StyleKeys.Bg(off));
                button.Root.style.alignContent = Align.Center;
                button.Root.style.alignItems = Align.Center;
                button.InsertRootTo(buttonbox);
                return button;
            }

            Button_E Avatar = SetButton("avatarOn", "avatarOff");
            Button_E Sound = SetButton("SoundOn", "SoundOff");
            Button_E Mic = SetButton("MicOn", "MicOff");

            item.userData = new ItemData(Avatar, Sound, Mic);

            return item;
        }


        /// <summary>
        /// Get the VisualTreeAsset from [resourcePath] and return the first child of the new VisualElement.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        static VisualElement _GetVisualRoot(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) throw new Exception("resourcePath null or empty");
            VisualTreeAsset visualTA = Resources.Load<VisualTreeAsset>(resourcePath);
            if (visualTA == null) throw new NullReferenceException($"[{resourcePath}] return a null visual tree asset.");
            Debug.Assert(visualTA.CloneTree().childCount == 1, $"[{resourcePath}] must have a single visual as root.");
            IEnumerator<VisualElement> iterator = visualTA.CloneTree().Children().GetEnumerator();
            iterator.MoveNext();
            return iterator.Current;
        }

        public static int height { get; } = 65;


        public class ItemData
        {
            public ItemData(Button_E avatar, Button_E sound, Button_E mic)
            {
                Avatar = avatar;
                Sound = sound;
                Mic = mic;
            }

            public Button_E Avatar { get; private set; } = null;
            public Button_E Sound { get; private set; } = null;
            public Button_E Mic { get; private set; } = null;
        }

    }
}
