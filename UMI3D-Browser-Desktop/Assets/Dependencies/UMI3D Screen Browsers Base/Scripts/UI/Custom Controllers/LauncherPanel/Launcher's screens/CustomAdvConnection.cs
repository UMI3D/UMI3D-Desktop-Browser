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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomAdvConnection : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomAdvConnection;

            custom.Set();
        }
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/advConnection";
    public override string USSCustomClassName => "advanced-connection";
    public virtual string USSCustomClassTextField__IP => $"{USSCustomClassName}__text-field__ip";
    public virtual string USSCustomClassTextField__Port => $"{USSCustomClassName}__text-field__port";
    public virtual string USSCustomClassButton_Navigation => $"{USSCustomClassName}__button__navigation";
    public virtual string USSCustomClassButton_Submit => $"{USSCustomClassName}__button__submit";

    public override string ShortScreenTitle => "Connect...";
    public CustomTextfield IP;
    public CustomTextfield Port;
    public CustomButton Button_Submit;

    public override void InitElement()
    {
        base.InitElement();

        IP.AddToClassList(USSCustomClassTextField__IP);
        Port.AddToClassList(USSCustomClassTextField__Port);
        Button_Back.AddToClassList(USSCustomClassButton_Navigation);
        Button_Submit.AddToClassList(USSCustomClassButton_Navigation);
        Button_Submit.AddToClassList(USSCustomClassButton_Submit);
        

        IP.label = "IP adress";
        Port.label = "Http port";
        IP.Direction = ElemnetDirection.Top;
        Port.Direction = ElemnetDirection.Top;
        IP.Size = ElementSize.Large;
        Port.Size = ElementSize.Large;

        Button_Submit.text = "Connect";
        Button_Submit.Type = ButtonType.Primary;
        Button_Submit.clicked += () =>
        {
            UpdataCurrentConnectionData(IP.value?.Trim(), Port.value?.Trim());
            StoreCurrentConnectionDataAndConnect?.Invoke(IP.value?.Trim(), Port.value?.Trim());
        };

        Add(IP);
        Add(Port);

        Header__Right.Add(Button_Submit);
    }

    public override void Set()
    {
        Set("Connection settings");
    }

    #region Implementation

    public Action<string, string> StoreCurrentConnectionDataAndConnect;
    public System.Action<string, string> UpdataCurrentConnectionData;
    public ServerPreferences.Data currentConnectionData;

    public void Display()
    {
        ElementExtensions.Display(this);
        IP.value = currentConnectionData.ip ?? "localhost";
        Port.value = currentConnectionData.port ?? "";
    }

    #endregion
}
