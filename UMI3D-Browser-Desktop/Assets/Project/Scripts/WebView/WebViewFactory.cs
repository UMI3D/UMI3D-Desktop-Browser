/*
Copyright 2019 - 2023 Inetum

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

using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using UnityEngine;

namespace BrowserDesktop
{
    public class WebViewFactory : AbstractWebViewFactory
    {
        public List<GameObject> templates = new();

        public Dictionary<int, (GameObject, bool)> templatesQueue = new();

        protected override void Awake()
        {
            base.Awake();

            foreach (var template in templates)
            {
                templatesQueue.Add(template.GetInstanceID(), (template, true));
            }
        }

        protected override AbstractUMI3DWebView CreateWebView()
        {
            GameObject go = new GameObject("WebView");
            return go.AddComponent<WebView>();
        }

        public GameObject GetTemplate()
        {
            foreach (var entry in templatesQueue)
            {
                if (entry.Value.Item2)
                {
                    templatesQueue[entry.Key] = (entry.Value.Item1, false);
                    return entry.Value.Item1;
                }
            }

            return null;
        }

        public void ReleaseObject(int templateId)
        {
            if (templatesQueue.ContainsKey(templateId))
            {
                templatesQueue[templateId] = (templatesQueue[templateId].Item1, true);
            }
        }
    }
}