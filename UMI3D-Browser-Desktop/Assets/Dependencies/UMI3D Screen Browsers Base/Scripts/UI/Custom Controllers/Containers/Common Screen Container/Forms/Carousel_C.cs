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
        public virtual string USSCustomClassBottomButtons => $"{USSCustomClassName}-bottom_buttons";
        public virtual string USSCustomClassSelectHover => $"{USSCustomClassName}-select_hover";
        public virtual string USSCustomClassSelectActive => $"{USSCustomClassName}-select_active";


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
            BottomButtonsContainer.AddToClassList(USSCustomClassBottomButtons);

            ElementsContainer.AddToClassList(USSCustomClassBoxElts);
        }
        #endregion

        private int _nbrElementTotal;
        private int _nbrElementShown;
        private int _currentElementIndex;
        private bool _isLoop;

        private float _elementsWidth;

        private bool _isAnimationEnabled = true;
        private bool _isInitialized = false;

        public Button_C ButtonPrevious = new Button_C { name = "precedent" };
        public Button_C ButtonNext = new Button_C { name = "next" };
        public VisualElement IconPrevious = new VisualElement();
        public VisualElement IconNext = new VisualElement();
        public VisualElement Container = new VisualElement();
        public VisualElement ElementsContainer = new VisualElement();
        public VisualElement BottomButtonsContainer = new VisualElement();
        public List<VisualElement> BottomButtons = new List<VisualElement>();

        public void Setup(List<VisualElement> elements, int nbrElementShown = 1, bool isLoop = true)
        {
            if (!_isInitialized)
            {
                InitStyleSheet();
                InitElement();
                _isInitialized = true;
            }
            _nbrElementShown = nbrElementShown;
            _isLoop = isLoop;
            var count = 0;
            foreach (var element in elements)
            {
                ElementsContainer.Add(element);

                var button = new VisualElement();
                button.AddToClassList(USSCustomClassSelectHover);
                var tmp_i = count;
                button.RegisterCallback<ClickEvent>((ce) => Goto(tmp_i));
                BottomButtons.Add(button);
                BottomButtonsContainer.Insert(count, button);

                count++;
            }
            _nbrElementTotal = count;

            ElementsContainer.RegisterCallback<GeometryChangedEvent>(CalculateElementsSize);

            if (BottomButtons.Count > 0)
                BottomButtons[0].AddToClassList(USSCustomClassSelectActive);

            _currentElementIndex = 0;
        }

        public virtual void InitElement()
        {
            ButtonPrevious.ClickedUp += () => Goto(_currentElementIndex - 1);
            ButtonNext.ClickedUp += () => Goto(_currentElementIndex + 1);

            ButtonPrevious.Add(IconPrevious);
            ButtonNext.Add(IconNext);

            Container.Add(ElementsContainer);
            Container.Add(BottomButtonsContainer);

            Add(ButtonPrevious);
            Add(Container);
            Add(ButtonNext);
        }

        private void CalculateElementsSize(GeometryChangedEvent ec)
        {
            _elementsWidth = ec.newRect.width / _nbrElementShown;

            foreach (var element in ElementsContainer.Children())
            {
                element.style.minWidth = _elementsWidth;
                element.style.maxWidth = _elementsWidth;
            }
        }

        private void Goto(int newIndex)
        {
            BottomButtons[_currentElementIndex].RemoveFromClassList(USSCustomClassSelectActive);

            Debug.Log(_currentElementIndex);
            Debug.Log(newIndex);
            _currentElementIndex = newIndex;
            Debug.Log(_currentElementIndex);
            if (_currentElementIndex < 0) _currentElementIndex = _isLoop ? _nbrElementTotal - 1 : 0;
            if (_currentElementIndex > _nbrElementTotal - _nbrElementShown) _currentElementIndex = _isLoop ? 0 : _nbrElementTotal - _nbrElementShown;
            Debug.Log(_currentElementIndex);

            BottomButtons[_currentElementIndex].AddToClassList(USSCustomClassSelectActive);

            if (_isAnimationEnabled)
                ElementsContainer.experimental.animation.TopLeft(new Vector2(-_currentElementIndex * _elementsWidth, 0), 500);
            else
                ElementsContainer.style.left = -_currentElementIndex * _elementsWidth;
        }

        public void SetAnimationActive(bool active)
        {
            _isAnimationEnabled = active;
        }
    }
}
