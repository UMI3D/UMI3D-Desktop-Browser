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
using umi3d.commonScreen.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Container
{
    public class ScrollableDataCollection_C<D> : CustomScrollableDataCollection<D>
    {
        public new class UxmlFactory : UxmlFactory<ScrollableDataCollection_C<D>, UxmlTraits> { }

        public ScrollableDataCollection_C() => Set();

        public override void InitElement()
        {
            if (ScrollView == null) ScrollView = new ScrollView_C();

            base.InitElement();
        }
    }
}
