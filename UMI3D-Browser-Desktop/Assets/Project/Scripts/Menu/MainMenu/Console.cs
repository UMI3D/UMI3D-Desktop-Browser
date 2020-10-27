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

using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{

    public class Console : MonoBehaviour
    {
        public PanelRenderer panelRenderer;

        Label consolePreview;

        void Start()
        {
            consolePreview = panelRenderer.visualTree.Q<Label>("console-footer");
            consolePreview.text = "";

            panelRenderer.visualTree.Q<Label>("version-footer").text = umi3d.UMI3DVersion.version;

            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            consolePreview.ClearClassList();
            bool display = true;
            
            switch (type)
            {
                case LogType.Error:
                    consolePreview.AddToClassList("error-txt");
                    break;
                case LogType.Assert:
                    consolePreview.AddToClassList("error-txt");
                    break;
                case LogType.Warning:
                    consolePreview.AddToClassList("warning-txt");
                    display = false;
                    break;
                case LogType.Log:
                    display = false;
                    break;
                case LogType.Exception:
                    consolePreview.AddToClassList("error-txt");
                    break;
                default:
                    break;
            }
            if(display)
                consolePreview.text = logString;
        }
    }
}