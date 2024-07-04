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
using SFB;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.cdk.menu.view;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.VisualElement;

namespace umi3d.commonScreen.Displayer
{
    public class UploadFileDisplayer : AbstractDisplayer
    {
        protected UploadInputMenuItem menuItem;

        /// <summary>
        /// Name of the parameter.
        /// </summary>
        public string Name => menuItem.Name;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.Exception"></exception>
        public override void SetMenuItem(AbstractMenuItem item)
        {
            if (item is not UploadInputMenuItem uploadItem)
            {
                throw new System.Exception("MenuItem must be a UploadInputMenuItem");
            }
            menuItem = uploadItem;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="forceUpdate"></param>
        public override void Display(bool forceUpdate = false)
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Hide()
        {
        }

        public void OpenFileBrowser()
        {
            ExtensionFilter[] extensions = (menuItem.authorizedExtensions == null || menuItem.authorizedExtensions.Count == 0) 
                ? null 
                : new[] { new ExtensionFilter("", menuItem.authorizedExtensions.ToArray()) };

            string[] paths = StandaloneFileBrowser.OpenFilePanel(
                    menuItem.Name, 
                    "", 
                    extensions, 
                    false
                );

            if (paths == null || paths.Length == 0)
            {
                return;
            }

            menuItem.dto.value = paths[0];
            menuItem.NotifyValueChange(paths[0]);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is UploadInputMenuItem) ? 2 : 0;
        }
    }
}
