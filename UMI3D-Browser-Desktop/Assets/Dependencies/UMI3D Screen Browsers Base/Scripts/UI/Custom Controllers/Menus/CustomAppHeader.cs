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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomAppHeader : VisualElement, ICustomElement
{
    public virtual string StyleSheetMenuPath => $"USS/menu";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/appHeader";
    public virtual string USSCustomClassName => "app__header";

    public virtual string USSCustomClassWindowButton => $"{USSCustomClassName}-window__button";
    public virtual string USSCustomClassMinimize => $"{USSCustomClassName}-minimize__icon";
    public virtual string USSCustomClassMaximize => $"{USSCustomClassName}-maximize__icon";
    public virtual string USSCustomClassClose => $"{USSCustomClassName}-close__icon";

    public CustomButton Minimize;
    public VisualElement Minimize_Icon = new VisualElement { name = "mimimize-icon" };
    public CustomButton Maximize;
    public VisualElement Maximize_Icon = new VisualElement { name = "maximize-icon" };
    public CustomButton Close;
    public VisualElement Close_Icon = new VisualElement { name = "close-icon" };

    protected bool m_hasBeenInitialized;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);

        Minimize.AddToClassList(USSCustomClassWindowButton);
        Maximize.AddToClassList(USSCustomClassWindowButton);
        Close.AddToClassList(USSCustomClassWindowButton);
        Minimize_Icon.AddToClassList(USSCustomClassMinimize);
        Maximize_Icon.AddToClassList(USSCustomClassMaximize);
        Close_Icon.AddToClassList(USSCustomClassClose);

        Minimize.Size = ElementSize.Small;
        Maximize.Size = ElementSize.Small;
        Close.Size = ElementSize.Small;

        Close.Type = ButtonType.Danger;

        Add(Minimize);
        Minimize.Add(Minimize_Icon);
        Add(Maximize);
        Maximize.Add(Maximize_Icon);
        Add(Close);
        Close.Add(Close_Icon);
    }

    public virtual void Set()
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }
    }
}
