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
using BrowserDesktop.UI.GenericElement;
using System;
using umi3d.common;
using UnityEngine;

namespace DesktopBrowser.UIControllers.Toolbox
{
    public sealed class Tools_UIController : Singleton<Tools_UIController>
    {
        [SerializeField]
        private GlobalUIController<ToolboxButtonGenericElement> controller = new GlobalUIController<ToolboxButtonGenericElement>();

        public static ToolboxButtonGenericElement CloneAndSetup(string name, string buttonClassOn, string buttonClassOff, bool isOn, Action buttonClicked)
        {
            return Instance.controller.CloneVisual().
                Setup(name, buttonClassOn, buttonClassOff, isOn, buttonClicked);
        }
    }

}