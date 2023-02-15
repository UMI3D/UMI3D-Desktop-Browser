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
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class Form_C : BaseScrollableForm_C
    {
        public new class UxmlFactory : UxmlFactory<Form_C, UxmlTraits> { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ElementCategory Category
        {
            get => base.Category;
            set
            {
                base.Category = value;
                VScroll.Category = value;
            }
        }

        public virtual string USSCustomClassForm => "form";

        public ScrollView_C VScroll = new ScrollView_C { name = "scroll-view" };

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassForm);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(VScroll);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override VisualElement contentContainer => IsSet && VScroll != null ? VScroll.contentContainer : this;
    }
}
