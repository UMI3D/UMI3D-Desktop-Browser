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

using umi3d.commonScreen.Container;

namespace umi3d.commonScreen.menu
{
    public abstract class BaseSettingScreen_C : BaseMenuScreen_C
    {
        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/settings";

        public override LocalisationAttribute ShortScreenTitle => new LocalisationAttribute("Settings", "LauncherScreen", "Settings");
        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void InitElement()
        {
            base.InitElement();
            Add(ScrollView);
        }
    }
}
