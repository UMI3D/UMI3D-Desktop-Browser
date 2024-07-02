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
            string[] paths = StandaloneFileBrowser.OpenFilePanel(menuItem.Name, "", "", false);

            if (paths == null || paths.Length == 0)
            {
                return;
            }

            FileUploader.AddFileToUpload(paths[0]);
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
