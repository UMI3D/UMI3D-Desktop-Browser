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

namespace umi3d.commonScreen
{
    public class VBox : BaseVisual_C
    {
        public float Spacing;
        public ElementHorizontalAlignment Alignment;

        public VBox(ElementHorizontalAlignment alignment = ElementHorizontalAlignment.Center, float spacing = 0f)
        {
            this.style.position = Position.Absolute;
            this.style.justifyContent = Justify.Center;
            this.style.alignItems = Align.Center;

            this.Spacing = spacing;
            this.Alignment = alignment;
        }

        protected override void AttachStyleSheet()
        {
        }

        public new void Add(VisualElement child)
        {
            base.Add(child);
            child.RegisterCallback<GeometryChangedEvent>(ChildGeometrChanged);
        }

        public new void Remove(VisualElement child)
        {
            base.Remove(child);
            child.UnregisterCallback<GeometryChangedEvent>(ChildGeometrChanged);
        }

        protected void ChildGeometrChanged(GeometryChangedEvent evt)
        {
            if (evt.newRect.width.EqualsEpsilone(evt.oldRect.width)) return;
            if (evt.newRect.height.EqualsEpsilone(evt.oldRect.height)) return;

            // TODO is the size of the box constraint.

            var maxWidth = 0f;
            var height = 0f;
            var i = 0;
            foreach (var child in Children())
            {
                // Height
                child.style.top = height;
                height += child.layout.height;
                if (childCount > 1 && i != childCount - 1) height += Spacing;

                // Width
                if (child.layout.width > maxWidth) maxWidth = child.layout.width;

                ++i;
            }

            this.style.width = maxWidth;
            this.style.height = height;

            foreach (var child in Children())
            {
                switch (Alignment)
                {
                    case ElementHorizontalAlignment.Leading:
                        child.style.left = 0f;
                        break;
                    case ElementHorizontalAlignment.Center:
                        child.style.left = (maxWidth - child.layout.width) / 2f;
                        break;
                    case ElementHorizontalAlignment.Trailing:
                        child.style.left = maxWidth - child.layout.width;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
