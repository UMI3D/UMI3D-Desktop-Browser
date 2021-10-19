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
using System.Collections.Generic;
using umi3d.cdk.interaction;
using UnityEngine;

namespace BrowserDesktop.Selection
{
    public class SelectionHighlighter : AbstractVisualCueHandler
    {
        /// <summary>
        /// Glowing effect shader
        /// </summary>
        [SerializeField]
        private Shader outlineShader;

        /// <summary>
        /// Stores shaders while the predicted object receives a glowing effect
        /// </summary>
        private Dictionary<Renderer, Shader> cachedShaders = new Dictionary<Renderer, Shader>();

        public override void ActivateSelectedVisualCue(InteractableContainer interactable)
        {
            var renderer = interactable.gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                cachedShaders.Add(renderer, renderer.material.shader);
                renderer.material.shader = outlineShader;
            }
        }

        public override void DeactivateSelectedVisualCue(InteractableContainer interactable)
        {
            var renderer = interactable.gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.shader = cachedShaders[renderer];
            }
            cachedShaders.Clear();
        }
    }
}
