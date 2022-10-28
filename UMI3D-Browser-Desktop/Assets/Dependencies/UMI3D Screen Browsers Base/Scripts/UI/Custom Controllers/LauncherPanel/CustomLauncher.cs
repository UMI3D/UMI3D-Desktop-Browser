/*
Copyright 2019 - 2022 Inetum

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
using BeardedManStudios.Forge.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomLauncher : CustomMenuContainer<LauncherScreens>
{
    public new class UxmlTraits : CustomMenuContainer<LauncherScreens>.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomLauncher;

            custom.Set
            (
                m_currentScreen.GetValueFromBag(bag, cc),
                m_displayHeader.GetValueFromBag(bag, cc),
                m_version.GetValueFromBag(bag, cc)
            );
        }
    }

    public virtual string StyleSheetLauncherPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/launcher";
    public virtual string USSCustomClassLauncher => "launcher";

    public CustomHomeScreen Home;
    public CustomSessionScreen Sessions;
    public CustomAdvConnection ConnectionSettings;
    public CustomLibraryScreen Libraries;
    public CustomSettingsContainer Settings;

    public CustomButtonGroup<LauncherScreens> NavigationButtons;

    public override void InitElement()
    {
        base.InitElement();

        try
        {
            this.AddStyleSheetFromPath(StyleSheetLauncherPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassLauncher);

        NavigationButtons.ValueEnumChanged += value => CurrentScreen = value;
        NavigationButtons.EnumValue = LauncherScreens.Home; 

        Home.BackButtonCkicked = () => RemoveScreenFromStack();
        Sessions.BackButtonCkicked = () => RemoveScreenFromStack();
        ConnectionSettings.BackButtonCkicked = () => RemoveScreenFromStack();
        Libraries.BackButtonCkicked = () => RemoveScreenFromStack();
        Settings.BackButtonCkicked = () => RemoveScreenFromStack();

        Libraries.WrongLibraryPathFound += (pathes) =>
        {
            var dialogueBox = CreateDialogueBox();
            dialogueBox.Type = DialogueboxType.Default;
            dialogueBox.Title = "Some libraries are not found";
            dialogueBox.Message = "Here is the list of the path that are not found:";
            var pathList = CreateText();
            pathList.text = "\"" + string.Join("\"\n\"", pathes);
            pathList.style.whiteSpace = WhiteSpace.Normal;
            dialogueBox.Add(pathList);
            dialogueBox.ChoiceAText = "Show me";
            dialogueBox.Callback = (index) =>
            {
                AddScreenToStack = LauncherScreens.Libraries;
            };
            dialogueBox.AddToTheRoot(this);
        };

        Navigation_ScrollView.Add(NavigationButtons);
    }

    public override void Set() => Set(LauncherScreens.Home, false, null);

    public virtual void Set(LauncherScreens screen, bool displayHeader, string version)
    {
        Set(displayHeader, version);

        CurrentScreen = screen;
    }

    protected override void RemoveAllScreen()
    {
        Home.RemoveFromHierarchy();
        Sessions.RemoveFromHierarchy();
        ConnectionSettings.RemoveFromHierarchy();
        Libraries.RemoveFromHierarchy();
        Settings.RemoveFromHierarchy();
    }

    protected override void GetScreen(LauncherScreens screenEnum, out CustomMenuScreen screen)
    {
        switch (screenEnum)
        {
            case LauncherScreens.Home:
                screen = Home;
                break;
            //case LauncherScreens.ConnectionSettings:
            //    screen = ConnectionSettings;
            //    break;
            //case LauncherScreens.Session:
            //    screen = Sessions;
            //    break;
            case LauncherScreens.Libraries:
                screen = Libraries;
                break;
            case LauncherScreens.Settings:
                screen = Settings;
                break;
            default:
                screen = null;
                break;
        }
    }

    protected override void GetScreenAndButton(LauncherScreens screenEnum, out CustomMenuScreen screen, out CustomButton button)
    {
        GetScreen(screenEnum, out screen);
        button = null;
    }

    protected abstract CustomDialoguebox CreateDialogueBox();
    protected abstract CustomText CreateText();

#region Implementation

    public Action<bool> Connect
    {
        set
        {
            Home.connect = value;
        }
    }
    public Action<string, string> StoreCurrentConnectionDataAndConnect
    {
        set
        {
            ConnectionSettings.StoreCurrentConnectionDataAndConnect = value;
            Sessions.StoreCurrentConnectionDataAndConnect = value;
        }
    }
    public Action<string, Action<MasterServerResponse.Server>> SendDataSession
    {
        set
        {
            Sessions.SendDataSession = value;
        }
    }

    public umi3d.baseBrowser.preferences.ServerPreferences.ServerData CurrentServer
    {
        set
        {
            Home.currentServer = value;
        }
    }
    public List<umi3d.baseBrowser.preferences.ServerPreferences.ServerData> SavedServers
    {
        set
        {
            Home.savedServers = value;
            Home.InitSavedServer();
        }
    }
    public umi3d.baseBrowser.preferences.ServerPreferences.Data CurrentConnectionData
    {
        set
        {
            currentConnectionData = value;
            ConnectionSettings.currentConnectionData = value;
            Sessions.UpdataCurrentConnectionData = UpdataCurrentConnectionData;
            ConnectionSettings.UpdataCurrentConnectionData = UpdataCurrentConnectionData;
        }
    }
    public void InitLibraries() => Libraries.InitLibraries();

    protected umi3d.baseBrowser.preferences.ServerPreferences.Data currentConnectionData;

    /// <summary>
    /// Gets the url and port written by users and stores them.
    /// </summary>
    public void UpdataCurrentConnectionData(string ip, string port)
    {
        currentConnectionData.environmentName = null;
        currentConnectionData.ip = ip;
        currentConnectionData.port = port;
    }
#endregion
}
