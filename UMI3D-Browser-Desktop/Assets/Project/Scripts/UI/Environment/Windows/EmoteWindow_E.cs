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

using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.ui.viewController;
using umi3dDesktopBrowser.ui.viewController;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Window that contains buttons for emotes triggering
/// </summary>
public class EmoteWindow_E : AbstractWindow_E, ISingleUI
{
    public List<Button_E> EmoteButtons { get; private set; } = new List<Button_E>();

    public static EmoteWindow_E Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new EmoteWindow_E();
            return s_instance;
        }
    }

    public bool AreButtonsLoaded { get; private set; } = false;

    private static EmoteWindow_E s_instance;

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

    public override void Reset()
    {
        base.Reset();
        foreach (var button in EmoteButtons)
            button.ResetClickedEvent();
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
        SetTopBar("Emotes");
        var menuLabel = Root.Q<Label>("windowName");
        menuLabel.text = "Emotes";
    }

    public void LoadButtons(List<Sprite> emotesToDisplay)
    {
        if (emotesToDisplay.Count == 0)
        {
            Hide();
            var menuLabel = Root.Q<Label>("windowName");
            menuLabel.text = "No emote available";
            return;
        }
        var buttonbox = Root.Q("buttonBox");

        Button_E SetButton(string on, string off, Sprite sprite)
        {
            var button = new Button_E("Square_m", StyleKeys.Bg_Border("menuOff", ""));
            var icon = new Icon_E();
            icon.Root.style.backgroundImage = new StyleBackground(sprite);
            button.AddIconInFront(icon, "Square2", null, null);
            button.Root.style.alignContent = Align.Center;
            button.Root.style.alignItems = Align.Center;
            button.InsertRootTo(buttonbox);
            button.ClickedUp += this.Hide;
            return button;
        }

        var i = 0;
        foreach (var sprite in emotesToDisplay)
        {
            EmoteButtons.Add(SetButton($"{sprite.name}_{i}", $"{sprite.name}_{i}", sprite));
            i++;
        }
        AreButtonsLoaded = true;
    }

    public Dictionary<Button_E, int> MapButtons(System.Action<Button_E> trigger)
    {
        int i = 0;
        var dict = new Dictionary<Button_E, int>();
        foreach (var button in EmoteButtons)
        {
            button.ClickedDown += delegate { trigger(button); };
            dict.Add(button, i);
            i++;
        }
        return dict;
    }

    private EmoteWindow_E() :
        base("emoteWindow", "EmoteWindow", StyleKeys.DefaultBackground)
    { }
}