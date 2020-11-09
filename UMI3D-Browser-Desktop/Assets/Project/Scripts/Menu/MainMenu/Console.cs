/*
Copyright 2019 Gfi Informatique

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
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public class Console : MonoBehaviour
    {
        public PanelRenderer panelRenderer;

        VisualElement console;
        ScrollView consoleContainer;
        Button openConsoleButton;

        int consoleHeight;

        bool isDisplayed = true;

        void Start()
        {
            var root = panelRenderer.visualTree;
            openConsoleButton = root.Q<Button>("open-console-button");
            openConsoleButton.clickable.clicked += () => DisplayConsole(!isDisplayed);
            console = root.Q<VisualElement>("console");
            console.Q<Label>("version").text = umi3d.UMI3DVersion.version;
            consoleContainer = console.Q<ScrollView>("console-container");

            Application.logMessageReceived += HandleLog;

            console.RegisterCallback<GeometryChangedEvent>(e =>
            {
                consoleHeight = (int)console.layout.height;
            });

            DisplayConsole(false);
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            string buttonClassName = string.Empty;

            Label log = new Label { text = "\u00B7 " + logString };

            if (type != LogType.Warning)
                consoleContainer.Add(log);

            switch (type)
            {
                case LogType.Error:
                    log.AddToClassList("error-txt");
                    buttonClassName = "error-txt";
                    break;
                case LogType.Assert:
                    log.AddToClassList("error-txt");
                    buttonClassName = "error-txt";
                    break;
                case LogType.Warning:
                    log.AddToClassList("warning-txt");
                    buttonClassName = "warning-txt";
                    break;
                case LogType.Log:
                    log.AddToClassList("grey-txt");
                    buttonClassName = "grey-txt";
                    break;
                case LogType.Exception:
                    log.AddToClassList("error-txt");
                    buttonClassName = "error-txt";
                    break;
                default:
                    break;
            }

            openConsoleButton.ClearClassList();
            openConsoleButton.AddToClassList(buttonClassName);

        }
    
        void DisplayConsole(bool val)
        {
            isDisplayed = val;
            console.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}