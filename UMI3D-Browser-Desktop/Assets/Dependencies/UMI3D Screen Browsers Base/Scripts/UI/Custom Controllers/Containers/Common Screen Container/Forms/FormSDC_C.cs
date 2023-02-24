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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    /// <summary>
    /// A formulaire container for scrollable data collection. See <see cref="ScrollableDataCollection_C{D}"/>
    /// </summary>
    /// <typeparam name="D"></typeparam>
    public class FormSDC_C<D> : BaseScrollableForm_C
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ElementCategory Category
        {
            get => base.Category;
            set
            {
                base.Category = value;
                SDC.Category = value;
            }
        }

        public virtual string USSCustomClassFormSDC => "form__sdc";

        public ScrollableDataCollection_C<D> SDC = new ScrollableDataCollection_C<D> { name = "sdc" };

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            AddToClassList(USSCustomClassFormSDC);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(SDC);
        }
    }
}
