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
using UnityEngine;

namespace umi3d.commonScreen.Container
{
    public class Carrousel_C : CustomCarrousel
    {
        public new class UxmlFactory : UxmlFactory<Carrousel_C, UxmlTraits> { }

        public Carrousel_C() => Set();

        public Carrousel_C(ElementCategory category, int nb_elt, float min_margin, EltSize elt_size, bool scroll_all, bool loop, int auto_scroll)
         => Set(category, nb_elt, min_margin, elt_size, scroll_all, loop, auto_scroll);

        public override void InitElement()
        {
            if (Prev == null) Prev = new Displayer.Button_C();
            if (Next == null) Next = new Displayer.Button_C();

            base.InitElement();
        }
    }
}
