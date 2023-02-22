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
using umi3d.baseBrowser.preferences;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class ServerButton_C : Button_C
    {
        public new class UxmlFactory : UxmlFactory<ServerButton_C, UxmlTraits> { }

        public virtual string StyleSheetMenuPath => $"USS/menu";
        public virtual string StyleSheetServerPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/serverButton";

        public virtual string USSCustomClassServerButton => "server-button";
        public virtual string USSCutomClassIcon => $"{USSCustomClassServerButton}__icon";
        public virtual string USSCustomClassServerName => $"{USSCustomClassServerButton}__server-name";

        public VisualElement Icon = new VisualElement { name = "icon" };

        public ServerButton_C() { }

        protected override void AttachStyleSheet()
        {
            base.AttachStyleSheet();
            this.AddStyleSheetFromPath(StyleSheetMenuPath);
            this.AddStyleSheetFromPath(StyleSheetServerPath);
        }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassServerButton);
            Icon.AddToClassList(USSCutomClassIcon);
            LabelVisual.AddToClassList(USSCustomClassServerName);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Front.RemoveFromHierarchy();
            Add(Icon);
        }

        protected override void StartElement()
        {
            Add(Icon);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            LabelDirection = ElemnetDirection.Bottom;
            Type = ButtonType.Invisible;
        }

        #region Implementation

        public ServerPreferences.ServerData Data;

        #endregion
    }
}
