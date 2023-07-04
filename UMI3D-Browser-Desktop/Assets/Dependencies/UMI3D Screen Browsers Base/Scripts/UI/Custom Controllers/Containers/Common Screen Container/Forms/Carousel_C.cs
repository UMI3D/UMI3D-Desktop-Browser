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
using UnityEngine.UIElements;
using UnityEngine;
using umi3d.commonScreen.Displayer;
using System.Collections.Generic;
using System.Linq;

namespace umi3d.commonScreen.Container
{
    public class Carousel_C : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Carousel_C, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {

            protected UxmlIntAttributeDescription _nbrElementShown = new UxmlIntAttributeDescription
            {
                name = "number-of-elements-shown",
                defaultValue = 1
            };

            protected UxmlBoolAttributeDescription _isLoop = new UxmlBoolAttributeDescription
            {
                name = "loop",
                defaultValue = false
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;
                base.Init(ve, bag, cc);
                var custom = ve as Carousel_C;

                custom.Setup(new List<VisualElement>());
            }
        }

        #region Style
        public virtual string StyleSheetContainerPath => $"USS/container";
        public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetContainersFolderPath}/carrousel";

        public virtual string USSCustomClassName => "carrousel";
        public virtual string USSCustomClassButton => $"{USSCustomClassName}-button";
        public virtual string USSCustomClassContainerBox => $"{USSCustomClassName}-container_box";
        public virtual string USSCustomClassBoxElts => $"{USSCustomClassName}-box_elts";
        public virtual string USSCustomClassPrevIcon => $"{USSCustomClassName}-prev_icon";
        public virtual string USSCustomClassNextIcon => $"{USSCustomClassName}-next_icon";


        private void InitStyleSheet()
        {
            try
            {
                this.AddStyleSheetFromPath(StyleSheetContainerPath);
                this.AddStyleSheetFromPath(StyleSheetPath);
            }
            catch (System.Exception e)
            {
                throw e;
            }

            AddToClassList(USSCustomClassName);
            Container.AddToClassList(USSCustomClassContainerBox);

            ButtonPrevious.AddToClassList(USSCustomClassButton);
            ButtonNext.AddToClassList(USSCustomClassButton);
            IconPrevious.AddToClassList(USSCustomClassPrevIcon);
            IconNext.AddToClassList(USSCustomClassNextIcon);

            ElementsContainer.AddToClassList(USSCustomClassBoxElts);
        }
        #endregion

        private int _nbrElementTotal;
        private int _nbrElementShown;
        private int _currentElementIndex;
        private bool _isLoop;

        private float _elementsWidth;

        private bool _isInitialized = false;

        public Button_C ButtonPrevious = new Button_C { name = "precedent" };
        public Button_C ButtonNext = new Button_C { name = "next" };
        public VisualElement IconPrevious = new VisualElement();
        public VisualElement IconNext = new VisualElement();
        public VisualElement Container = new VisualElement();
        public VisualElement ElementsContainer = new VisualElement();

        public void Setup(List<VisualElement> elements, int nbrElementShown = 1, bool isLoop = true)
        {
            if (!_isInitialized)
            {
                InitStyleSheet();
                InitElement();
                _isInitialized = true;
            }

            _nbrElementTotal = elements.Count();
            _nbrElementShown = nbrElementShown;
            _isLoop = isLoop;

            foreach (var element in elements)
            {
                ElementsContainer.Add(element);
            }

            ElementsContainer.RegisterCallback<GeometryChangedEvent>(CalculateElementsSize);

            _currentElementIndex = 0;
        }

        public virtual void InitElement()
        {
            ButtonPrevious.ClickedUp += Previous;
            ButtonNext.ClickedUp += Next;

            ButtonPrevious.Add(IconPrevious);
            ButtonNext.Add(IconNext);

            Container.Add(ElementsContainer);

            Add(ButtonPrevious);
            Add(Container);
            Add(ButtonNext);
        }

        private void CalculateElementsSize(GeometryChangedEvent ec)
        {
            _elementsWidth = ec.newRect.width / _nbrElementShown;

            foreach (var element in ElementsContainer.Children())
            {
                element.style.width = new StyleLength(_elementsWidth);
            }
        }

        private void Previous()
        {
            _currentElementIndex -= _nbrElementShown;
            if (_currentElementIndex < 0) _currentElementIndex = _isLoop ? _nbrElementTotal - 1 : 0;
            ElementsContainer.experimental.animation.TopLeft(new Vector2(-_currentElementIndex * _elementsWidth, 0), 500);
        }

        private void Next()
        {
            _currentElementIndex += _nbrElementShown;
            if (_currentElementIndex > _nbrElementTotal - _nbrElementShown) _currentElementIndex = _isLoop ? 0 : _nbrElementTotal - _nbrElementShown;
            ElementsContainer.experimental.animation.TopLeft(new Vector2(-_currentElementIndex * _elementsWidth, 0), 500);
        }
    }
}
